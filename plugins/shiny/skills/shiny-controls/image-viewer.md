# ImageViewer

A full-screen image overlay with pinch-to-zoom, pan (when zoomed), double-tap to toggle zoom, animated fade open/close, and a close button. Designed to overlay page content.

## Basic Usage

```xml
<Grid>
    <!-- Main page content -->
    <ScrollView>
        <VerticalStackLayout>
            <Image Source="photo.png">
                <Image.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding OpenViewerCommand}"
                                          CommandParameter="photo.png" />
                </Image.GestureRecognizers>
            </Image>
        </VerticalStackLayout>
    </ScrollView>

    <!-- ImageViewer overlays on top -->
    <shiny:ImageViewer Source="{Binding SelectedImage}"
                       IsOpen="{Binding IsViewerOpen}" />
</Grid>
```

## Remote images (MAUI)

The thumbnail and the full-screen overlay are both a `ShinyImage`, so binding `Uri` instead of `Source` brings the entire loading pipeline from [shiny-image.md](shiny-image.md) with it: placeholder artwork, a loading ring, error artwork, and `IImageService` memory + disk caching with a bounded download queue.

```xml
<shiny:ImageViewer Uri="{Binding PhotoUrl}"
                   PlaceholderImage="blur_thumb.png"
                   ErrorImage="broken_image.png"
                   Aspect="AspectFill"
                   RingSize="32"
                   HeightRequest="180" />
```

Generate `Uri` for anything coming off a server and `Source` only for streams, embedded resources and font images. `Source` wins when both are set.

Three things follow from there being two images, and generated code should not fight any of them:

- **The overlay is empty until it opens.** Populating it up front would decode a second full-size bitmap for every viewer in a list. It is filled in `OpenAsync`, from the same URI, so it comes back off the memory cache the thumbnail already warmed — do not add your own preload.
- **`ImageLoaded`/`ImageFailed` fire once**, from the thumbnail. The overlay loading the same URI on open is not a second event.
- **`State`, `Progress`, `IsLoading` and `LoadError` mirror the thumbnail** — the copy that is always in the visual tree, so they still say something useful while the viewer is closed.

This is MAUI-only. On Blazor `Source` is a URL string handed straight to `<img>`; there is no `Uri`, no ring and no `PlaceholderUri` on the viewer. Use `<ShinyImage>` separately there if you need progress.

## ImageViewer Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Uri` | `string?` | `null` | OneWay | Image to load. `http`/`https` goes through `IImageService`; anything else loads directly as a file or bundled resource (MAUI only) |
| `Source` | `ImageSource?` | `null` | OneWay | An explicit source. Takes precedence over `Uri` and skips the service |
| `IsOpen` | `bool` | `false` | TwoWay | Opens/closes the viewer with fade animation |
| `Aspect` | `Aspect` | `AspectFit` | OneWay | Aspect ratio mode for the thumbnail image (MAUI only) |
| `OverlayAspect` | `Aspect` | `AspectFit` | OneWay | Aspect ratio mode for the full-screen overlay image (MAUI only) |
| `OpenViewerOnTap` | `bool` | `true` | OneWay | When true, tapping the thumbnail opens the full-screen viewer; set to false to control opening via code only |
| `MaxZoom` | `double` | `5.0` | OneWay | Maximum pinch zoom scale |
| `CloseButtonTemplate` | `DataTemplate?` | `null` | OneWay | Custom close button template (tapping the templated view closes the viewer) |
| `HeaderTemplate` | `DataTemplate?` | `null` | OneWay | Custom header overlay at the top of the viewer |
| `FooterTemplate` | `DataTemplate?` | `null` | OneWay | Custom footer overlay at the bottom of the viewer |
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on double-tap zoom |
| `PlaceholderImage` | `ImageSource?` | `null` | OneWay | Artwork shown before and during the load, behind the ring (MAUI only) |
| `ErrorImage` | `ImageSource?` | `null` | OneWay | Artwork shown when the load fails. Ignored when `ErrorTemplate` is set (MAUI only) |
| `LoadingTemplate` | `DataTemplate?` | `null` | OneWay | Replaces the ring. Binding context is the live `ImageLoadProgress` (MAUI only) |
| `ErrorTemplate` | `DataTemplate?` | `null` | OneWay | Replaces the error artwork (MAUI only) |
| `FadeInDuration` | `uint` | `150` | OneWay | Milliseconds each image fades in over once loaded. `0` shows it instantly (MAUI only) |
| `RingSize` | `double` | `48` | OneWay | Diameter of the loading ring (MAUI only) |
| `RingColor` | `Color?` | `null` | OneWay | Progress arc colour; null uses the theme `Primary` token (MAUI only) |
| `RingTrackColor` | `Color?` | `null` | OneWay | Unfilled track; null uses `SurfaceContainerHighest` (MAUI only) |
| `ProgressTextColor` | `Color?` | `null` | OneWay | Percentage label; null uses `OnSurface` (MAUI only) |
| `ShowProgressText` | `bool` | `true` | OneWay | Draw the percentage inside the ring. Never shown when indeterminate (MAUI only) |
| `CacheEnabled` | `bool` | `true` | OneWay | Whether this image participates in the memory and disk caches (MAUI only) |
| `CacheDuration` | `TimeSpan?` | `null` | OneWay | Overrides `ImageOptions.DiskCacheDuration` for this image (MAUI only) |
| `State` | `ImageLoadState` | `None` | — | Read-only: `None`, `Queued`, `Downloading`, `Loaded`, `Failed` (MAUI only) |
| `Progress` | `ImageLoadProgress` | — | — | Read-only live snapshot (MAUI only) |
| `IsLoading` | `bool` | `false` | — | Read-only: true while `Queued` or `Downloading` (MAUI only) |
| `LoadError` | `Exception?` | `null` | — | Read-only: why the last load failed (MAUI only) |
| `ImageLoadedCommand` | `ICommand?` | `null` | OneWay | Invoked with `ImageLoadedEventArgs` once on screen (MAUI only) |
| `ImageFailedCommand` | `ICommand?` | `null` | OneWay | Invoked with the exception when a load fails (MAUI only) |

Events (MAUI): `ImageLoaded` and `ImageFailed`. Methods: `ReloadAsync()` re-fetches skipping both cache tiers; `ImageService` can be assigned to override the resolved service for one instance.

**These are named `ImageLoaded`/`ImageFailed`, not `Loaded`/`Failed`** — `VisualElement` already declares `Loaded`. Never generate `Loaded="..."` expecting the image callback.

## ImageViewer Features

- **Pinch-to-zoom**: Two-finger pinch gesture scales around the pinch origin, clamped between 1x and MaxZoom
- **Pan when zoomed**: One-finger pan is enabled after zooming in, with translation clamped to image bounds
- **Double-tap to zoom**: Double-tap zooms to 2.5x centered on the tap point; double-tap again resets to 1x
- **Animated open/close**: Backdrop, image, and close button fade in/out together (250ms)
- **Close button**: "✕" button in the top-right corner (customizable via `CloseButtonTemplate`)
- **Header/Footer templates**: Optional overlays at the top/bottom of the viewer for custom UI
- **Backdrop**: Black overlay that swallows touches so nothing falls through to the page behind
- **Touch passthrough**: When `Source` is null, `InputTransparent` is automatically set to true so the viewer does not block touches on content underneath
- **Tap-to-open control**: Set `OpenViewerOnTap="False"` to prevent the thumbnail from opening the viewer on tap — useful when you want to control opening via code only (e.g., from a button or command)

## ImageViewer Placement

ImageViewer must be placed inside a Grid that fills the page so it overlays correctly:

```xml
<ContentPage>
    <Grid>
        <!-- Main page content -->
        <ScrollView>
            <!-- ... -->
        </ScrollView>

        <!-- ImageViewer overlays on top -->
        <shiny:ImageViewer Source="{Binding SelectedImage}"
                           IsOpen="{Binding IsViewerOpen}" />
    </Grid>
</ContentPage>
```

## ImageViewer ViewModel Pattern

```csharp
public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty] ImageSource? selectedImage;
    [ObservableProperty] bool isViewerOpen;

    [RelayCommand]
    void OpenViewer(string imageSource)
    {
        SelectedImage = imageSource;
        IsViewerOpen = true;
    }
}
```
