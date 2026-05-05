# Overlay & LoadingOverlay

Full-screen overlay controls with configurable background color, transparency, and fade animation. The base `Overlay` supports custom content via `DataTemplate` (MAUI) or `RenderFragment` (Blazor). The `LoadingOverlay` subclass provides a built-in loading template that can show an indeterminate spinner or a determinate progress bar (uses the `ProgressBar` control).

## MAUI

### Overlay (Custom Content)

```xml
<shiny:Overlay IsShown="{Binding IsOverlayVisible}"
               OverlayColor="Black"
               OverlayOpacity="0.6"
               AnimationDuration="250">
    <shiny:Overlay.OverlayContentTemplate>
        <DataTemplate>
            <VerticalStackLayout HorizontalOptions="Center" VerticalOptions="Center" Spacing="12">
                <Label Text="Custom content" TextColor="White" FontSize="20" />
                <Button Text="Dismiss" Command="{Binding DismissCommand}" />
            </VerticalStackLayout>
        </DataTemplate>
    </shiny:Overlay.OverlayContentTemplate>
</shiny:Overlay>
```

### LoadingOverlay (Indeterminate — Spinner)

```xml
<shiny:LoadingOverlay IsShown="{Binding IsBusy}"
                      Message="Loading..."
                      SpinnerColor="White" />
```

### LoadingOverlay (Determinate — Progress Bar)

```xml
<shiny:LoadingOverlay IsShown="{Binding IsBusy}"
                      IsIndeterminate="False"
                      Progress="{Binding DownloadProgress}"
                      Message="Downloading..." />
```

### Overlay Properties (MAUI)

| Property | Type | Default | Description |
|---|---|---|---|
| IsShown | bool | false | Show/hide the overlay (TwoWay binding) |
| OverlayColor | Color | Black | Backdrop color |
| OverlayOpacity | double | 0.5 | Target backdrop opacity when shown |
| AnimationDuration | uint | 250 | Fade in/out duration in ms |
| OverlayContentTemplate | DataTemplate | null | Custom overlay content |

### LoadingOverlay Properties (MAUI)

Inherits all Overlay properties plus:

| Property | Type | Default | Description |
|---|---|---|---|
| IsIndeterminate | bool | true | true = spinner, false = progress bar |
| Progress | double | 0 | Progress 0–100 (TwoWay, used when determinate) |
| Message | string? | null | Text below spinner/progress bar |
| SpinnerColor | Color | White | ActivityIndicator color |

## Blazor

### Overlay (Custom Content)

The Blazor `Overlay` wraps child content and overlays when `IsShown` is true. Custom content goes in the `<OverlayContent>` render fragment.

```razor
<Overlay IsShown="@isShown" OverlayColor="rgba(0, 0, 0, 0.6)">
    <ChildContent>
        <p>Normal page content here</p>
    </ChildContent>
    <OverlayContent>
        <div style="color: white; text-align: center;">
            <h2>Custom overlay</h2>
            <button @onclick="() => isShown = false">Dismiss</button>
        </div>
    </OverlayContent>
</Overlay>
```

### LoadingOverlay

```razor
<LoadingOverlay IsShown="@isBusy"
               IsIndeterminate="false"
               Progress="@progress"
               Message="Downloading...">
    <p>Page content that gets overlaid</p>
</LoadingOverlay>
```

### Overlay Parameters (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| IsShown | bool | false | Show/hide overlay |
| IsShownChanged | EventCallback&lt;bool&gt; | — | Two-way binding callback |
| OverlayColor | string | "rgba(0,0,0,0.5)" | CSS color for backdrop |
| OverlayOpacity | double | 1.0 | Additional opacity multiplier |
| ChildContent | RenderFragment | — | Normal page content |
| OverlayContent | RenderFragment | — | Content shown in overlay |
| CssClass | string? | null | Additional CSS class |

### LoadingOverlay Parameters (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| IsIndeterminate | bool | true | Spinner (true) or progress bar (false) |
| Progress | double | 0 | Progress 0–100 |
| Message | string? | null | Text below loading indicator |
| SpinnerColor | string | "#FFFFFF" | CSS color for spinner border |
| SpinnerSize | double | 48 | Spinner diameter in px |
| ProgressBarColor | string | "#FFFFFF" | Progress bar fill color |
| ProgressTrackColor | string | "rgba(255,255,255,0.2)" | Progress bar track color |

## Placement

**MAUI:** Place the `Overlay` or `LoadingOverlay` inside a `Grid` alongside your page content. The overlay renders on top via z-order.

**Blazor:** The `Overlay` component wraps your content via `ChildContent`. When `IsShown` is true, a fixed-position backdrop and content layer appear above everything.

## Notes

- The `LoadingOverlay` uses the `ProgressBar` control for its determinate mode
- MAUI uses `FadeToAsync` for smooth show/hide animation
- Blazor uses CSS transitions for fade effects
- Both support two-way binding on `IsShown` for MVVM/state management
