# AnimatedBackground

A container that draws an animated effect **behind** its content. Both hosts have it, in the **core** packages (`Shiny.Maui.Controls` and `Shiny.Blazor.Controls`). Use it like a MAUI `Border` or an HTML `div`: content on top, effect behind, taps and clicks go straight through to the content. One `Effect` property picks the effect, and each effect is its own typed options object.

Effects: `AuroraEffect`, `ParticlesEffect`, `SnowEffect`, `RainEffect`, `MeteorsEffect`, `DotPatternEffect`, `NoiseEffect`, `StarfieldEffect`, `FirefliesEffect`, `BubblesEffect`, `WavesEffect`, `MatrixEffect`. All derive from `BackgroundEffect`, which has `Speed` (multiplier, default 1, 0 holds still).

## MAUI

```xml
xmlns:shiny="http://shiny.net/maui/controls"

<shiny:AnimatedBackground CornerRadius="16" Padding="24" Background="#0B1020">
    <shiny:AnimatedBackground.Effect>
        <shiny:SnowEffect Density="7" Wind="0.3" />
    </shiny:AnimatedBackground.Effect>
    <Label Text="Let it snow" TextColor="White" />
</shiny:AnimatedBackground>

<shiny:AnimatedBackground Background="#0F172A" HeightRequest="240">
    <shiny:AnimatedBackground.Effect>
        <shiny:DotPatternEffect Spotlight="Auto" SpotlightColor="#22D3EE" />
    </shiny:AnimatedBackground.Effect>
</shiny:AnimatedBackground>
```

- **Base type:** a `Layout` with a single `Content` (the content property), inset by `Padding`. It is **not** a `Border`, so don't use `StrokeShape`/`Stroke`. Use `CornerRadius` (clips background, effect and content).
- **Set `Background`:** most effects are made for a dark background. To fill a page, make it the page's content.
- **Colour lists** (`Colors` on Aurora, Particles, Fireflies, Bubbles, Waves) are `IReadOnlyList<Color>`; in XAML a comma-separated string: `Colors="#22D3EE,#6366F1"`.
- **`MeteorsEffect.MaxDelay`** is a `TimeSpan` (`"0:0:5"`).
- **Bindable:** effect properties are bindable and change live. One effect instance (a resource, a style) can be shared by several backgrounds.
- **Rendering:** a `GraphicsView` under the content, ticked by the window's shared frame ticker, only while loaded. Works on every head, AppKit and GTK4 included.
- **Pointer:** DotPattern spotlight and Particles repel/attract use hover (`PointerGestureRecognizer`: mouse, trackpad, pen) on the desktop heads only; iOS/Android track no pointer (on Android the recognizer stole taps from the content). On phones use `SpotlightMode.Auto` (wanders) or `Wander`, never `Pointer`.

## Blazor

```razor
<AnimatedBackground Effect="snow" Background="#0b1020" CornerRadius="16" Style="padding:24px">
    <h2>Let it snow</h2>
</AnimatedBackground>

<AnimatedBackground Effect="stars" Fixed="true" Background="#000">@Body</AnimatedBackground>

@code {
    readonly SnowEffect snow = new() { Density = 7, Wind = 0.3 };
    readonly StarfieldEffect stars = new() { Warp = true };
}
```

- **Effect** is an object instance, not a string. Keep it in a field; mutate it and re-render to change it live (settings are compared, not the reference).
- **Colours** are CSS strings; lists are `IReadOnlyList<string>`: `Colors = ["#22d3ee", "#6366f1"]`. `Background` is any CSS background (colour or gradient).
- **Styling:** padding, size, shadow through `Style`/`Class`. There is no `Padding` parameter.
- **`Fixed="true"`** (Blazor only): pins the effect to the viewport as a full-page background.
- **Rendering:** canvas + `requestAnimationFrame` in JS, no per-frame interop; pauses offscreen (IntersectionObserver) and in hidden tabs. Pointer is tracked on the window, so content on top still drives it.

## Host properties

| Property | Default | Notes |
|---|---|---|
| `Effect` | null | Null draws only the background |
| `Content` / `ChildContent` | null | On top of the effect |
| `Background` | none | MAUI `Brush`; Blazor CSS string |
| `CornerRadius` | 0 | Clips everything |
| `IsRunning` | true | False holds the effect where it is |
| `RespectReducedMotion` | true | Holds a still frame (never blank) under reduced motion |
| `Fixed` | false | Blazor only |

## Effect properties (defaults)

`Density` is always **per 100×100 area**, so it scales with the box.

| Effect | Properties |
|---|---|
| `AuroraEffect` | `Colors` (#22D3EE, #6366F1, #A855F7, #10B981), `Intensity` 0.6, `Size` 0.65 (fraction of longer side) |
| `ParticlesEffect` | `Colors` (#A5B4FC, #F0ABFC, #67E8F9), `Density` 2.5 (max 400), `MinSize` 1, `MaxSize` 2.5, `LinkDistance` 110 (0 = no lines), `LinkColor` (unset = first colour), `LinkOpacity` 0.35, `PointerMode` `Repel` (`ParticlePointerMode.None/Repel/Attract`), `PointerRadius` 120 |
| `SnowEffect` | `Color` white, `Density` 5, `MinSize` 1, `MaxSize` 3.5, `Wind` 0 (-1..1), `Sway` 1 |
| `RainEffect` | `Color` #A5B4FC, `Density` 8, `Angle` 12 (from vertical), `Length` 18, `Thickness` 1, `Splash` true |
| `MeteorsEffect` | `Color` white, `Density` 0.25, `Angle` 135 (clockwise from right), `Length` 90, `Thickness` 1.5, `MaxDelay` 5s |
| `DotPatternEffect` | `Color` #94A3B8, `Spacing` 18, `DotSize` 1.2, `DotOpacity` 0.35, `Spotlight` `Auto` (`SpotlightMode.None/Pointer/Wander/Auto`), `SpotlightColor` (unset = `Color`), `SpotlightRadius` 140, `Glow` 0.12, `Pulse` false |
| `NoiseEffect` | `Color` white, `Intensity` 0.15, `GrainSize` 2, `Coverage` 0.3, `FrameRate` 12 |
| `StarfieldEffect` | `Color` white, `Density` 6, `MaxSize` 1.6, `Warp` false, `Twinkle` true |
| `FirefliesEffect` | `Colors` (#FDE68A, #FACC15, #BEF264), `Density` 0.5, `Size` 2.5, `Glow` 5 (multiple of Size) |
| `BubblesEffect` | `Colors` (#7DD3FC, #A5B4FC, #F0ABFC), `Density` 0.6, `MinSize` 4, `MaxSize` 18 |
| `WavesEffect` | `Colors` (#38BDF8, #3B82F6, #6366F1; back to front), `Opacity` 0.45, `Amplitude` 18, `Wavelength` 320, `Level` 0.65 |
| `MatrixEffect` | `Color` #22C55E, `HeadColor` #DCFCE7, `FontSize` 14, `Characters` (katakana + digits), `TrailLength` 16 |
