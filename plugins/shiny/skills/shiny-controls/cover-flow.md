# CoverFlowView

An iTunes/iPod-style **Cover Flow** carousel. The centre item faces the viewer, and the side items tilt toward it and stack. Drag or flick to browse. Tapping a side item centres it, and tapping the centre item selects it. Both hosts have it, in the **core** packages. MAUI is pure cross-platform transforms with no handler. Blazor is a JS engine driving CSS 3D transforms.

## MAUI

```xml
<shiny:CoverFlowView ItemsSource="{Binding Albums}"
                     CurrentPosition="{Binding Position}"
                     ItemWidth="180" ItemHeight="180"
                     Loop="True" ShowReflection="True"
                     ItemSelectedCommand="{Binding OpenAlbumCommand}">
    <shiny:CoverFlowView.ItemTemplate>
        <DataTemplate x:DataType="local:Album">
            <Image Source="{Binding Artwork}" Aspect="AspectFill" />
        </DataTemplate>
    </shiny:CoverFlowView.ItemTemplate>
    <shiny:CoverFlowView.CaptionTemplate>
        <DataTemplate x:DataType="local:Album">
            <Label Text="{Binding Title}" HorizontalTextAlignment="Center" />
        </DataTemplate>
    </shiny:CoverFlowView.CaptionTemplate>
</shiny:CoverFlowView>
```

- Methods: `Next()`, `Previous()`, `GoTo(int index, bool animate = true)`.
- Events: `PositionChanged` (`EventHandler<int>`), `ItemSelected` (`EventHandler<CollectionItemEventArgs>`).
- Commands: `PositionChangedCommand`, `ItemSelectedCommand`.
- `DataTemplateSelector` is honoured for both templates.
- A tap with no position (accessibility activation, UI automation) selects the centre cover.

## Blazor

```razor
<CoverFlowView TItem="Album" Items="@albums" Loop="true" ShowReflection="true"
               @bind-CurrentPosition="position" ItemSelected="OpenAlbum">
    <ItemTemplate Context="a"><img src="@a.Artwork" alt="@a.Title" /></ItemTemplate>
    <CaptionTemplate Context="a">@a.Title</CaptionTemplate>
</CoverFlowView>
```

- Methods: `NextAsync()`, `PreviousAsync()`, `GoToAsync(int index, bool animate = true)`.
- Also: `EmptyTemplate`, `AriaLabel`, and `Perspective` (px, 800, Blazor-only).
- Keyboard: arrows, Home/End, and Enter/Space to select.

## Shared properties

| Property | Default | Notes |
|---|---|---|
| `ItemWidth` / `ItemHeight` | 200 / 200 | Cover size |
| `CurrentPosition` | 0 | Centre index, two-way |
| `SideAngle` | 60 | Degrees side covers turn toward the centre |
| `SideScale` | 0.85 | Side cover scale |
| `SideOpacity` | 1.0 | Opacity of the farthest visible cover |
| `CenterSpacing` | 130 | px from the centre to the first side cover. Also the drag distance for one item |
| `ItemSpacing` | 50 | px between stacked side covers |
| `VisibleItems` | 4 | Covers each side. Only these are created |
| `Loop` | false | Wrap around |
| `ShowReflection` / `ReflectionOpacity` | false / 0.3 | Mirrored, fading reflection |
| `AnimationDuration` | 300 | ms per step |
| `IsSwipeEnabled` | true | Drag on or off |

## Rules for generated code

- Keep item templates display-only (images, labels, borders). The control handles all taps, so a button inside a cover never fires.
- Give each cover a fixed size with `ItemWidth`/`ItemHeight`, not with sizes inside the template.
- On MAUI, `ShowReflection` fades into the theme's Surface colour, so put the control on a Surface background.
- Use `CarouselGallery` for a flat snap carousel, and `Carousel` (Blazor) for slides-per-view or autoplay. Use `CoverFlowView` for the 3D album-art look.

### Automation ids (MAUI)

Set `AutomationId` on the control and its parts are named after it, for UI tests (Appium, UI Automation, DevFlow). Without an id the parts get none.

| Part | Id |
|---|---|
| Each realized cover (index in `ItemsSource`) | `<id>_Item_<index>` |
| Caption area | `<id>_Caption` |

Only covers near the centre exist at any moment, and covers take no touches of their own: tap a cover's bounds and the control hit-tests it.
