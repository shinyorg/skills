# SkeletonView

A content-wrapping control that displays animated shimmer placeholders while data is loading — conceptually similar to MAUI's `RefreshView`. You wrap your real content, bind `IsBusy`, and while it is true the content is hidden and shimmer placeholders are shown in its place. When `IsBusy` becomes false, the placeholders disappear and the real content fades in.

The built-in placeholder is a stack of shimmer "lines" (good for text/paragraph content). For anything else, supply a custom placeholder layout (`SkeletonTemplate` on MAUI, `SkeletonContent` on Blazor) that matches the shape of the content being loaded.

## MAUI

`SkeletonView` derives from `Grid` and uses `Content` for the wrapped content (set it directly as the child element). The shimmer is a gradient highlight that sweeps left-to-right across the placeholder area, and it is always masked to the placeholder shapes — the gaps between them never shimmer. Built-in lines each clip their own copy of the sweep; a `SkeletonTemplate` is masked to the silhouettes of its shapes.

### Default placeholder (text lines)

```xml
<shiny:SkeletonView IsBusy="{Binding IsBusy}" ItemCount="4">
    <VerticalStackLayout Spacing="8">
        <Label Text="Loaded Article" FontSize="16" FontAttributes="Bold" />
        <Label Text="This real content appears once loading finishes." />
    </VerticalStackLayout>
</shiny:SkeletonView>
```

The child element is the `Content` (it is the `ContentProperty`), so you do not need to write `<shiny:SkeletonView.Content>` explicitly — though you must when you also set `SkeletonTemplate` (see below).

### Custom placeholder (match the content shape)

```xml
<shiny:SkeletonView IsBusy="{Binding IsBusy}"
                    BaseColor="{AppThemeBinding Light=#E1E1E6, Dark=#242B32}">
    <shiny:SkeletonView.SkeletonTemplate>
        <DataTemplate>
            <HorizontalStackLayout Spacing="12">
                <BoxView WidthRequest="56" HeightRequest="56" CornerRadius="28" Color="{AppThemeBinding Light=#E1E1E6, Dark=#242B32}" />
                <VerticalStackLayout Spacing="10" VerticalOptions="Center" WidthRequest="200">
                    <BoxView HeightRequest="14" CornerRadius="6" Color="{AppThemeBinding Light=#E1E1E6, Dark=#242B32}" HorizontalOptions="Fill" />
                    <BoxView HeightRequest="14" CornerRadius="6" Color="{AppThemeBinding Light=#E1E1E6, Dark=#242B32}" WidthRequest="120" HorizontalOptions="Start" />
                </VerticalStackLayout>
            </HorizontalStackLayout>
        </DataTemplate>
    </shiny:SkeletonView.SkeletonTemplate>
    <shiny:SkeletonView.Content>
        <HorizontalStackLayout Spacing="12">
            <BoxView WidthRequest="56" HeightRequest="56" CornerRadius="28" Color="#7C3AED" />
            <VerticalStackLayout Spacing="4" VerticalOptions="Center">
                <Label Text="Allan Ritchie" FontSize="16" FontAttributes="Bold" />
                <Label Text="Shiny Controls maintainer" TextColor="#6B7280" />
            </VerticalStackLayout>
        </HorizontalStackLayout>
    </shiny:SkeletonView.Content>
</shiny:SkeletonView>
```

The sweep is masked to the template's shapes: the control walks the arranged template, builds a rounded-rect silhouette per leaf shape (honoring `BoxView.CornerRadius` and `Border.StrokeShape`, so a `CornerRadius="28"` avatar masks as a circle) and clips the band to it. The empty box around the shapes never shimmers. Set `BaseColor` and give the template's shapes that same fill — the default sheen is derived from `BaseColor`, so hardcoding a light grey into the template while the theme (and therefore the sheen) is dark makes the sweep *darken* the shapes instead of highlighting them. Prefer an `AppThemeBinding` (or the `SurfaceContainerHigh` token) over a fixed hex.

### SkeletonView Properties (MAUI)

| Property | Type | Default | Description |
|---|---|---|---|
| Content | View? | null | The real content shown when `IsBusy` is false (the `ContentProperty`) |
| IsBusy | bool | false | When true, hides content and shows animated placeholders |
| SkeletonTemplate | DataTemplate? | null | Custom placeholder layout; when null, built-in lines are used |
| ItemCount | int | 3 | Number of built-in placeholder lines (last line is shortened) |
| BaseColor | Color? | null (theme `SurfaceContainerHigh`) | Fill color of built-in placeholder shapes |
| ShimmerColor | Color? | null (derived from `BaseColor`) | Color of the sweeping highlight |
| ShimmerEnabled | bool | true | When false, placeholders are static (no sweep) |
| AnimationDuration | uint | 1200 | Duration (ms) of a single shimmer sweep |
| CornerRadius | double | 6 | Corner radius of built-in placeholder lines |
| ItemHeight | double | 16 | Height of each built-in placeholder line |
| ItemSpacing | double | 12 | Vertical spacing between built-in placeholder lines |

## Blazor

The Blazor `SkeletonView` wraps content via `ChildContent` and renders placeholders when `IsBusy` is true. The shimmer is pure CSS (a moving gradient on `.shiny-skeleton__shape` elements), so it honors `prefers-reduced-motion`.

### Default placeholder (text lines)

```razor
<SkeletonView IsBusy="@isBusy" ItemCount="4">
    <ChildContent>
        <h3>Loaded Article</h3>
        <p>This real content appears once loading finishes.</p>
    </ChildContent>
</SkeletonView>
```

### Custom placeholder

Add the `shiny-skeleton__shape` class to any element in `SkeletonContent` to give it the shimmer. CSS variables for the colors/duration are inherited from the root, so your shapes shimmer automatically.

```razor
<SkeletonView IsBusy="@isBusy">
    <SkeletonContent>
        <div style="display:flex; gap:12px; align-items:center;">
            <div class="shiny-skeleton__shape" style="width:56px; height:56px; border-radius:50%;"></div>
            <div style="display:flex; flex-direction:column; gap:10px; flex:1;">
                <div class="shiny-skeleton__shape" style="height:14px; border-radius:6px; width:60%;"></div>
                <div class="shiny-skeleton__shape" style="height:14px; border-radius:6px; width:40%;"></div>
            </div>
        </div>
    </SkeletonContent>
    <ChildContent>
        <!-- real content -->
    </ChildContent>
</SkeletonView>
```

### SkeletonView Parameters (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| IsBusy | bool | false | When true, hides content and shows placeholders |
| ChildContent | RenderFragment? | — | The real content shown when `IsBusy` is false |
| SkeletonContent | RenderFragment? | — | Custom placeholder markup; when null, built-in lines are used |
| ItemCount | int | 3 | Number of built-in placeholder lines |
| ItemHeight | double | 16 | Height (px) of each built-in placeholder line |
| ItemSpacing | double | 12 | Vertical spacing (px) between built-in lines |
| CornerRadius | double | 6 | Corner radius (px) of built-in placeholder lines |
| BaseColor | string | #e1e1e6 | Base fill color of placeholder shapes |
| HighlightColor | string | rgba(255,255,255,0.6) | Color of the sweeping highlight |
| AnimationDuration | double | 1.4 | Duration (seconds) of a single shimmer sweep |
| ShimmerEnabled | bool | true | When false, placeholders are static (no sweep) |
| CssClass | string? | null | Additional CSS class on the root |

## Notes

- `SkeletonView` swaps between content and placeholders by visibility, so the control's height follows whichever is shown — size your placeholder to roughly match the loaded content to avoid layout jumps.
- On MAUI the shimmer is a translating `LinearGradientBrush` band (same technique as the `ProgressBar` pulse); on Blazor it is an animated `background-position` gradient. The MAUI band is a plain layout, not a `BoxView` — a `BoxView` with no `Color` of its own inherits the host app's implicit `Style TargetType="BoxView"`, which in the .NET MAUI template sets `BackgroundColor` to near-black and turned the shimmer into a black bar sweeping over the placeholders.
- Colors for a code-built `Brush` must be **resolved to concrete values**, never applied with `SetDynamicResource`. A `GradientStop` in a brush that is assigned straight to `Background` has no element to resolve a theme resource against, so the stop silently keeps its uncoloured default — which is what originally made the middle of this shimmer black.
- `ShimmerColor` defaults to the base fill lightened by ~10% luminosity, **not** to a theme surface token. The surface tokens are ordered by elevation, not luminance — `SurfaceContainerHighest` is *darker* than `SurfaceContainerHigh` in every light theme — so a sheen taken straight from a token sweeps a dark band across the placeholders.
- A `Clip` travels with the element it is set on, so the mask that keeps a sweeping band inside the placeholder shapes has to live on a **fixed** parent, not on the band itself.
- Gradient stops in a MAUI shimmer must fade to `highlight.WithAlpha(0)`, never to `Colors.Transparent`. iOS interpolates gradient stops per channel without premultiplying alpha, so fading out to `#00000000` drags the sweep through black and the band renders as a dark box crossing the control.
- For a full-screen "loading the whole page" experience use `LoadingOverlay` instead — `SkeletonView` is for inline content regions.
- Both hosts mirror the same API: `IsBusy`, custom placeholder slot, item count/height/spacing, base/highlight colors, animation duration, and a shimmer on/off toggle.
