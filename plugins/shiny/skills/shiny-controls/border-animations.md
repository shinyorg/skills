# Border Animations (BorderBeam, ShineBorder)

Animated card outlines in the style of Magic UI's border beam and shine border. Both hosts have them, in the **core** packages (`Shiny.Maui.Controls` and `Shiny.Blazor.Controls`). Each one is a rounded card that wraps content. The effect is drawn **over** the content and does not take input, and a static track sits under it.

## MAUI

```xml
<shiny:BorderBeam CornerRadius="16" BorderWidth="2" Padding="20" Background="{AppThemeBinding Light=White, Dark=#18181B}">
    <VerticalStackLayout>...</VerticalStackLayout>
</shiny:BorderBeam>

<shiny:ShineBorder ShineColors="#A07CFE,#FE8FB5,#FFBE7B" Duration="0:0:8" BorderWidth="2" Padding="20">
    <Label Text="Pro plan" />
</shiny:ShineBorder>
```

- **Base type:** both derive from `AnimatedBorder`, which is a `Layout` with a single `Content` (the content property). They are not `Border`s, so don't use `StrokeShape` or `Stroke`. Use `CornerRadius`, `BorderWidth` and `TrackColor`.
- **Clipping and inset:** the control clips its `Background` and content to `CornerRadius`. Content is inset by `Padding` plus `BorderWidth`.
- **Two beams:** a `BorderBeam` with no content is a plain overlay. Put a second one over the first in a `Grid` with `TrackColor="Transparent"`, `InputTransparent="True"`, and `Reverse="True"` or an `InitialOffset`. Do **not** nest them, because the inner one would be inset.
- **`ShineColors`:** an `IReadOnlyList<Color>`. In XAML it is a comma-separated string, which can be a single colour or `rgb(...)`.
- **`Duration` and `Delay`:** `TimeSpan` values. In XAML write them as `"0:0:6"`.
- **Rendering:** drawn on a `GraphicsView` that the shared frame ticker drives, on every head including AppKit and GTK4. It stops while unloaded, while `IsRunning` is false, or under OS reduced motion (`RespectReducedMotion`).

## Blazor

```razor
<BorderBeam CornerRadius="16" BorderWidth="2" Style="padding:20px">...</BorderBeam>
<ShineBorder ShineColors="@(["#A07CFE", "#FE8FB5", "#FFBE7B"])" Style="padding:20px">...</ShineBorder>
<BorderBeam Overlay="true" Reverse="true" TrackColor="transparent" />   @* inside any position:relative element *@
```

- **No script:** pure CSS, with a ring masked to the border. The beam rides `offset-path: rect()`, which needs Chrome 116, Safari 17.2 or Firefox 122 and up.
- **Styling:** set padding, background, size and shadow through `Style` or `Class`. Padding defaults to `BorderWidth`.
- **Colours:** any CSS colour strings, including theme tokens such as `var(--shiny-color-primary)`.
- **`Overlay="true"`:** renders only the ring, absolutely positioned over the nearest positioned ancestor. Use it to decorate an existing element or to stack a second effect.

## Properties

| Property | Default | Applies to |
|---|---|---|
| `CornerRadius` | 12 | both |
| `BorderWidth` | 1 | both |
| `TrackColor` | theme outline-variant (`Transparent` hides it) | both |
| `IsRunning` | true | both |
| `RespectReducedMotion` | true | both |
| `Overlay` | false | Blazor only |
| `Size` | 50 | BorderBeam |
| `Duration` | 6s | BorderBeam |
| `Delay` | 0 (a head start into the lap, not a wait) | BorderBeam |
| `ColorFrom` / `ColorTo` | `#FFAA40` / `#9C40FF` | BorderBeam |
| `Reverse` | false (anticlockwise when true) | BorderBeam |
| `InitialOffset` | 0 (% of the lap from the top-left) | BorderBeam |
| `ShineColors` | `#A07CFE, #FE8FB5, #FFBE7B` | ShineBorder |
| `Duration` | 14s | ShineBorder |
