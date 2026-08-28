# Slider

A slider control where the entire track displays a single solid color that is the interpolated blend between ColdColor and HotColor based on the current value position. At minimum, the track is fully ColdColor; at maximum, fully HotColor; at midpoints, the color is the proportional mix (e.g. blue→red produces purple/orange in between). A configurable tooltip floats above the thumb displaying the current value. The slider can run **vertically** (minimum at the bottom) and can carry labelled **stop points** — dots, ticks or pill-shaped bubbles the thumb snaps to. Available on both MAUI and Blazor.

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
| Step | double | 1 | OneWay | Snap increment (ignored while a mark is snapped to) |
| Orientation | SliderOrientation | Horizontal | OneWay | `Horizontal` or `Vertical` (minimum at the bottom) |
| VerticalLength | double | 220 | OneWay | Track length when vertical |
| ColdColor | Color | #3B82F6 | OneWay | Left/bottom (cold) gradient color |
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
| Marks | IList\<SliderMark\> | empty | — | Stop points, as `<shiny:Slider.Marks>` children |
| SnapToMarks | bool | true | OneWay | Thumb comes to rest on the nearest mark |
| MarkShape | SliderMarkShape | Dot | OneWay | `Dot`, `Bubble` or `Line` |
| MarkSize | double | 10 | OneWay | Dot diameter / tick thickness |
| MarkColor | Color? | null | OneWay | Fill for marks that set no color (null = theme Surface) |
| MarkTextColor | Color? | null | OneWay | Label color (null = theme OnSurfaceVariant) |
| MarkFontSize | double | 11 | OneWay | Label size |
| ShowMarkLabels | bool | true | OneWay | Draw the mark labels at all |

### Events

| Event | Args | Description |
|---|---|---|
| ValueChangedEvent | double | Fired when value changes |

### Behavior

- The whole track is one solid blend of `ColdColor` and `HotColor` at the current position
- Thumb border color is the blended color at the current position
- Supports both pan gesture (drag) and tap gesture (jump to position)
- Values snap to `Step` increments, unless marks are present and `SnapToMarks` is on
- Vertical runs bottom-to-top: the tooltip sits to the left of the track, mark labels to its right

### Stop Points

```xml
<shiny:Slider Value="{Binding Quality}" Minimum="0" Maximum="3" ShowTooltip="False">
    <shiny:Slider.Marks>
        <shiny:SliderMark Value="0" Text="Draft"  Color="#94A3B8" />
        <shiny:SliderMark Value="1" Text="Good"   Color="#38BDF8" />
        <shiny:SliderMark Value="2" Text="Better" Color="#22C55E" />
        <shiny:SliderMark Value="3" Text="Best"   Color="#F59E0B" />
    </shiny:Slider.Marks>
</shiny:Slider>
```

`SliderMark` properties: `Value`, `Text`, `Color`, `TextColor`, `Shape` (`SliderMarkShape?`, null
inherits `MarkShape`), `Size` (`-1` inherits `MarkSize`), `IsVisible`.

### Vertical

```xml
<shiny:Slider Orientation="Vertical" VerticalLength="180"
              Value="{Binding Bass}" Minimum="0" Maximum="10" Step="1" />
```

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
| Step | double | 1 | Snap increment (ignored while a mark is snapped to) |
| Orientation | SliderOrientation | Horizontal | `Horizontal` or `Vertical` (minimum at the bottom) |
| VerticalLength | double | 220 | Track length in px when vertical |
| ColdColor | string | #3B82F6 | Left/bottom gradient CSS color |
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
| ChildContent | RenderFragment? | null | The `<SliderMark>` stop points |
| SnapToMarks | bool | true | Thumb comes to rest on the nearest mark |
| MarkShape | SliderMarkShape | Dot | `Dot`, `Bubble` or `Line` |
| MarkSize | double | 10 | Dot diameter / tick thickness (px) |
| MarkColor | string | var(--shiny-color-surface) | Fill for marks that set no color |
| MarkTextColor | string | var(--shiny-color-on-surface-variant) | Label color |
| MarkFontSize | double | 11 | Label size (px) |
| ShowMarkLabels | bool | true | Draw the mark labels at all |

### Stop Points

```razor
<Slider @bind-Value="quality" Minimum="0" Maximum="3" ShowTooltip="false">
    <SliderMark Value="0" Text="Draft" Color="#94A3B8" />
    <SliderMark Value="1" Text="Good" Color="#38BDF8" />
    <SliderMark Value="2" Text="Better" Color="#22C55E" />
    <SliderMark Value="3" Text="Best" Color="#F59E0B" />
</Slider>
```

`SliderMark` parameters: `Value`, `Text`, `Color`, `TextColor`, `Shape` (`SliderMarkShape?`),
`Size` (`-1` inherits `MarkSize`), `IsVisible`.

### Vertical

```razor
<Slider @bind-Value="bass"
        Orientation="SliderOrientation.Vertical"
        VerticalLength="180"
        Minimum="0" Maximum="10" Step="1" />
```

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
- Marks are the stop points: a dot or tick on the track, with the label in the band beside it. Never
  draw a mark's label on the track — snapping parks the thumb on a mark by definition, so it would
  spend its life underneath the thumb
- `SnapToMarks` defaults to true, so a slider with marks ignores `Step`. Set it false for marks that
  are only reference points (a target line, a redline) on an otherwise continuous slider
- A vertical slider needs `VerticalLength`: it has no width to stretch into
