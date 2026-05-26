# Overlay & LoadingOverlay

Full-screen overlay controls that integrate with `OverlayHost`/`ShinyContentPage`. When `IsShown` is true, the shared backdrop dims the page (with optional blur) and custom content is displayed centered. The base `Overlay` supports custom content via `DataTemplate`. The `LoadingOverlay` subclass provides a built-in loading template that can show an indeterminate spinner or a determinate progress bar (uses the `ProgressBar` control).

**Important:** On MAUI, overlays must be placed inside `ShinyContentPage.Panels` (or an `OverlayHost`). They use the same backdrop infrastructure as `FloatingPanel`.

## MAUI

### Overlay (Custom Content)

```xml
<shiny:ShinyContentPage xmlns:shiny="http://shiny.net/maui/controls" ...>

    <!-- Page content (set via ContentProperty) -->
    <ScrollView>
        <VerticalStackLayout Padding="16">
            <Button Text="Show Overlay" Command="{Binding ShowOverlayCommand}" />
        </VerticalStackLayout>
    </ScrollView>

    <!-- Overlays go in Panels -->
    <shiny:ShinyContentPage.Panels>
        <shiny:Overlay IsShown="{Binding IsOverlayVisible}"
                       BlurRadius="10"
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
    </shiny:ShinyContentPage.Panels>

</shiny:ShinyContentPage>
```

### LoadingOverlay (Indeterminate — Spinner)

```xml
<shiny:ShinyContentPage.Panels>
    <shiny:LoadingOverlay IsShown="{Binding IsBusy}"
                          Message="Loading..."
                          SpinnerColor="White" />
</shiny:ShinyContentPage.Panels>
```

### LoadingOverlay (Determinate — Progress Bar)

```xml
<shiny:ShinyContentPage.Panels>
    <shiny:LoadingOverlay IsShown="{Binding IsBusy}"
                          IsIndeterminate="False"
                          Progress="{Binding DownloadProgress}"
                          Message="Downloading..." />
</shiny:ShinyContentPage.Panels>
```

### Overlay Properties (MAUI)

| Property | Type | Default | Description |
|---|---|---|---|
| IsShown | bool | false | Show/hide the overlay (TwoWay binding) |
| AnimationDuration | uint | 250 | Fade in/out duration in ms |
| BlurRadius | double | 0 | When > 0, applies a frosted glass blur effect behind the backdrop (uses FrostedGlassView) |
| OverlayContentTemplate | DataTemplate | null | Custom overlay content |

Backdrop color and opacity are controlled by `ShinyContentPage.BackdropColor` and `ShinyContentPage.BackdropMaxOpacity` (or the equivalent properties on `OverlayHost`).

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
<Overlay IsShown="@isShown" OverlayColor="rgba(0, 0, 0, 0.6)" BlurRadius="10">
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
               BlurRadius="8"
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
| BlurRadius | double | 0 | When > 0, applies CSS `backdrop-filter: blur(Xpx)` to the backdrop |
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
| BlurRadius | double | 0 | When > 0, applies CSS `backdrop-filter: blur(Xpx)` to the backdrop |

## Placement

**MAUI:** Place `Overlay` or `LoadingOverlay` inside `ShinyContentPage.Panels` or an `OverlayHost`. They share the backdrop with `FloatingPanel`. Backdrop tap dismisses the overlay. The page must use `ShinyContentPage` as its base class (or manually add an `OverlayHost` to the visual tree).

**Blazor:** The `Overlay` component wraps your content via `ChildContent`. When `IsShown` is true, a fixed-position backdrop and content layer appear above everything.

## Notes

- The `LoadingOverlay` uses the `ProgressBar` control for its determinate mode
- MAUI uses `FadeToAsync` for smooth show/hide animation on the overlay view
- Blazor uses CSS transitions for fade effects
- Both support two-way binding on `IsShown` for MVVM/state management
- Backdrop tap dismisses the overlay on MAUI (same as FloatingPanel behavior)
- The blur effect uses `FrostedGlassView` on MAUI (native UIVisualEffectView on iOS, RenderEffect on Android 12+) and CSS `backdrop-filter` on Blazor
