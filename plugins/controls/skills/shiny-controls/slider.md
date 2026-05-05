# Slider

A slider control where the entire track displays a single solid color that is the interpolated blend between ColdColor and HotColor based on the current value position. At minimum, the track is fully ColdColor; at maximum, fully HotColor; at midpoints, the color is the proportional mix (e.g. blue→red produces purple/orange in between). A configurable tooltip floats above the thumb displaying the current value. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

### Basic Usage

```xml
<shiny:Slider Value="{Binding Temperature}"
                      Minimum="0"
                      Maximum="100"
                      ColdColor="#3B82F6"
                      HotColor="#EF4444"
                      ShowTooltip="True"
                      ValueFormat="0°" />
```

### Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| Value | double | 0 | TwoWay | Current slider value |
| Minimum | double | 0 | OneWay | Minimum value |
| Maximum | double | 100 | OneWay | Maximum value |
| Step | double | 1 | OneWay | Snap increment |
| ColdColor | Color | #3B82F6 | OneWay | Left (cold) gradient color |
| HotColor | Color | #EF4444 | OneWay | Right (hot) gradient color |
| TrackHeight | double | 8 | OneWay | Height of the track |
| ThumbSize | double | 24 | OneWay | Thumb diameter |
| ThumbColor | Color | White | OneWay | Thumb fill color |
| ThumbBorderWidth | double | 2 | OneWay | Thumb border width (colored by blended gradient) |
| ShowTooltip | bool | true | OneWay | Show value tooltip above thumb |
| TooltipBackgroundColor | Color | #1F2937 | OneWay | Tooltip badge background |
| TooltipTextColor | Color | White | OneWay | Tooltip text color |
| TooltipFontSize | double | 12 | OneWay | Tooltip font size |
| ValueFormat | string? | null | OneWay | .NET format string for display value |
| TooltipTemplate | DataTemplate? | null | OneWay | Custom tooltip content template (BindingContext = Value) |
| ValueChangedCommand | ICommand? | null | OneWay | Command fired on value change |

### Events

| Event | Args | Description |
|---|---|---|
| ValueChangedEvent | double | Fired when value changes |

### Behavior

- Track displays a full `LinearGradientBrush` from `ColdColor` to `HotColor`
- Fill portion shows gradient blended up to the current position
- Thumb border color is the blended color at the current position
- Supports both pan gesture (drag) and tap gesture (jump to position)
- Values snap to `Step` increments

### Custom Tooltip Template

```xml
<shiny:Slider Value="{Binding Temp}" Minimum="0" Maximum="100">
    <shiny:Slider.TooltipTemplate>
        <DataTemplate>
            <Border BackgroundColor="#7C3AED" StrokeShape="{RoundRectangle CornerRadius=8}" Padding="8,4">
                <Label Text="{Binding StringFormat='{0:0}°F'}" TextColor="White" FontSize="14" />
            </Border>
        </DataTemplate>
    </shiny:Slider.TooltipTemplate>
</shiny:Slider>
```

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

### Basic Usage

```razor
<Slider @bind-Value="temperature"
                Minimum="0"
                Maximum="100"
                ColdColor="#3B82F6"
                HotColor="#EF4444"
                ValueFormat="0°" />

@code {
    double temperature = 50;
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| Value | double | 0 | Current value (two-way via ValueChanged) |
| Minimum | double | 0 | Minimum value |
| Maximum | double | 100 | Maximum value |
| Step | double | 1 | Snap increment |
| ColdColor | string | #3B82F6 | Left gradient CSS color |
| HotColor | string | #EF4444 | Right gradient CSS color |
| TrackHeight | double | 8 | Track height (px) |
| ThumbSize | double | 24 | Thumb diameter (px) |
| ThumbColor | string | #FFFFFF | Thumb fill CSS color |
| ThumbBorderWidth | double | 2 | Thumb border width |
| CornerRadius | string | 4px | Track corner radius |
| ShowTooltip | bool | true | Show tooltip above thumb |
| TooltipBackgroundColor | string | #1F2937 | Tooltip background CSS color |
| TooltipTextColor | string | #FFFFFF | Tooltip text CSS color |
| TooltipFontSize | double | 12 | Tooltip font size (px) |
| ValueFormat | string? | null | Format string for value display |
| TooltipTemplate | RenderFragment\<double\>? | null | Custom tooltip render fragment |
| IsEnabled | bool | true | Enable/disable interaction |
| CssClass | string? | null | Additional CSS class |

### Custom Tooltip

```razor
<Slider @bind-Value="temp" Minimum="0" Maximum="100">
    <TooltipTemplate Context="val">
        <div style="background: #7C3AED; color: white; padding: 4px 12px; border-radius: 8px;">
            @val.ToString("0")°F
        </div>
    </TooltipTemplate>
</Slider>
```

### Code Generation Guidance

- Default tooltip is a dark badge with downward-pointing arrow/pointer
- Gradient blends from cold (blue) to hot (red) by default but colors are fully configurable
- Thumb border color always reflects the blended color at current position
- Blazor uses JS interop for pointer drag handling
- MAUI uses PanGestureRecognizer and TapGestureRecognizer
