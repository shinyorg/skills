# RangeSlider

A two-thumb range slider that selects a lower/upper value pair. It reuses the visual language of [Slider](slider.md) — the cold-to-hot gradient, blended thumb borders, and floating value tooltip — but shows the gradient only across the **active segment between the two thumbs** (the rest of the track is a neutral surface color), draws a tooltip per thumb, and adds two gap constraints:

- **MinimumRange** — the smallest distance the thumbs may be apart. The dragged thumb **hard-stops** rather than crossing this gap; the other thumb stays put. `0` disables it.
- **MaximumRange** — the largest distance the thumbs may be apart. Dragging one thumb past this **pushes the other thumb** along so the gap never exceeds it. `0` disables it.

Constraints apply to interaction (drag/tap). Programmatically-set / data-bound values render as-is. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

### Basic Usage

```xml
<shiny:RangeSlider LowerValue="{Binding PriceLow}"
                   UpperValue="{Binding PriceHigh}"
                   Minimum="0"
                   Maximum="1000"
                   Step="10"
                   MinimumRange="50"
                   MaximumRange="500"
                   ValueFormat="C0" />
```

### Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| LowerValue | double | 0 | TwoWay | Lower thumb value |
| UpperValue | double | 100 | TwoWay | Upper thumb value |
| Minimum | double | 0 | OneWay | Minimum value |
| Maximum | double | 100 | OneWay | Maximum value |
| Step | double | 1 | OneWay | Snap increment |
| MinimumRange | double | 0 | OneWay | Minimum gap between thumbs (hard stop); 0 = off |
| MaximumRange | double | 0 | OneWay | Maximum gap between thumbs (pushes the other thumb); 0 = off |
| ColdColor | Color | #3B82F6 | OneWay | Left (cold) gradient color |
| HotColor | Color | #EF4444 | OneWay | Right (hot) gradient color |
| TrackHeight | double | 8 | OneWay | Height of the track |
| ThumbSize | double | 24 | OneWay | Thumb diameter |
| ThumbColor | Color | White | OneWay | Thumb fill color |
| ThumbBorderWidth | double | 2 | OneWay | Thumb border width (colored by blended gradient) |
| ShowTooltip | bool | true | OneWay | Show a value tooltip per thumb |
| TooltipBackgroundColor | Color | #1F2937 | OneWay | Tooltip badge background |
| TooltipTextColor | Color | White | OneWay | Tooltip text color |
| TooltipFontSize | double | 12 | OneWay | Tooltip font size |
| ValueFormat | string? | null | OneWay | .NET format string for display values |
| TooltipTemplate | DataTemplate? | null | OneWay | Custom tooltip content (BindingContext = the thumb's value); applied to both thumbs |
| RangeChangedCommand | ICommand? | null | OneWay | Command fired with a `SliderRange(Lower, Upper)` on change |

### Events

| Event | Args | Description |
|---|---|---|
| RangeChanged | SliderRange (record struct: Lower, Upper) | Fired when either value changes through interaction |

### Behavior

- The active segment between the two thumbs renders a `LinearGradientBrush` blended cold→hot at the thumb positions; the rest of the track is the theme SurfaceVariant color
- Each thumb's border color is the blended color at its position
- Each thumb has its own pan gesture; tapping the track moves whichever thumb is nearer
- Values snap to `Step`; drags respect `MinimumRange` (hard stop) and `MaximumRange` (pushes the other thumb)

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

### Basic Usage

```razor
<RangeSlider @bind-LowerValue="priceLow"
             @bind-UpperValue="priceHigh"
             Minimum="0"
             Maximum="1000"
             Step="10"
             MinimumRange="50"
             MaximumRange="500"
             ValueFormat="C0" />

@code {
    double priceLow = 250;
    double priceHigh = 750;
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| LowerValue | double | 0 | Lower thumb value (two-way via LowerValueChanged) |
| UpperValue | double | 100 | Upper thumb value (two-way via UpperValueChanged) |
| Minimum | double | 0 | Minimum value |
| Maximum | double | 100 | Maximum value |
| Step | double | 1 | Snap increment |
| MinimumRange | double | 0 | Minimum gap between thumbs (hard stop); 0 = off |
| MaximumRange | double | 0 | Maximum gap between thumbs (pushes the other thumb); 0 = off |
| ColdColor | string | #3B82F6 | Left gradient CSS color |
| HotColor | string | #EF4444 | Right gradient CSS color |
| TrackHeight | double | 8 | Track height (px) |
| ThumbSize | double | 24 | Thumb diameter (px) |
| ThumbColor | string | #FFFFFF | Thumb fill CSS color |
| ThumbBorderWidth | double | 2 | Thumb border width |
| CornerRadius | string | 4px | Track corner radius |
| ShowTooltip | bool | true | Show a tooltip per thumb |
| TooltipBackgroundColor | string | #1F2937 | Tooltip background CSS color |
| TooltipTextColor | string | #FFFFFF | Tooltip text CSS color |
| TooltipFontSize | double | 12 | Tooltip font size (px) |
| ValueFormat | string? | null | Format string for value display |
| TooltipTemplate | RenderFragment\<double\>? | null | Custom tooltip render fragment (applied to both thumbs) |
| IsEnabled | bool | true | Enable/disable interaction |
| CssClass | string? | null | Additional CSS class |

### Events

| Event | Type | Description |
|---|---|---|
| LowerValueChanged | EventCallback\<double\> | Two-way binding callback for the lower thumb |
| UpperValueChanged | EventCallback\<double\> | Two-way binding callback for the upper thumb |
| RangeChanged | EventCallback\<(double Lower, double Upper)\> | Fired with both values on change |

### Code Generation Guidance

- Use `@bind-LowerValue` / `@bind-UpperValue` for two-way binding
- Gradient fills only the segment between thumbs; the base track is `var(--shiny-color-surface-variant)`
- Blazor uses JS interop (`rangeslider.js`) for pointer drag; the dragged thumb is identified by a `data-thumb` attribute
- For a single-value slider, use [Slider](slider.md) instead
```
