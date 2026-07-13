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
| `AllowCamera` | `bool` | `true` | Offer "take photo". When both are true, tapping shows a gallery/camera chooser; when only one is true it is invoked directly |
| `AllowPhotoEdit` | `bool` | `false` | Show an **Edit** button in the viewer that opens the ImageEditor; edits are re-saved into the collection |
| `PermissionDeniedText` | `string` | "Permission denied…" | Message shown when camera/gallery access is denied |
| `NoImagesTemplate` | `DataTemplate?` / `RenderFragment?` | `null` | Shown when there are no photos yet (a default "No photos yet" is used otherwise) |
| `ShowAsCarouselInView` | `bool` | `true` | `true` → inline carousel of thumbnails, tap to view/edit; `false` → compact preview that opens a paged pinch/zoom overlay |
| `MaxPhotos` | `int` | `1` | Maximum photos; the add trigger hides once reached |
| `CompressionQuality` | `int` | `92` | Encoder quality as a percentage (1–100) |
| `MaxImageDimension` | `int` | `0` | If > 0, the longest edge is downscaled to this many pixels |
| `OutputFormat` | `ImageExportFormat` / `string` | `Jpeg` / `"jpeg"` | Output encoding — PNG or JPEG |
| `Photos` | `IList<MediaPickerItem>` / `IReadOnlyList<MediaPickerItem>` | empty | The collected photos (two-way) |
| `AddButtonText` | `string` | "➕ Add Photo" | Text on the add trigger |
| `GalleryActionText` / `CameraActionText` | `string` | "Choose from Gallery" / "Take Photo" | Chooser labels |
| `UseFeedback` | `bool` (MAUI) | `true` | Haptic/sound feedback on actions |

## Events / Callbacks

- `PhotoAdded` / `PhotoRemoved` — a photo was added/removed (`MediaPickerItem`).
- `PhotosChanged` (MAUI event + `PhotosChangedCommand`) / `PhotosChanged` (Blazor `EventCallback`) — the collection changed.
- `PermissionDenied` — camera/gallery access was denied (message string).

## MediaPickerItem

The result DTO in each package. Bytes are already compressed/converted to `OutputFormat`.

- MAUI: `record MediaPickerItem(byte[] Data, int Width, int Height, string ContentType)` with `Stream OpenRead()` and an `ImageSource Thumbnail`.
- Blazor: `class MediaPickerItem { byte[] Data; string DataUri; int Width; int Height; string ContentType; }` — bind `DataUri` directly to `<img src>`.

## Notes

- **Multi-photo** is add-one-at-a-time: each tap picks/captures a single photo and appends until `MaxPhotos`. The OS pickers do not offer native multi-select here.
- **Permissions**: on MAUI the built-in `MediaPicker` drives the OS permission UI; a denial surfaces `PermissionDeniedText` (and the `PermissionDenied` event). On Blazor the browser handles file/camera access.
- **Reuse**: viewing uses the ImageViewer (pinch/zoom), editing uses the ImageEditor — the same controls documented in image-viewer.md and image-editor.md.
