# MediaPickerButton

A button that lets the user add photos from the gallery and/or camera, compresses/re-encodes each to PNG or JPEG at a chosen quality (with optional max-dimension downscale), caps the count with `MaxPhotos` (added one at a time — the platform pickers are single-select), and shows the collected photos inline as a tappable carousel (opening the ImageViewer, with an optional Edit button that reuses the ImageEditor) or a compact pinch/zoom overlay.

- **MAUI**: `Shiny.Maui.Controls.Media.MediaPickerButton` — a plain `ContentView`, no handler/registration. Uses the built-in `Microsoft.Maui.Media.MediaPicker`. Available under the `shiny:` XAML namespace (`http://shiny.net/maui/controls`).
- **Blazor**: `Shiny.Blazor.Controls.MediaPickerButton` — self-imports its JS module; no DI registration. Gallery/camera via a hidden `<input type="file" accept="image/*">` (`capture="environment"` for camera), compressed on an offscreen canvas.

Both hosts ship inside the **base** packages (`Shiny.Maui.Controls` / `Shiny.Blazor.Controls`) — no extra package.

## MAUI Usage

```xml
<shiny:MediaPickerButton Photos="{Binding Photos}"
                         AllowGallery="True"
                         AllowCamera="True"
                         AllowPhotoEdit="True"
                         ShowAsCarouselInView="True"
                         MaxPhotos="5"
                         CompressionQuality="85"
                         OutputFormat="Jpeg"
                         PermissionDeniedText="Photo access was denied — enable it in Settings." />
```

```csharp
// ViewModel
[ObservableProperty]
ObservableCollection<MediaPickerItem> photos = new();   // Shiny.Maui.Controls.Media
```

`OutputFormat` is `Shiny.Maui.Controls.ImageEditor.ImageExportFormat` (`Png` / `Jpeg`; other values encode as JPEG).

## Blazor Usage

```razor
<MediaPickerButton @bind-Photos="photos"
                   AllowGallery="true"
                   AllowCamera="true"
                   AllowPhotoEdit="true"
                   ShowAsCarouselInView="true"
                   MaxPhotos="5"
                   CompressionQuality="85"
                   OutputFormat="jpeg"
                   PermissionDeniedText="Photo access was denied." />

@code {
    IReadOnlyList<Shiny.Blazor.Controls.MediaPickerItem> photos = [];
}
```

`OutputFormat` on Blazor is a string: `"jpeg"` or `"png"`.

## Properties (both hosts)

| Property | Type (MAUI / Blazor) | Default | Description |
|---|---|---|---|
| `AllowGallery` | `bool` | `true` | Offer "choose from gallery" |
| `AllowCamera` | `bool` | `true` | Offer "take photo". When both are true, tapping shows a gallery/camera chooser; when only one is true it is invoked directly. Blazor on iPhone/iPad skips the chooser: WebKit's own Photo Library / Take Photo / Choose File sheet already offers both, so don't turn `AllowCamera` off there to avoid a double prompt |
| `AllowPhotoEdit` | `bool` | `false` | Show an **Edit** button in the viewer that opens the ImageEditor; edits are re-saved into the collection |
| `PermissionDeniedText` | `string` | "Permission denied…" | Message shown when camera/gallery access is denied |
| `NoImagesTemplate` | `DataTemplate?` / `RenderFragment?` | `null` | Shown when there are no photos yet (a default "No photos yet" is used otherwise) |
| `ShowAsCarouselInView` | `bool` | `true` | `true` → inline carousel of thumbnails, tap to view/edit; `false` → compact preview that opens a paged pinch/zoom overlay |
| `MaxPhotos` | `int` | `1` | Maximum photos; the add trigger hides once reached |
| `CompressionQuality` | `int` | `92` | Encoder quality as a percentage (1–100) |
| `MaxImageDimension` | `int` | `0` | If > 0, the longest edge is downscaled to this many pixels |
| `OutputFormat` | `ImageExportFormat` / `string` | `Jpeg` / `"jpeg"` | Output encoding — PNG or JPEG |
| `Photos` | `IList<MediaPickerItem>` / `IReadOnlyList<MediaPickerItem>` | empty | The collected photos (two-way) |
| `AddButtonText` | `string` | "➕ Add Photo" | Text on the add trigger (Blazor: also the accessible name when `AddButtonTemplate` is set) |
| `AddButtonTemplate` | `DataTemplate?` / `RenderFragment?` | `null` | Replaces the dashed add trigger; still hides at `MaxPhotos`. Visuals only: the template is the tap target (MAUI — a `Button` inside swallows the tap) or sits inside a `<button>` (Blazor — no nested buttons/links) |
| `ItemSpacing` | `double` | `0` (MAUI) / `12` (Blazor) | Space between thumbnails in the inline carousel |
| `GalleryActionText` / `CameraActionText` | `string` | "Choose from Gallery" / "Take Photo" | Chooser labels |
| `UseFeedback` | `bool` (MAUI) | `true` | Haptic/sound feedback on actions |

## Events / Callbacks

- `PhotoAdded` / `PhotoRemoved` — a photo was added/removed (`MediaPickerItem`).
- `PhotosChanged` (MAUI event + `PhotosChangedCommand`) / `PhotosChanged` (Blazor `EventCallback`) — the collection changed.
- `PermissionDenied` — camera/gallery access was denied (message string).

## Uploading (Blazor only)

A picked photo's bytes stay in the browser. Only a descriptor crosses into .NET; the bytes move as binary — a JS stream, or a multipart POST the browser makes itself. **Never base64**: on Blazor Server that was one SignalR message per photo, over the default 32KB cap, and it closed the circuit.

```razor
<MediaPickerButton @ref="picker" @bind-Photos="photos" MaxPhotos="5" UploadProgress="OnProgress" />

@code {
    MediaPickerButton? picker;

    async Task Save()
    {
        var id = await SaveTheThing();      // save FIRST — the photos need something to belong to
        var results = await picker!.UploadAllAsync(new MediaPickerUpload($"/photos/{id}")
        {
            Headers = new Dictionary<string, string> { ["X-Upload-Ticket"] = ticket }
        });
    }

    void OnProgress(MediaPickerUploadProgress p) { percent = p.Percent ?? 0; StateHasChanged(); }
}
```

| Member | Purpose |
|---|---|
| `UploadUrl` (`MediaPickerUpload`) | Address, `FieldName` (default `file`), `FileName`, `Method`, `Headers`, `WithCredentials` |
| `AutoUpload` | Send each photo as it is picked (needs `UploadUrl`) |
| `UploadAllAsync(upload?, keep?)` / `UploadAsync(item, …)` | Send now, with an address only known after saving. `keep: true` sends the same photos again elsewhere |
| `Uploaded` / `UploadProgress` | Per-photo result (`Success`, `StatusCode`, `Body`) and progress (`Percent`) |
| `ClearAsync()` | Drop everything picked and free the browser's copies |
| `LoadBytes` / `MaxReadSize` | Whether to fill `Data` on pick (default true, false when uploading), and the read cap (32MB) |

Never hand an authorization token to the browser just to upload: post to your own origin with a short-lived ticket in `Headers`, and let the server attach the real credential.

## MediaPickerItem

The result DTO in each package. Bytes are already compressed/converted to `OutputFormat`.

On **Blazor** the bytes live in the browser: `Id`, `DataUri` (an object URL for `<img src>`), `Width`, `Height`, `ContentType`, `Size`, `FileName`, plus `Data` (filled on pick unless uploading), `HasData`, `OpenReadStreamAsync(max, ct)` and `ReadAllBytesAsync(max, ct)`.

- Both: `bool CanRemove` (default `true`) — `false` hides the thumbnail's ✕ so the user can't remove it (e.g. an already-saved photo). Kept through an edit. Code-side removal is not blocked.
- MAUI: `record MediaPickerItem(byte[] Data, int Width, int Height, string ContentType)` with `Stream OpenRead()` and an `ImageSource Thumbnail`; `CanRemove` is `init` → `new MediaPickerItem(bytes, w, h, "image/jpeg") { CanRemove = false }`.
- Blazor: `class MediaPickerItem { byte[] Data; string DataUri; int Width; int Height; string ContentType; }` — bind `DataUri` directly to `<img src>`.

## Pre-loaded / locked photos

- MAUI: add items to the bound `Photos` collection.
- Blazor: pass them in through `@bind-Photos` — a list the screen passes replaces what the picker holds. A saved photo can be given by URL only: `new MediaPickerItem { DataUri = "/photos/41.jpg", ContentType = "image/jpeg", CanRemove = false }`. Such an item has no browser bytes: `UploadAllAsync` skips it, Edit isn't offered unless `Data` is set, `ReadAllBytesAsync` throws.

## Notes

- **Multi-photo** is add-one-at-a-time: each tap picks/captures a single photo and appends until `MaxPhotos`. The OS pickers do not offer native multi-select here.
- **Permissions**: on MAUI the built-in `MediaPicker` drives the OS permission UI; a denial surfaces `PermissionDeniedText` (and the `PermissionDenied` event). On Blazor the browser handles file/camera access.
- **Reuse**: viewing uses the ImageViewer (pinch/zoom), editing uses the ImageEditor — the same controls documented in image-viewer.md and image-editor.md.

### Automation ids (MAUI)

Set `AutomationId` on the control and its parts are named after it, for UI tests (Appium, UI Automation, DevFlow). Without an id the parts get none.

| Part | Id |
|---|---|
| Add-photo tap target | `<id>_Add` |
| Permission-denied text | `<id>_Permission` |
| Carousel / each photo / its remove button | `<id>_Carousel` / `<id>_Photo_<index>` / `<id>_Remove_<index>` (default item template) |
| Compact preview | `<id>_Preview` |
| Viewer overlay / close | `<id>_Viewer_Popup` / `<id>_Viewer_Close` |
| Viewer previous / next / "2 / 5" label / edit | `<id>_Previous` / `<id>_Next` / `<id>_Position` / `<id>_Edit` |
| The photo editor and its tools | `<id>_Editor`, then the ImageEditor ids: `<id>_Editor_Save`, `<id>_Editor_Tool_Crop`, … |
| Editor cancel (✕) | `<id>_EditorCancel` |
