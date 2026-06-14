# CameraView

A cross-platform camera control for **.NET MAUI** (iOS, Android, Windows, macOS AppKit) and **Blazor WebAssembly**. Live preview with zoom, torch, lens/device selection, photo + video capture, and live color filters — plus a pluggable **frame-analysis pipeline**. Each analyzer raises its **own strongly-typed event** for its result (barcode value, faces, recognized text, a structured document) and returns **styled bounding boxes** (`OverlayBox`) that the overlay draws.

- **MAUI core**: `Shiny.Maui.Controls.Camera` — `CameraView` (handler-based `View`) + `CameraOverlayView` (drop-in box overlay), backed by AVFoundation (Apple), CameraX (Android), Media Capture (Windows). Register with `.UseShinyCamera()`.
- **Blazor core**: `Shiny.Blazor.Controls.Camera` — `<CameraView>` component over `getUserMedia` / `MediaRecorder` / `BarcodeDetector`.
- **Analyzer add-ons** (MAUI): `Shiny.Maui.Controls.Camera.Barcode`, `.Camera.Face`, `.Camera.Motion`, `.Camera.Ocr`, `.Camera.Documents` (Invoice / DriversLicense / HealthCard). Add only what you need.
- **Shared contracts**: `Shiny.Controls.Camera` namespace — `IFrameAnalyzer`, `FrameAnalyzer` (base), `OverlayBox`, `CameraFrame`, `CoordinateTransform`, the document building blocks (`RecognizedText`, `DocumentField`, `DocumentLineItem`, `DocumentDetectedEventArgs<T>`, `IDocumentParser<T>`), and the enums (`CameraFacing`, `CameraFilter`, `CameraFlashMode`, `PreviewScaleMode`).

**Two channels per analyzer:** a typed *event* carries the semantic result; the *return value* of `AnalyzeAsync` is the styled `OverlayBox`es to draw. A returned set persists until the analyzer returns a different set (replace) or `null` (clear).

## Installation

### .NET MAUI

```bash
dotnet add package Shiny.Maui.Controls.Camera
# optional analyzers
dotnet add package Shiny.Maui.Controls.Camera.Barcode
dotnet add package Shiny.Maui.Controls.Camera.Face
dotnet add package Shiny.Maui.Controls.Camera.Motion
dotnet add package Shiny.Maui.Controls.Camera.Ocr
dotnet add package Shiny.Maui.Controls.Camera.Documents   # Invoice / DriversLicense / HealthCard
```

```csharp
builder
    .UseShinyControls()
    .UseShinyCamera();
```

```xml
xmlns:cam="http://shiny.net/maui/camera"
```

**Permissions (consumer responsibility):**
- iOS / Mac Catalyst / macOS — `NSCameraUsageDescription` (+ `NSMicrophoneUsageDescription` for video-with-audio) in `Info.plist`; macOS camera entitlement when sandboxed.
- Android — `<uses-permission android:name="android.permission.CAMERA" />` (+ `RECORD_AUDIO` for audio). **Minimum SDK 23** (CameraX).
- Windows — `webcam` (+ `microphone`) capability in `Package.appxmanifest`.

### Blazor

```bash
dotnet add package Shiny.Blazor.Controls.Camera
```

```razor
@using Shiny.Blazor.Controls.Camera
@using Shiny.Controls.Camera
```

`getUserMedia` requires a secure context (HTTPS or `localhost`).

## Basic Usage — MAUI

```xml
<cam:CameraView x:Name="Camera"
                Facing="Back"
                ScaleMode="AspectFill"
                Zoom="1"
                IsTorchOn="False"
                Filter="None" />
```

```csharp
// The preview auto-starts when the view is added (IsActive defaults true) and the control requests camera
// permission itself — handle a denial (or any error) via CameraError. Toggle IsActive for lifecycle; it is
// safe to set any time (unlike StartAsync/RequestPermissionAsync, which need the handler already connected).
this.Camera.CameraError += (_, e) => status = e.Message;   // e.g. "Camera permission denied"

protected override void OnAppearing()
{
    base.OnAppearing();
    this.Camera.IsActive = true;    // resume (no-op on first show — it already auto-started)
}

protected override void OnDisappearing()
{
    base.OnDisappearing();
    this.Camera.IsActive = false;   // release the camera while off-screen
}

// Capture a still photo — the current Filter is baked into the JPEG, so preview and photo match
CameraPhoto photo = await this.Camera.CapturePhotoAsync();   // photo.Data is JPEG bytes

// Record video (audio optional, defaults on). NOTE: recorded video is NOT filtered (it records the raw feed)
await this.Camera.StartVideoRecordingAsync(new VideoRecordingOptions { IncludeAudio = true });
CameraVideo video = await this.Camera.StopVideoRecordingAsync();   // video.FilePath

// Flip lens
this.Camera.Facing = this.Camera.Facing == CameraFacing.Back ? CameraFacing.Front : CameraFacing.Back;

// Live filter — applied to the preview AND to captured photos (Apple + Android; not recorded video, not Windows)
this.Camera.Filter = CameraFilter.Noir;
```

> `RequestPermissionAsync()` / `StartAsync()` / `StopAsync()` / `GetAvailableCamerasAsync()` are still available
> for explicit control, but they route through the platform handler and **no-op (return `false`/empty) if the
> handler isn't connected yet** — e.g. when called in `OnAppearing` on first show. Don't gate startup on them;
> prefer `IsActive` + `CameraError`. Call `GetAvailableCamerasAsync()` once the view is loaded (e.g. from a
> `Loaded`/handler-ready hook) so the lens list isn't empty.

### Selecting a specific camera

`Facing` picks by position; `CameraId` pins an exact device (multiple back lenses, USB webcams on macOS).

```csharp
IReadOnlyList<CameraInfo> cameras = await this.Camera.GetAvailableCamerasAsync();
this.Camera.CameraId = cameras.First(c => c.Name.Contains("USB")).Id;   // null => fall back to Facing
```

## Frame Analysis (the differentiator)

Add `IFrameAnalyzer`s to `Camera.Analyzers`. The pipeline streams frames off the UI thread with per-analyzer drop-on-busy back-pressure. **Subscribe to each analyzer's own typed event** for the result; the boxes are drawn for you by `CameraOverlayView`.

```csharp
using Shiny.Maui.Controls.Camera.Barcode;
using Shiny.Maui.Controls.Camera.Documents;
using Shiny.Maui.Controls.Camera.Motion;
using FaceAnalyzer = Shiny.Maui.Controls.Camera.Face.FaceAnalyzer;

var barcode = new BarcodeAnalyzer();                          // ZXing.Net over luminance — all platforms
barcode.BarcodeDetected += (_, e) => status = $"{e.Format}: {e.Value}";

var faces = new FaceAnalyzer();                               // Apple Vision / MLKit / Windows.FaceAnalysis
faces.FacesDetected += (_, e) => status = $"{e.Faces.Count} face(s)";

var motion = new MotionAnalyzer();                            // pure-managed frame differencing
motion.MotionChanged += (_, e) =>                             // e.Regions = a box per distinct moving area
    status = e.InMotion ? $"Motion in {e.Regions.Count} area(s)" : "Still";

Camera.Analyzers.Add(barcode);
Camera.Analyzers.Add(faces);
Camera.Analyzers.Add(motion);
```

Analyzer events are raised on the UI thread (the pipeline marshals them), so handlers can touch UI directly.

### Enabling / disabling analyzers

`Analyzers` is an observable collection — **add or remove at runtime and the running pipeline picks it up live**
(seamless on Apple/Windows; Android rebinds its capture use-cases automatically). To turn an analyzer off
*without losing its bindings/state*, set **`FrameAnalyzer.IsEnabled`** (bool, default `true`) instead of
removing it — its command/event bindings stay wired and it resumes instantly when re-enabled. When **every**
analyzer is disabled the camera behaves as if it had none (so, e.g., Android can record video again).

```xml
<cam:CameraView Facing="Back">
    <!-- toggle live from a switch; binding + state are preserved while off -->
    <cam:BarcodeAnalyzer BarcodeDetectedCommand="{Binding ScanCommand}"
                         IsEnabled="{Binding IsToggled, Source={x:Reference ScanSwitch}}" />
</cam:CameraView>
```

`IsEnabled` (run or not) is distinct from `ShowBoundingBox` (run, but draw nothing).

### Declaring analyzers in XAML + Commands (MVVM)

Analyzers are `BindableObject`s, so you can declare them inside `<cam:CameraView>` (its content property is
`Analyzers`) under the one `cam:` prefix, and bind each analyzer's **`…Command`** to a ViewModel — the
command fires (on the UI thread) with the same args as the event. They inherit the camera's `BindingContext`.

```xml
<cam:CameraView Facing="Back" Filter="Chrome">
    <cam:BarcodeAnalyzer BarcodeDetectedCommand="{Binding ScanCommand}" />
    <cam:InvoiceAnalyzer DocumentDetectedCommand="{Binding InvoiceCommand}" />
    <!-- run an analyzer for its command only, no box: -->
    <cam:MotionAnalyzer MotionChangedCommand="{Binding MotionCommand}" ShowBoundingBox="False" />
</cam:CameraView>
```

Each analyzer exposes the command matching its event: `BarcodeDetectedCommand`, `MotionChangedCommand`,
`FacesDetectedCommand`, `TextRecognizedCommand`, and `DocumentDetectedCommand` (documents). Bind a
`Command<T>` whose `T` is that analyzer's event-args type. Commands are MAUI-only (Blazor uses
`EventCallback`s).

### Controlling the overlay per analyzer

- `IsEnabled` (bool, default `true`) — run the analyzer or not (see *Enabling / disabling analyzers* above).
- `ShowBoundingBox` (bool, default `true`) — set `False` to run an analyzer purely for its event/command
  and draw nothing. Every built-in analyzer honors it, so it disables box rendering for any of them.
- `OverlayProvider` (code-level `Func<TArgs, IReadOnlyList<OverlayBox>?>`) — return the exact boxes to draw
  for a detection, or `null` for none. When unset, the analyzer draws its own default styled box.

```csharp
barcode.OverlayProvider = e => e.Value.StartsWith("OK")
    ? [ new OverlayBox(e.BoundingBox, Colors.Lime, e.Value) ]
    : null;   // don't box anything that doesn't start with "OK"
```

### Drawing bounding boxes

Drop a `CameraOverlayView` over the `CameraView` in the same cell and point it at the camera — it auto-subscribes and redraws. Each analyzer styles its own boxes (color/text); `Default*` fills in anything unset.

```xml
<Grid>
    <cam:CameraView x:Name="Camera" ScaleMode="AspectFill" />
    <cam:CameraOverlayView Camera="{x:Reference Camera}" InputTransparent="True" />
</Grid>
```

(Low-level alternative: a `GraphicsView` + `CameraOverlayDrawable`, fed from `Camera.OverlaysChanged` — only needed for custom rendering.)

### Motion — localized regions

`MotionAnalyzer` clusters movement into **separate regions**, so motion in two spots yields two boxes rather
than one box spanning both. `MotionChanged` carries `MotionEventArgs.Regions` (one normalized `RectF` per
moving area) plus `Region` (their union, for back-compat) and `Intensity` (0..1). Tune detection with
`PixelThreshold` / `AreaThreshold` (overall trigger), `SampleStride` (speed), and `GridColumns` /
`CellThreshold` (how finely motion is split into regions). Boxes still honor `ShowBoundingBox` and
`OverlayProvider`.

### Documents — typed events (Invoice, DriversLicense, HealthCard)

Each document type is its **own analyzer with its own strongly-typed event** (`DocumentDetected` typed to its payload). Every payload is a strong record with **nullable fields** (only what was found is set). Ships `InvoiceAnalyzer` (`Invoice`, order lines in `.Lines`), `DriversLicenseAnalyzer` (`DriversLicense`), `HealthCardAnalyzer` (`HealthCard`), `CreditCardAnalyzer` (`CreditCard`), `PassportAnalyzer` (`Passport`). Order lines come back as `Invoice.Lines`.

```csharp
var invoice = new InvoiceAnalyzer();                         // OCR + rules
invoice.DocumentDetected += (_, e) =>
{
    Invoice doc = e.Document;
    status = $"Invoice {doc.Number} — total {doc.Total}, {doc.Lines.Count} line(s)";
};

var license = new DriversLicenseAnalyzer();                 // PDF417 + AAMVA parse (deterministic)
license.DocumentDetected += (_, e) =>
    status = $"{e.Document.FirstName} {e.Document.LastName} — {e.Document.Number}";

Camera.Analyzers.Add(invoice);
Camera.Analyzers.Add(license);
```

- **Driver's licenses** are decoded from the back's **PDF417 barcode** and parsed against the **AAMVA** standard — deterministic, no ML.
- **Passports** parse the **MRZ** (the two `<<<` lines, ICAO TD3) → `Passport` (number, surname, given names, nationality, issuing country, DOB, expiry, sex) — MRZ parse is deterministic.
- **Credit cards**: `CreditCard.Type` (Visa/Mastercard/Amex/…) and number validity come from the IIN prefix + Luhn (deterministic); name/expiry/company are best-effort OCR. `Cvv` is on the back and PCI-sensitive, so it's almost always `null` from a front scan.
- **Invoices / health cards / credit cards** are OCR + best-effort rules. Swap the rules by passing a custom `IDocumentParser<T>`:
  `new InvoiceAnalyzer(new MyInvoiceParser())`, where `MyInvoiceParser : IDocumentParser<Invoice>` returns the typed payload + the boxes to draw. (`OcrAnalyzer` itself just raises `TextRecognized` with raw `RecognizedText` blocks.)

**Payload records** (all data properties nullable; enums default to `Unknown`/`Unspecified` — populated only when found):

```csharp
record Invoice(string? Number, DateOnly? Date, decimal? Total, IReadOnlyList<InvoiceLine> Lines, IReadOnlyList<DocumentField> Fields);
record InvoiceLine(string? Description, decimal? Quantity, decimal? UnitPrice, decimal? Amount, RectF? Bounds);
record DriversLicense(string? Number, string? FirstName, string? LastName, DateOnly? DateOfBirth, DateOnly? Expiry, string? Address, IReadOnlyList<DocumentField> Fields);
record HealthCard(string? Number, string? Name, DateOnly? Expiry, string? Issuer, IReadOnlyList<DocumentField> Fields);
record CreditCard(CreditCardType Type, string? Number, DateOnly? Expiry, string? FirstName, string? LastName, string? CompanyName, string? Cvv, IReadOnlyList<DocumentField> Fields);
enum CreditCardType { Unknown, Visa, Mastercard, Amex, Discover, DinersClub, JCB, UnionPay, Maestro }
record Passport(string? Number, string? Surname, string? GivenNames, string? Nationality, string? IssuingCountry, DateOnly? DateOfBirth, DateOnly? Expiry, PassportSex Sex, IReadOnlyList<DocumentField> Fields);
enum PassportSex { Unspecified, Male, Female }
record DocumentField(string Label, string? Value, RectF? Bounds, float Confidence);   // each payload's Fields bag
```

## Basic Usage — Blazor

```razor
<CameraView @ref="camera"
            Facing="CameraFacing.Back"
            EnableBarcode="true"
            ShowOverlay="true"
            Filter="filter"
            BarcodeDetected="OnBarcode"
            OnError="m => status = m"
            Style="width:100%;height:100%;" />

@code {
    CameraView? camera;
    CameraFilter filter = CameraFilter.None;
    string? status;

    void OnBarcode(CameraBarcode b) => status = $"{b.Format}: {b.Value}";

    async Task Photo() { var jpeg = await camera!.CapturePhotoAsync(); }            // byte[]
    async Task Rec()   { await camera!.StartRecordingAsync(includeAudio: true); }
    async Task Stop()  { var webm = await camera!.StopRecordingAsync(); }           // byte[] WebM
}
```

Blazor barcode scanning uses the browser `BarcodeDetector` (Chromium only); on unsupported browsers `OnError` fires once and preview continues. The component also exposes `OverlaysChanged` (`IReadOnlyList<OverlayBox>`) if you want to draw boxes yourself; `ShowOverlay="true"` already draws them on the JS canvas.

## Properties (MAUI `CameraView`)

| Property | Type | Default | Description |
|---|---|---|---|
| `Facing` | `CameraFacing` | `Back` | `Back` / `Front` / `External` |
| `CameraId` | `string?` | `null` | Exact device id (overrides `Facing`); from `GetAvailableCamerasAsync()` |
| `IsActive` | `bool` | `true` | Whether the session runs |
| `Zoom` | `double` | `1` | Clamped to `MinZoom`..`MaxZoom` |
| `MinZoom` / `MaxZoom` | `double` | `1` | Reported zoom range |
| `IsTorchOn` | `bool` | `false` | Continuous torch |
| `FlashMode` | `CameraFlashMode` | `Off` | Still-capture flash (`Off`/`On`/`Auto`) |
| `ScaleMode` | `PreviewScaleMode` | `AspectFill` | `AspectFill` / `AspectFit` |
| `Filter` | `CameraFilter` | `None` | `None`/`Mono`/`Noir`/`Sepia`/`Invert`/`Vivid`/`Cool`/`Warm`/`Fade`/`Chrome`/`Instant`/`Tonal` — applied to preview + captured photos (not recorded video; no-op on Windows) |
| `ShowDetectionOverlay` | `bool` | `true` | Surface overlay boxes for the overlay |
| `Analyzers` | `IList<IFrameAnalyzer>` | empty | Frame analyzers — add/remove live; toggle one via its `IsEnabled` |
| `IsRecording` | `bool` (get) | `false` | Recording in progress |
| `Overlays` | `IReadOnlyList<OverlayBox>` | empty | Latest aggregated overlay boxes (read-only) |

**Methods:** `RequestPermissionAsync` · `StartAsync` · `StopAsync` · `CapturePhotoAsync` → `CameraPhoto` · `StartVideoRecordingAsync` / `StopVideoRecordingAsync` → `CameraVideo` · `GetAvailableCamerasAsync` → `IReadOnlyList<CameraInfo>`.
**Events:** `MediaCaptured` · `VideoCaptured` · `OverlaysChanged` (presentation only) · `CameraError`. For results, subscribe to each analyzer's own typed event (`BarcodeDetected`, `FacesDetected`, `MotionChanged`, `TextRecognized`, `DocumentDetected`).

## Code-Generation Rules

- XAML namespace is `xmlns:cam="http://shiny.net/maui/camera"`; prefix `cam` is the assembly default.
- The preview auto-starts (`IsActive` defaults `true`) and requests permission itself — handle denials via `CameraError` and toggle `IsActive` for lifecycle. `RequestPermissionAsync`/`StartAsync`/`GetAvailableCamerasAsync` route through the handler and no-op/return empty if called before the view is rendered (e.g. in `OnAppearing` on first show), so **don't gate startup on `RequestPermissionAsync()`** — that returns `false` when the handler isn't connected yet and looks like a denied permission. Add the platform permission entries (see Installation) — omitting `NSCameraUsageDescription` crashes iOS instantly.
- `CameraView` is the native preview surface. To draw boxes, layer a `CameraOverlayView` over it in the same `Grid` cell with `Camera="{x:Reference Camera}"` — don't try to draw inside the `CameraView` itself.
- Subscribe to each analyzer's **typed event** for results (`BarcodeAnalyzer.BarcodeDetected`, `FaceAnalyzer.FacesDetected`, `MotionAnalyzer.MotionChanged`, `OcrAnalyzer.TextRecognized`, `*Analyzer.DocumentDetected`). Don't read semantic data off `OverlaysChanged` — that channel is presentation only.
- `OverlayBox.Rect` is normalized (0..1), upright, mirror-corrected. The overlay converts via `CoordinateTransform.MapToView(...)` — never assume raw pixel coordinates.
- `BarcodeAnalyzer` is pure-managed (ZXing) and works on every platform; `DriversLicenseAnalyzer` (PDF417/AAMVA) is also pure-managed. `FaceAnalyzer`, `OcrAnalyzer`, and the OCR-backed document analyzers (`InvoiceAnalyzer`, `HealthCardAnalyzer`) need native OCR/ML and only produce results on iOS/Android/Windows/macOS (not bare `net10.0`). `MotionAnalyzer` is managed and works everywhere.
- Custom analyzers should derive from `FrameAnalyzer` (not implement `IFrameAnalyzer` directly) so typed events marshal to the UI thread, they get `IsEnabled` + `ShowBoundingBox`, and their `Command`s bind in XAML. Raise results with `Emit(raiseEvent, command, args)` and return boxes via `ResolveOverlay(args, OverlayProvider, () => defaultBoxes)` — that path already suppresses boxes when `ShowBoundingBox` is `false`.
- All analyzers live under the single `xmlns:cam="http://shiny.net/maui/camera"` prefix and can be declared inside `<cam:CameraView>` (content property = `Analyzers`). Bind results with `…Command="{Binding …}"`; the analyzer inherits the camera's `BindingContext`.
- Invoice/health-card parsing is **best-effort rules** — swap accuracy in via a custom `IDocumentParser<T>`. Driver's-license parsing is deterministic (AAMVA).
- Use `QRCodeView`/`BarcodeView` from `Shiny.Maui.Controls.Barcodes` to *render* (generate) a code; use the CameraView `BarcodeAnalyzer` to *scan* one. They are different packages for different jobs.

## Common Pitfalls

- **Android: video + analyzers together** — CameraX caps concurrent use-cases, so the camera binds either `ImageAnalysis` (while any analyzer is **enabled**) or `VideoCapture`, not both. To record, disable every analyzer (`IsEnabled = false`) or clear `Camera.Analyzers` — the camera rebinds automatically; `StartVideoRecordingAsync` throws a clear error while an enabled analyzer is attached.
- **Filters affect preview + photos, not video** — `Filter` is baked into the live preview and the `CapturePhotoAsync` JPEG, but **recorded video records the unfiltered feed**. Windows has no live filter at all (preview and photo are unfiltered there).
- **"Camera permission denied" but the preview works** — you're gating on `RequestPermissionAsync()` in `OnAppearing` before the handler is connected (it returns `false` → looks denied, and the early-return also leaves the lens list empty). Don't gate on it; rely on auto-start + `CameraError`, and load cameras once the view is loaded.
- **Android minSdk** — CameraX requires API 23+. Set `<SupportedOSPlatformVersion>` to 23 or higher in the consuming app or you'll get a manifest-merge error.
- **macOS** — best-effort host; multiple webcams enumerate via `GetAvailableCamerasAsync()`; FaceTime + USB devices report an `External`/unspecified facing, so use `CameraId` rather than `Facing` to choose.
- **Blazor barcode on Firefox/Safari** — `BarcodeDetector` is Chromium-only; feature-detect and provide a fallback if you need universal coverage. Filters (CSS) and capture/record work everywhere.
- **Overlay misaligned** — prefer `CameraOverlayView` (it tracks `ScaleMode`/aspect for you). If you hand-roll a `CameraOverlayDrawable`, set `ScaleMode` to match `CameraView.ScaleMode` and `ImageAspect` from `CameraOverlaysChangedEventArgs.ImageWidth/Height`.
- **Box flicker / not persisting** — an analyzer's boxes stay drawn until it returns a *different* set or `null`; return the same set (or nothing while busy) to keep them. Returning `null`/empty every frame clears them.

## When to Use What

- **Just take a photo / record video** → `CapturePhotoAsync` / `StartVideoRecordingAsync`; no analyzers needed.
- **Scan a barcode/QR** → add `BarcodeAnalyzer` (or set `EnableBarcode` on Blazor).
- **Detect/box faces** → add `FaceAnalyzer`.
- **Trigger on movement (security cam)** → add `MotionAnalyzer` and handle `MotionChanged`.
- **Read raw text** → add `OcrAnalyzer` and handle `TextRecognized`.
- **Parse a receipt/invoice** → add `InvoiceAnalyzer` and handle `DocumentDetected` (`Invoice` with `.Lines`).
- **Scan a driver's license / health card** → add `DriversLicenseAnalyzer` (deterministic AAMVA) or `HealthCardAnalyzer` from `.Camera.Documents`.
- **Scan a passport** → add `PassportAnalyzer` (deterministic MRZ); **read a credit card** → add `CreditCardAnalyzer` (brand+number deterministic).
- **Apply a live look** → set `Filter`.
- **Pick a specific lens / webcam** → `GetAvailableCamerasAsync()` + `CameraId`.
