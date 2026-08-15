# Barcodes & QR Codes

Pure-managed 1D and 2D barcode renderer powered by ZXing.Net. Ships as separate add-on packages so apps that don't need barcodes don't pay for ZXing.

- **MAUI**: `Shiny.Maui.Controls.Barcodes` — `BarcodeView` and `QRCodeView` are `ContentView` subclasses that produce PNG bytes (via a pure-managed encoder, no SkiaSharp / `System.Drawing`) and feed an `Image`.
- **Blazor**: `Shiny.Blazor.Controls.Barcodes` — same two components rendered as inline `<svg>` by default (crisp at any size) or a PNG `data:` URI.
- **Shared core**: `Shiny.Controls.Barcodes` namespace exposes a static `BarcodeRenderer` for raw PNG / SVG / data-URI output if you don't want the view at all.

Supported formats (from `BarcodeFormat`): `QRCode`, `Aztec`, `DataMatrix`, `Pdf417`, `Code128`, `Code39`, `Code93`, `Codabar`, `Ean8`, `Ean13`, `UpcA`, `UpcE`, `Itf`.

## Installation

### .NET MAUI

```bash
dotnet add package Shiny.Maui.Controls.Barcodes
```

```xml
xmlns:bc="http://shiny.net/maui/barcodes"
```

No DI registration is required — the views are plain `ContentView`s.

### Blazor

```bash
dotnet add package Shiny.Blazor.Controls.Barcodes
```

```razor
@using Shiny.Blazor.Controls.Barcodes
@using Shiny.Controls.Barcodes
```

## Basic Usage — MAUI

```xml
<!-- Any 1D/2D symbology -->
<bc:BarcodeView Value="5901234123457"
                Format="Ean13"
                PixelWidth="400"
                PixelHeight="150"
                MarginPixels="10"
                ForegroundColor="Black"
                BarcodeBackgroundColor="White" />

<!-- QR shortcut with error correction -->
<bc:QRCodeView Value="https://shinylib.net"
               Size="300"
               ErrorCorrection="High" />
```

## Basic Usage — Blazor

```razor
<!-- SVG output (default) — crisp at any CSS size -->
<BarcodeView Value="5901234123457"
             Format="BarcodeFormat.Ean13"
             PixelWidth="400"
             PixelHeight="150" />

<!-- PNG output when you need a raw <img> -->
<BarcodeView Value="5901234123457"
             Format="BarcodeFormat.Ean13"
             ImageFormat="BarcodeImageFormat.Png"
             CssWidth="100%" CssHeight="80px" />

<!-- QR shortcut -->
<QRCodeView Value="https://shinylib.net"
            Size="300"
            QRErrorCorrection="QRErrorCorrection.High" />
```

`CssWidth` and `CssHeight` override the host element's CSS size — set them to `"100%"`, `vh` units, etc. while keeping `PixelWidth`/`PixelHeight` at the desired encoder resolution.

## Render Directly (No View)

For PDF generation, file export, email attachments, or anywhere you need raw bytes:

```csharp
using Shiny.Controls.Barcodes;

var opts = new BarcodeRenderOptions
{
    PixelWidth = 600,
    PixelHeight = 200,
    Margin = 10,
    ForegroundColor = "#000000",
    BackgroundColor = "#FFFFFF",
    QRErrorCorrection = QRErrorCorrection.High // QR only — ignored for other formats
};

byte[] png     = BarcodeRenderer.RenderPng("Hello", BarcodeFormat.QRCode, opts);
string svg     = BarcodeRenderer.RenderSvg("Hello", BarcodeFormat.QRCode, opts);
string dataUri = BarcodeRenderer.RenderDataUri("Hello", BarcodeFormat.QRCode, BarcodeImageFormat.Png, opts);
```

All three calls are static, stateless, and AOT-safe.

## Properties

### BarcodeView (MAUI)

| Property | Type | Default | Description |
|---|---|---|---|
| `Value` | `string` | `""` | Content to encode. Empty clears the image. Invalid content for the chosen format silently clears too |
| `Format` | `BarcodeFormat` | `Code128` | Symbology — see list above |
| `PixelWidth` | `int` | `400` | Output bitmap width in pixels |
| `PixelHeight` | `int` | `150` | Output bitmap height in pixels |
| `MarginPixels` | `int` | `10` | Quiet-zone padding around the symbol |
| `ForegroundColor` | `Color` | `Black` | Bar / module color |
| `BarcodeBackgroundColor` | `Color` | `White` | Background fill |

### QRCodeView (MAUI)

Inherits everything from `BarcodeView`. Forces `Format = QRCode` and adds:

| Property | Type | Default | Description |
|---|---|---|---|
| `Size` | `int` | `300` | Square output edge length in px (sets both `PixelWidth` and `PixelHeight`) |
| `ErrorCorrection` | `QRErrorCorrection` | `Medium` | `Low` / `Medium` / `Quartile` / `High` — higher tolerates more damage at the cost of capacity |

### BarcodeView (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Value` | `string?` | `null` | Content to encode |
| `Format` | `BarcodeFormat` | `Code128` | Symbology |
| `ImageFormat` | `BarcodeImageFormat` | `Svg` | `Svg` (inline `<svg>`) or `Png` (`<img>` with `data:` URI) |
| `PixelWidth` / `PixelHeight` | `int` | `400` / `150` | Encoder pixel size and default CSS size when no `CssWidth`/`CssHeight` is set |
| `MarginPixels` | `int` | `10` | Quiet-zone padding |
| `ForegroundColor` / `BackgroundColor` | `string` | `"#000000"` / `"#FFFFFF"` | CSS hex colors |
| `CssWidth` / `CssHeight` | `string?` | `null` | CSS sizing overrides (`"100%"`, `"4cm"`, etc.) |
| `AltText` | `string?` | `null` | `alt` attribute when rendered as PNG `<img>` |
| `QRErrorCorrection` | `QRErrorCorrection` | `Medium` | Only honored when `Format = QRCode` |

### QRCodeView (Blazor)

Inherits everything from `BarcodeView`. Forces `Format = QRCode` and adds:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Size` | `int` | `300` | Square edge length — overwrites `PixelWidth`/`PixelHeight` |

## BarcodeRenderOptions (shared)

| Property | Type | Default | Description |
|---|---|---|---|
| `PixelWidth` / `PixelHeight` | `int` | `250` / `250` | Output bitmap size |
| `Margin` | `int` | `10` | Quiet zone in pixels |
| `ForegroundColor` | `string` | `"#000000"` | Hex (`#RGB`, `#RRGGBB`, or `#AARRGGBB` — alpha stripped) |
| `BackgroundColor` | `string` | `"#FFFFFF"` | Hex |
| `QRErrorCorrection` | `QRErrorCorrection` | `Medium` | QR-only, ignored for other formats |

## Code-Generation Rules

### MAUI

- XAML namespace is `xmlns:bc="http://shiny.net/maui/barcodes"` (already wired via `XmlnsDefinition`). Choose any prefix; `bc` is the assembly default.
- Use `QRCodeView` (not `BarcodeView` with `Format="QRCode"`) when you specifically want a QR code — it adds the `ErrorCorrection` property and a square `Size` shortcut. The base `BarcodeView` still works but exposes the rectangular `PixelWidth`/`PixelHeight` pair.
- `BarcodeView` is a `ContentView` wrapping a single `Image` set to `Aspect="AspectFit"`. Size the control with `WidthRequest`/`HeightRequest` or via layout; the encoded PNG renders at `PixelWidth × PixelHeight` regardless of the on-screen size.
- Setting `Value` to `null` or `""` clears the image. The control swallows encoder exceptions (e.g. EAN-13 with non-numeric content) and clears the image rather than throwing.

### Blazor

- Default `ImageFormat` is `Svg` — prefer it. Inline SVG scales infinitely without aliasing and stays tiny in DOM size (`shape-rendering="crispEdges"` plus a single horizontal-run `<path>`).
- Switch to `ImageFormat="BarcodeImageFormat.Png"` only when you need a raw `<img>` element (e.g. for `<img>`-specific styling, save-as menus, or copy-image-to-clipboard flows).
- Use `CssWidth`/`CssHeight` for responsive sizing while keeping `PixelWidth`/`PixelHeight` at the desired encoder resolution (the PNG/SVG is generated once per parameter change).
- The Blazor component re-encodes inside `OnParametersSet` whenever any parameter changes — keep `Value` stable across renders to avoid wasted encoding work.

### Both hosts

- `BarcodeRenderer.RenderPng/RenderSvg/RenderDataUri` are the static escape hatches. Use them for: PDF export (raw PNG bytes), `BlobUrl` creation in JS interop, server-side rendering, automated label printing, etc.
- `QRErrorCorrection.High` adds ~30% redundancy — use it for printed labels, stickers, or any QR that might be partially obscured. `Low` maximizes data capacity at the cost of damage tolerance.

## Symbology Quick Reference

| Format | Use For | Sample Payload |
|---|---|---|
| `QRCode` | URLs, vCards, Wi-Fi configs, generic text | `https://shinylib.net` |
| `Aztec` | Transport tickets, boarding passes | `M1JOHNDOE/EXAMPLE...` |
| `DataMatrix` | Tiny labels, electronics, healthcare | `SKU-12345` |
| `Pdf417` | Driver's licenses, shipping labels | larger structured data |
| `Code128` | General-purpose 1D barcodes, shipping, IDs | `ABC-12345-XYZ` |
| `Code39` | Legacy industry / military / automotive | `HELLO 42` (uppercase + a few symbols) |
| `Code93` | Compact alternative to Code 39 | `HELLO123` |
| `Codabar` | Libraries, blood banks, FedEx airbills | `A123456789B` |
| `Ean8` / `Ean13` | Retail products (Europe / global) | `5901234123457` (must be valid GTIN) |
| `UpcA` / `UpcE` | Retail products (North America) | `036000291452` |
| `Itf` | Carton labels, ITF-14 SCC-14 | even-length numeric string |

> 1D symbologies have strict input rules (length, alphabet, check digits). Invalid input does **not** throw — the view clears the image silently. Validate the payload before binding if you need explicit failure feedback.

## Common Pitfalls

- **EAN-13 / UPC-A with non-numeric content** — the encoder rejects it, the view goes blank. Validate digits before assigning to `Value`.
- **Blazor: bound CSS size but tiny encoder resolution** — set `PixelWidth`/`PixelHeight` to roughly the rendered size (or 2×) when using PNG output, otherwise the bitmap looks blocky. SVG output ignores resolution entirely.
- **MAUI: very small `WidthRequest` on a 1D code** — the bars may fall under the device pixel grid and read inconsistently. Encode at a high `PixelWidth` and let `AspectFit` shrink it; don't shrink the encoder.
- **MAUI: hot-binding `Value` from a stream of fast updates** — every change triggers a re-encode and `ImageSource.FromStream`. Throttle or debounce upstream if values change more than a few times per second.

## When to Use What

- **Show a URL / pairing code / Wi-Fi join** → `QRCodeView` with default `Medium` error correction.
- **Print a sticker that may get scuffed** → `QRCodeView` with `ErrorCorrection="High"`.
- **Render a retail product barcode** → `BarcodeView` with `Format="Ean13"` (or `UpcA` in North America). Make sure the payload is a valid GTIN.
- **Generate a PDF or email attachment with a barcode** → `BarcodeRenderer.RenderPng(...)` from your service layer; no view required.
- **Embed a barcode in a Blazor HTML email** → `BarcodeRenderer.RenderDataUri(..., BarcodeImageFormat.Png, ...)` and drop the result in an `<img src="...">`.
- **Need vector output for a print template** → `BarcodeRenderer.RenderSvg(...)` and embed in your XAML / HTML directly.
