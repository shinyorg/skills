# ShinyImage

A remote image that always shows *something*: placeholder artwork, a loading ring, the image itself, or error artwork. On MAUI it loads through an `IImageService` that caches to memory and disk, caps how many downloads run at once, and collapses concurrent requests for the same URI into one. On Blazor it streams through `fetch` so the ring can report a real download percentage, falling back to a plain `<img>` when CORS blocks that.

Use it anywhere a URL is bound to an image — avatars, product photos, feed images, thumbnail grids. For a full-screen zoomable viewer use `ImageViewer` instead; for editing use `ImageEditor`.

## The loading ring picks its own mode

There is no `IsIndeterminate` to set. `ImageLoadProgress.Percent` is `null` in exactly the cases where a percentage would be a lie:

- the request is **queued**, waiting for a download slot — nothing has been measured yet
- the response carried **no `Content-Length`** (chunked transfer, or a proxy that stripped it)
- the browser is loading the image itself (Blazor CORS fallback)

Null → the ring spins. Non-null → the ring fills and shows the percentage. Generate code that binds `Uri` and lets the control decide; do **not** try to drive the mode manually.

## MAUI

```xml
<shiny:ShinyImage Uri="{Binding AvatarUrl}"
                  PlaceholderImage="avatar_placeholder.png"
                  ErrorImage="broken_image.png"
                  Aspect="AspectFill"
                  HeightRequest="120" />
```

`PlaceholderImage` sits **behind** the ring rather than being swapped out for it, so a blurred thumbnail with a progress ring over it is two properties.

### Properties (MAUI)

| Property | Type | Default | Description |
|---|---|---|---|
| Uri | string? | null | The image to load. `http`/`https` goes through `IImageService`; anything else is treated as a local file or bundled resource and loaded directly |
| Source | ImageSource? | null | An explicit source. Takes precedence over `Uri` and skips the service entirely — use it for embedded resources, font images or a stream you already hold |
| PlaceholderImage | ImageSource? | null | Artwork shown before and during the load, behind the ring |
| ErrorImage | ImageSource? | null | Artwork shown when the load fails. Ignored when `ErrorTemplate` is set |
| LoadingTemplate | DataTemplate? | null | Replaces the ring entirely. `BindingContext` is the live `ImageLoadProgress` |
| ErrorTemplate | DataTemplate? | null | Replaces the error artwork |
| Aspect | Aspect | AspectFit | How the image scales. Applies to the placeholder too |
| FadeInDuration | uint | 150 | Fade-in milliseconds once loaded. `0` shows it instantly |
| RingSize | double | 48 | Diameter of the loading ring |
| RingColor | Color? | null | Progress arc colour; null uses the theme `Primary` token |
| RingTrackColor | Color? | null | Unfilled track; null uses `SurfaceContainerHighest` |
| ProgressTextColor | Color? | null | Percentage label; null uses `OnSurface` |
| ShowProgressText | bool | true | Draw the percentage inside the ring. Never shown when indeterminate |
| CacheEnabled | bool | true | Whether this image participates in the memory and disk caches |
| CacheDuration | TimeSpan? | null | Overrides `ImageOptions.DiskCacheDuration` for this image |
| State | ImageLoadState | None | Read-only: `None`, `Queued`, `Downloading`, `Loaded`, `Failed` |
| Progress | ImageLoadProgress | — | Read-only live snapshot; also the `LoadingTemplate` binding context |
| IsLoading | bool | false | Read-only: true while `Queued` or `Downloading` |
| LoadError | Exception? | null | Read-only: why the last load failed |
| ImageLoadedCommand | ICommand? | null | Invoked with `ImageLoadedEventArgs` once on screen |
| ImageFailedCommand | ICommand? | null | Invoked with the exception when a load fails |

Events: `ImageLoaded` (`ImageLoadedEventArgs(Uri, Origin, ContentLength)`) and `ImageFailed` (`ImageFailedEventArgs(Uri, Error)`). Method: `ReloadAsync()` re-fetches, skipping both cache tiers.

**These are named `ImageLoaded`/`ImageFailed`, not `Loaded`/`Failed`** — `VisualElement` already declares a `Loaded` event, and shadowing it would silently break anything relying on the base member. Never generate `Loaded="..."` expecting the image callback.

### Custom loading template

```xml
<shiny:ShinyImage Uri="{Binding PhotoUrl}" Aspect="AspectFill" HeightRequest="180">
    <shiny:ShinyImage.LoadingTemplate>
        <DataTemplate x:DataType="shiny:ImageLoadProgress">
            <VerticalStackLayout Spacing="6" HorizontalOptions="Center" VerticalOptions="Center">
                <Label Text="{Binding State}" FontAttributes="Bold" />
                <shiny:ProgressBar Value="{Binding PercentDisplay}"
                                   IsIndeterminate="{Binding IsIndeterminate}"
                                   WidthRequest="160" />
                <Label Text="{Binding BytesRead, StringFormat='{0:N0} bytes'}" FontSize="11" />
            </VerticalStackLayout>
        </DataTemplate>
    </shiny:ShinyImage.LoadingTemplate>
</shiny:ShinyImage>
```

`ImageLoadProgress` exposes `State`, `BytesRead`, `TotalBytes`, `Percent` (0-1 or null), `PercentDisplay` (0-100, or 0 when indeterminate) and `IsIndeterminate`. Setting a template re-points the host's `BindingContext` to the progress record, so bindings inside it resolve against progress, not the page view model.

### ImageService

```csharp
builder.UseShinyControls(cfg => cfg
    .ConfigureImages(o =>
    {
        o.MaxConcurrentDownloads = 4;                 // past this, requests report Queued
        o.DiskCacheDuration      = TimeSpan.FromDays(7);
        o.CacheDirectory         = null;              // null => <platform cache>/shinyimage
        o.MaxDiskCacheBytes      = 100 * 1024 * 1024; // LRU-trimmed to 80% when exceeded
        o.MemoryCacheEnabled     = true;
        o.MaxMemoryCacheBytes    = 32 * 1024 * 1024;
        o.MaxMemoryItemBytes     = 2 * 1024 * 1024;   // larger images stay disk-only
        o.Timeout                = TimeSpan.FromSeconds(60);
    })
);
```

`IImageService` (inject it for cache management):

| Member | Description |
|---|---|
| `GetAsync(ImageRequest, IProgress<ImageLoadProgress>?, CancellationToken)` | Memory → disk → network. Returns `ImageResult` — failures come back as `Success == false` with the exception attached, never as a throw |
| `ClearCacheAsync(ct)` | Deletes every cached image, disk and memory |
| `ClearCacheAsync(uri, ct)` | Deletes one entry |
| `GetCacheSizeAsync(ct)` | Total bytes on disk |
| `PrefetchAsync(uris, ct)` | Warms the cache for the next page of a list. Sequential, so speculative work cannot starve visible images |

`ImageResult` carries `Success`, `Bytes` (when small enough to hold), `FilePath`, `ContentLength`, `Origin` (`Memory`/`Disk`/`Network`) and `Error`. `Origin` is the way to verify caching actually works in a demo or a test.

### Custom downloader — the auth hook

For authenticated images, replace `IImageDownloader`, **not** the whole service. Caching, queueing and de-duplication stay where they are.

```csharp
class AuthenticatedDownloader(HttpClient client, ITokenStore tokens) : IImageDownloader
{
    public async Task<ImageDownloadResult> DownloadAsync(ImageRequest request, CancellationToken ct)
    {
        var msg = new HttpRequestMessage(HttpMethod.Get, request.Uri);
        msg.Headers.Authorization = new("Bearer", await tokens.GetAsync(ct));

        var response = await client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        return new ImageDownloadResult(
            await response.Content.ReadAsStreamAsync(ct),
            response.Content.Headers.ContentLength,   // this is what makes the ring determinate
            response.Content.Headers.ContentType?.MediaType
        );
    }
}

// builder.UseShinyControls(cfg => cfg.SetCustomImageDownloader<AuthenticatedDownloader>());
```

Return the body stream **unread** — the service pumps it and reports progress. Use `HttpCompletionOption.ResponseHeadersRead` so `ContentLength` is known before the body arrives. `cfg.SetCustomImageService<T>()` replaces the whole pipeline, caching included; prefer the downloader hook unless you genuinely need your own cache.

## Blazor

```razor
<ShinyImage Uri="@PhotoUrl"
            PlaceholderUri="/images/placeholder.svg"
            Alt="Profile photo"
            ObjectFit="cover"
            ImageLoaded="@(uri => status = "loaded")"
            ImageFailed="@(msg => status = msg)" />
```

### Parameters (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| Uri | string? | null | The image to load |
| PlaceholderUri | string? | null | Artwork shown before and during the load, behind the ring |
| ErrorUri | string? | null | Artwork on failure. A built-in glyph is used when neither this nor `ErrorContent` is set |
| Alt | string? | null | Alt text |
| ObjectFit | string | "contain" | CSS `object-fit` |
| FadeInDuration | int | 150 | Fade-in milliseconds |
| LoadingContent | RenderFragment&lt;ImageLoadProgress&gt;? | null | Replaces the ring; `context` is the live progress |
| ErrorContent | RenderFragment&lt;ImageLoadProgress&gt;? | null | Replaces the error artwork |
| RingSize | double | 48 | Ring diameter in px |
| ShowProgressText | bool | true | Percentage inside the ring |
| RingColor / RingTrackColor / ProgressTextColor | string | theme vars | CSS colours, defaulting to `var(--shiny-color-*)` |
| DisableProgress | bool | false | Skip the streamed fetch and let the browser load the URL directly |
| ErrorGlyph | string | "🖼" | Glyph used when no error artwork is supplied |
| CssClass | string? | null | Extra classes on the wrapper |
| ImageLoaded / ImageFailed | EventCallback&lt;string?&gt; / EventCallback&lt;string&gt; | — | Completion callbacks |

Method: `ReloadAsync()` re-fetches with a cache-busting parameter. Properties: `State`, `Progress`.

### What Blazor does and does not do

- **Progress, yes.** Remote images are streamed through `fetch` + `ReadableStream` so the ring shows a genuine percentage. No `<img>` element can report this — the DOM has no progress event for images.
- **Caching, no — on purpose.** The browser already has a well-tuned HTTP cache, shared across tabs and persisted between sessions. There is no `ClearCacheAsync` on Blazor because there is no cache of ours to clear.
- **CORS caveat.** A plain `<img>` may load a cross-origin image with no server cooperation; `fetch` may not. When the streamed load is blocked the component falls back to letting the browser load the URL directly — the image still appears, the ring just stays indeterminate. `DisableProgress="true"` takes that path deliberately.

For authenticated images:

```csharp
builder.Services.AddShinyImages();                          // routes through the registered HttpClient
builder.Services.AddShinyImages<AuthenticatedDownloader>(); // or your own IImageDownloader
```

The Blazor `IImageDownloader` returns `ImageDownloadResult(byte[] Bytes, string? ContentType)` and takes `(string uri, IProgress<ImageLoadProgress>?, CancellationToken)` — a different shape from the MAUI one, because the bytes have to end up in a blob URL rather than a file.

## Gotchas

- **`ImageLoaded`, not `Loaded`** (MAUI) — `VisualElement.Loaded` already exists.
- **Do not cache `ImageSource` objects yourself.** A stream-backed `ImageSource` is consumed once and platform image handles are bound to the view that realized them, so sharing one across `CollectionView` cells renders blank. The memory tier caches encoded **bytes**; each control builds a fresh `ImageSource.FromStream(() => new MemoryStream(bytes))`, which is cheap.
- A recycled cell raises `Unloaded` then `Loaded` again; the control cancels on the way out and restarts on the way back in. Cache hits make the retry nearly free — do not add your own reload logic for this.
- Local files and bundled resources never touch the service. Passing `"photo.png"` to `Uri` loads it directly; there is no need to use `Source` for that.
