# Fab & FabMenu

Material-style floating action button (`Fab`) and an expanding multi-action menu (`FabMenu`) that animates `FabMenuItem` children up from the main FAB with a staggered reveal.

`FabMenuItem` renders as a **pill**: the label lives inside one capsule with a tinted circular icon chip on the edge nearest the main FAB, so the whole row is a single tap target. Every chip is inset so its centre lands on the main FAB's vertical axis. An item with no `Text` collapses to a plain circle of `Size`.

## Basic Usage

```xml
<!-- Single icon-only Fab pinned to bottom-right -->
<shiny:Fab Icon="add.png"
           FabBackgroundColor="#E91E63"
           Command="{Binding AddCommand}"
           HorizontalOptions="End"
           VerticalOptions="End"
           Margin="24" />

<!-- Extended Fab (icon + text) -->
<shiny:Fab Icon="add.png"
           Text="Add Item"
           FabBackgroundColor="#4CAF50"
           TextColor="White"
           Command="{Binding AddCommand}" />

<!-- FabMenu (speed dial) -->
<shiny:FabMenu IsOpen="{Binding IsMenuOpen}"
               Icon="plus.png"
               FabBackgroundColor="#2196F3"
               HorizontalOptions="End"
               VerticalOptions="End"
               Margin="24">
    <shiny:FabMenuItem Icon="share.png"  Text="Share"  Command="{Binding ShareCommand}" />
    <shiny:FabMenuItem Icon="edit.png"   Text="Edit"   Command="{Binding EditCommand}" />
    <shiny:FabMenuItem Icon="delete.png" Text="Delete" Command="{Binding DeleteCommand}" />
</shiny:FabMenu>
```

## Placement (important)

Place the `Fab` / `FabMenu` inside a `Grid` that fills the page — same pattern as `ImageViewer`:

```xml
<ContentPage>
    <Grid>
        <ScrollView>
            <!-- page content -->
        </ScrollView>

        <shiny:FabMenu IsOpen="{Binding IsMenuOpen}"
                       Icon="plus.png"
                       HorizontalOptions="End"
                       VerticalOptions="End"
                       Margin="24">
            <!-- items -->
        </shiny:FabMenu>
    </Grid>
</ContentPage>
```

## Fab Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Icon` | `ImageSource?` | `null` | OneWay | Icon shown inside the button |
| `Text` | `string?` | `null` | OneWay | Optional label; when null the Fab is a perfect circle. A short label (e.g. `+`) still renders circular — the Fab only stretches into a pill once the label needs more than `Size` |
| `Command` | `ICommand?` | `null` | OneWay | Executed on tap |
| `CommandParameter` | `object?` | `null` | OneWay | Parameter forwarded to the Command |
| `FabBackgroundColor` | `Color` | `#2196F3` | OneWay | Fill color |
| `BorderColor` | `Color?` | `null` | OneWay | Outline stroke color |
| `BorderThickness` | `double` | `0` | OneWay | Outline stroke thickness |
| `TextColor` | `Color` | `White` | OneWay | Label text color |
| `FontSize` | `double` | `14` | OneWay | Label font size |
| `FontAttributes` | `FontAttributes` | `None` | OneWay | Label font attributes |
| `Size` | `double` | `56` | OneWay | Height (diameter when circular) |
| `IconSize` | `double` | `24` | OneWay | Icon image size |
| `HasShadow` | `bool` | `true` | OneWay | Drop shadow on/off |
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on tap |

Events: `Clicked`.

## FabMenu Properties

In addition to the main-Fab pass-throughs (`Icon`, `Text`, `FabBackgroundColor`, `BorderColor`, `BorderThickness`, `TextColor`):

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `IsOpen` | `bool` | `false` | TwoWay | Opens/closes the menu with animation |
| `Items` | `IList<FabMenuItem>` | empty | — | Content property; place items directly inside `<shiny:FabMenu>` |
| `FabSize` | `double` | `56` | OneWay | Main FAB button size (diameter) |
| `HasShadow` | `bool` | `true` | OneWay | Drop shadow on the main FAB |
| `MenuAlignment` | `LayoutOptions` | `End` | OneWay | Horizontal alignment of the menu stack (`Start` for left-aligned, `End` for right-aligned) |
| `HasBackdrop` | `bool` | `true` | OneWay | Show dim backdrop while open |
| `BackdropColor` | `Color` | `Black` | OneWay | Backdrop color |
| `BackdropOpacity` | `double` | `0.4` | OneWay | Backdrop peak opacity |
| `CloseOnBackdropTap` | `bool` | `true` | OneWay | Close when backdrop tapped |
| `CloseOnItemTap` | `bool` | `true` | OneWay | Close after item tap |
| `AnimationDuration` | `uint` | `200` | OneWay | Open/close animation duration (ms) |
| `IconRotation` | `double` | `45` | OneWay | Degrees the main FAB rotates while open ("+" → "×"). `0` disables; ignored when the main FAB has `Text` |
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on toggle |

Events: `ItemTapped(FabMenuItem)`.
Methods: `Open()`, `Close()`, `Toggle()`.

## FabMenuItem Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Icon` | `ImageSource?` | `null` | Icon rendered in the circular chip |
| `Text` | `string?` | `null` | Label inside the pill; when null the item collapses to a plain circle |
| `Command` | `ICommand?` | `null` | Invoked on tap |
| `CommandParameter` | `object?` | `null` | Parameter forwarded to the Command |
| `FabBackgroundColor` | `Color` | theme `Primary` | Icon chip fill — and the whole pill's fill when the item has no `Text` |
| `BorderColor` | `Color?` | theme `OutlineVariant` | Pill outline stroke |
| `BorderThickness` | `double` | `1` | Pill outline thickness (`0` for a borderless pill) |
| `TextColor` | `Color` | theme `OnSurface` | Label text color |
| `LabelBackgroundColor` | `Color` | theme `SurfaceContainerHigh` | Pill body fill behind the label |
| `FontSize` | `double` | `13` | Label font size |
| `FontAttributes` | `FontAttributes` | `None` | Label font attributes |
| `Size` | `double` | `44` | Pill height (diameter when the item has no `Text`) |
| `IconSize` | `double` | `20` | Icon image size |
| `HasShadow` | `bool` | `true` | Drop shadow on the pill |
| `UseFeedback` | `bool` | `true` | Feedback on tap |

Events: `Clicked`.

## Behavior & Animation

- Tapping the main Fab of a `FabMenu` toggles `IsOpen`
- Opening the menu fades in the backdrop and animates each `FabMenuItem` up (fade + translate + scale) with a small stagger, anchored on the main FAB's axis; closing reverses it
- The main FAB rotates `IconRotation` degrees (45 by default) while open — the classic "+" → "×". It is skipped when the main FAB has `Text`, since a rotated label reads as broken
- The whole pill is one tap target — label and icon chip both fire the item's `Command`
- `IsOpen` is fully two-way bindable — setting it from a ViewModel animates in/out
- Child items' own animations never conflict with the main Fab — it stays fixed
- Tapping a menu item executes its `Command`, raises `ItemTapped` on the menu, and closes the menu when `CloseOnItemTap` is true (default)

## Code Generation Guidance

- Use `Fab` for a single primary action (e.g., "Add") and `FabMenu` for multiple related actions (speed dial)
- Always place `Fab` / `FabMenu` inside a Grid that fills the page so the FabMenu backdrop overlays everything (or use `ShinyContentPage` with `OverlayHost`)
- Default to `HorizontalOptions="End"` + `VerticalOptions="End"` + `Margin="24"` for the canonical Material bottom-right placement
- Bind `IsOpen` TwoWay when the ViewModel needs to drive the menu state; otherwise omit it and let taps control it
- Keep `FabMenuItem` icons monochrome/filled for best visual contrast against the colored chips
- Leave `LabelBackgroundColor` / `TextColor` / `BorderColor` unset so the pill picks up the theme (surface-container-high body, on-surface label, outline-variant hairline); set only `FabBackgroundColor` per item to tint its chip
- On an item that has `Text` but no `Icon` there is no chip to tint, so `FabBackgroundColor` silently does nothing — give the item an `Icon` or drop the colour rather than leaving a setting that never renders
- Keep every item's `Size` the same so the pills read as one column
- Use `Icon` on every item when possible; `Text` is optional but strongly recommended for accessibility

## ViewModel Pattern

```csharp
public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty] bool isMenuOpen;

    [RelayCommand] void Add()    { /* ... */ }
    [RelayCommand] void Share()  { /* ... */ }
    [RelayCommand] void Edit()   { /* ... */ }
    [RelayCommand] void Delete() { /* ... */ }
}
```

## Blazor

Same look and the same knobs. `Fab` is a component; `FabMenu` takes a `List<FabMenuItem>` of plain data
objects rather than child components. `Icon` is a string — an inline emoji, raw SVG markup, or an image
URL (anything starting with `http`/`/` or ending in `.png`/`.jpg`/`.svg`/`.webp` is treated as an image).

```razor
@using Shiny.Blazor.Controls

<FabMenu Items="items"
         Icon="+"
         FabBackgroundColor="#7C3AED"
         ItemTapped="OnItemTapped" />

@code {
    readonly List<FabMenuItem> items = new()
    {
        new FabMenuItem { Text = "New Note",     Icon = "📝", FabBackgroundColor = "#10B981", Tag = "note"  },
        new FabMenuItem { Text = "New Photo",    Icon = "📷", FabBackgroundColor = "#F59E0B", Tag = "photo" },
        new FabMenuItem { Text = "New Reminder", Icon = "⏰", FabBackgroundColor = "#EF4444", Tag = "alarm" },
    };

    void OnItemTapped(FabMenuItem item) { /* item.Tag */ }
}
```

### FabMenu Parameters (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| `Items` | `List<FabMenuItem>?` | `null` | Menu items |
| `IsOpen` / `IsOpenChanged` | `bool` | `false` | Two-way bindable open state |
| `Icon` / `Text` | `string?` | `null` | Main FAB icon / label |
| `FabBackgroundColor` | `string` | `var(--shiny-color-primary, #2196F3)` | Main FAB fill |
| `TextColor` | `string` | `var(--shiny-color-on-primary, #FFFFFF)` | Main FAB label color |
| `BorderColor` / `BorderThickness` | `string?` / `double` | `null` / `0` | Main FAB outline |
| `FabSize` | `double` | `56` | Main FAB size |
| `HasShadow` | `bool` | `true` | Drop shadow on the main FAB |
| `MenuAlignment` | `string` | `"end"` | `"start"` grows the menu from the left and flips the chip to the leading edge |
| `IconRotation` | `double` | `45` | Degrees the main FAB rotates while open (0 disables; ignored when `Text` is set) |
| `HasBackdrop` | `bool` | `true` | Dim + 2px-blur backdrop while open |
| `BackdropColor` | `string` | `var(--shiny-color-scrim, #000000)` | Backdrop color |
| `BackdropOpacity` | `double` | `0.4` | Backdrop peak opacity |
| `CloseOnBackdropTap` / `CloseOnItemTap` | `bool` | `true` | Auto-close behavior |
| `ItemTapped` | `EventCallback<FabMenuItem>` | — | Fires the tapped item |

Methods: `Open()`, `Close()`, `Toggle()` (via `@ref`).

### FabMenuItem (Blazor, plain class)

`Icon`, `Text`, `Tag` (`object?`, for identifying the item in `ItemTapped`), plus `FabBackgroundColor`,
`TextColor`, `LabelBackgroundColor`, `BorderColor`, `BorderThickness` (default `1`), `Size` (`44`),
`IconSize` (`20`), `FontSize` (`13`), `HasShadow` (`true`). Colors are CSS strings and default to the
theme variables, so leaving them alone is the right call unless an item needs its own tint.

- The host element is `display:inline-flex` and positions itself — wrap it in a `position:relative`
  container and place it with `position:absolute; right:24px; bottom:24px` (or `position:fixed` for a
  page-level FAB)
- The backdrop is `position:fixed; inset:0`, so it always covers the viewport regardless of the host
- All motion honors `prefers-reduced-motion`
