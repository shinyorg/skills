# VideoText

Text whose letters are cut out of a looping video (Magic UI "Video Text"), under a coloured overlay (black by default) with optional marquee scrolling. Ships in the **MediaElement** add-on on both hosts — `Shiny.Maui.Controls.MediaElement` (`xmlns:media="http://shiny.net/maui/media"`, namespace `Shiny.Maui.Controls.Media`) and `Shiny.Blazor.Controls.MediaElement` (namespace `Shiny.Blazor.Controls.Media`).

Decorative only: **no transport controls, always autoplays and loops, muted by default**. Do not add play/pause buttons or wire `MediaElement` APIs to it — it exposes none.

## MAUI

Requires `.UseShinyMediaElement()` in `MauiProgram` (registers the handler).

```xml
<media:VideoText Source="https://example.com/ocean.mp4"
                 Text="OCEAN"
                 HeightRequest="200" />
```

Properties: `Source` (MediaSource, string-convertible), `Text` (use `&#10;` for line breaks — never wraps), `FontSize` (double, **0 = auto-fit**, the default), `FontFamily`, `FontAttributes` (default `Bold`), `OverlayColor` (Color, default `Black`; `{x:Null}` = no overlay), `OverlayOpacity` (0–1, default 1), `Marquee` (`MarqueeDirection`: `None`/`Left`/`Right`/`Up`/`Down`, from `Shiny.Controls.Media`), `MarqueeDuration` (TimeSpan, default 8s, one pass), `IsMuted` (default `true`), `FallbackTextColor`.

```xml
<media:VideoText Source="intro.mp4" Text="SHINY CONTROLS"
                 OverlayColor="#0F172A" OverlayOpacity="0.7"
                 Marquee="Left" MarqueeDuration="0:0:6" HeightRequest="120" />
```

- Always give it a size (`HeightRequest`, or a sized parent) — the video covers the control and the text fits into it.
- The overlay colours everything around the letters; opacity below 1 lets the video show faintly through it. For a see-through control over a gradient/image backdrop, set `OverlayColor="{x:Null}"` and put the backdrop in a parent.
- With a marquee, auto-fit sizes the text to the cross axis only (90% of the height for Left/Right).
- It pauses on `Unloaded` and resumes on `Loaded` by itself; don't add page `OnDisappearing` code to stop it.
- Masked on iOS, Mac Catalyst, macOS AppKit and Android. On Windows and Linux it shows plain text in `FallbackTextColor`; check `VideoText.IsMaskSupported` if a page must differ.

## Blazor

```razor
<VideoText Source="https://example.com/ocean.mp4" Text="OCEAN" Style="height:220px;" />
```

Parameters: `Source`, `Text` (`\n` for lines), `FontSize` (CSS px, 0 = auto-fit), `FontFamily` (default `sans-serif`), `FontWeight` (default `bold`), `FontStyle`, `OverlayColor` (CSS colour, default `"black"`; `null`/`"transparent"`/`"none"` = no overlay), `OverlayOpacity` (default 1), `Marquee` (`MarqueeDirection`, needs `@using Shiny.Controls.Media`), `MarqueeDuration` (TimeSpan, default 8s), `IsMuted` (default `true`), `Preload`, `CssClass`, `Style` (container defaults to 200px tall).

```razor
<VideoText Source="intro.mp4" Text="SHINY CONTROLS" OverlayColor="#0f172a" OverlayOpacity="0.7"
           Marquee="MarqueeDirection.Left" MarqueeDuration="TimeSpan.FromSeconds(6)" Style="height:120px;" />
```

- With an overlay (the default) `FontFamily` can be a web font. Without an overlay the mask is an SVG image and can only use **system or generic** fonts.
- Unmuted playback needs a user gesture; until then it plays muted rather than freezing.
