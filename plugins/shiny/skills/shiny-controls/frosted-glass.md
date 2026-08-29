# FrostedGlassView

A view that applies a frosted glass (blur) effect behind its content. Uses native platform blur APIs for the best visual result.

## MAUI Usage

```xml
<shiny:FrostedGlassView BlurRadius="20"
                        TintColor="#80FFFFFF"
                        TintOpacity="0.6"
                        CornerRadius="16">
    <VerticalStackLayout Padding="20" Spacing="8">
        <Label Text="Frosted Glass Card" FontSize="20" FontAttributes="Bold" />
        <Label Text="Content on top of the glass effect." FontSize="14" />
    </VerticalStackLayout>
</shiny:FrostedGlassView>
```

## Blazor Usage

```razor
<FrostedGlass BlurRadius="20" TintColor="rgba(255,255,255,0.6)" CornerRadius="16">
    <h3>Frosted Glass Card</h3>
    <p>Content on top of the glass effect.</p>
</FrostedGlass>
```

## Properties

### MAUI

| Property | Type | Default | Description |
|---|---|---|---|
| `GlassContent` | `View` | - | Content rendered on top of the glass (ContentProperty) |
| `BlurRadius` | `double` | `20` | Blur strength in pixels |
| `TintColor` | `Color` | `#80FFFFFF` | Glass tint overlay color |
| `TintOpacity` | `double` | `0.6` | Tint overlay opacity |
| `CornerRadius` | `double` | `0` | Corner radius for clipping |

### Blazor

| Parameter | Type | Default | Description |
|---|---|---|---|
| `ChildContent` | `RenderFragment` | - | Content rendered on top of the glass |
| `BlurRadius` | `double` | `20` | Blur strength in pixels |
| `TintColor` | `string` | `rgba(255,255,255,0.6)` | CSS color for glass tint |
| `CornerRadius` | `double` | `0` | Border radius in pixels |
| `CssClass` | `string?` | `null` | Additional CSS classes on the root element |
| `Style` | `string?` | `null` | Additional inline CSS |

## Platform Behavior

| Platform | Implementation |
|---|---|
| iOS / macCatalyst | `UIVisualEffectView` using the system **ultra-thin** material (`SystemUltraThinMaterialLight` / `SystemUltraThinMaterialDark`, picked from the current theme). The material carries its own tint, so the control reduces its own tint overlay to avoid double-tinting |
| Android 12+ (API 31) | Background captured on each pre-draw pass and blurred with `RenderEffect` |
| Android 11 and older | No blur — `RenderEffect` is unavailable, so only the semi-transparent tint is drawn |
| Blazor | CSS `backdrop-filter: blur()` with `-webkit-` prefix |

## Tips

- Place behind content that overlays an image or busy background for best visual impact
- Use dark tint (`#80000000`) for glass over light backgrounds, light tint (`#80FFFFFF`) for glass over dark backgrounds
- Combine with `CornerRadius` for rounded glass cards
- The blur effect is applied to whatever is *behind* the view in the Z-order, not to the content inside it

## Code Generation Guidance

- Use `FrostedGlassView` (MAUI) or `FrostedGlass` (Blazor) when the user wants glass/blur/acrylic/frosted effects
- Always place over a visible background (image, gradient, or content) — blur over a solid color has no visible effect
- For navigation bars or toolbars, wrap the bar content in `FrostedGlassView` and overlay it on the page content

### Dark mode

Leave `TintColor` unset — it defaults to the themed surface at 60% and follows the scheme. A fixed
white veil reads as fog in dark mode. If you pin a light tint, pin the text colour of the content
inside it too.
