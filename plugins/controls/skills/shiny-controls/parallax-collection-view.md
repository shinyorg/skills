# ParallaxCollectionView (MAUI) / ParallaxList (Blazor)

A scrollable collection with a hero header that translates at a configurable fraction of the scroll offset, producing the familiar "parallax" effect seen in app-store and profile pages.

Both hosts are pure cross-platform — **no platform handlers**:

- **MAUI** (`Shiny.Maui.Controls.ParallaxCollectionView.ParallaxCollectionView`): a `ContentView` that hosts a real `CollectionView` plus a hero `ContentView` in a `Grid` and drives the translation from `CollectionView.Scrolled`.
- **Blazor** (`Shiny.Blazor.Controls.ParallaxList<TItem>`): a scrollable container with an absolutely-positioned hero. A tiny JS scroll listener mutates the hero `transform`/`opacity` directly (rAF-throttled) so parallax runs at native scroll framerate without going through Blazor's render loop.

## Basic Usage — MAUI

```xml
<shiny:ParallaxCollectionView ItemsSource="{Binding Items}"
                              HeaderHeight="260"
                              MinHeaderHeight="96"
                              ParallaxFactor="0.5"
                              CollapseToSticky="True"
                              FadeHeaderOnScroll="False"
                              SelectionMode="Single"
                              ItemSelectedCommand="{Binding ItemSelectedCommand}">

    <shiny:ParallaxCollectionView.HeaderTemplate>
        <DataTemplate>
            <Grid>
                <Grid.Background>
                    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
                        <GradientStop Color="#7C3AED" Offset="0.0" />
                        <GradientStop Color="#2563EB" Offset="0.5" />
                        <GradientStop Color="#0EA5E9" Offset="1.0" />
                    </LinearGradientBrush>
                </Grid.Background>
                <Label Text="Destinations"
                       FontSize="28" FontAttributes="Bold" TextColor="White"
                       VerticalOptions="Center" HorizontalOptions="Center" />
            </Grid>
        </DataTemplate>
    </shiny:ParallaxCollectionView.HeaderTemplate>

    <shiny:ParallaxCollectionView.ItemTemplate>
        <DataTemplate>
            <Border Margin="16,6" Padding="16">
                <Label Text="{Binding Title}" FontAttributes="Bold" />
            </Border>
        </DataTemplate>
    </shiny:ParallaxCollectionView.ItemTemplate>
</shiny:ParallaxCollectionView>
```

## Basic Usage — Blazor

```razor
<div style="height:600px;">
    <ParallaxList TItem="DestinationItem"
                  Items="@items"
                  HeaderHeight="260"
                  MinHeaderHeight="96"
                  ParallaxFactor="0.5"
                  CollapseToSticky="true"
                  FadeHeaderOnScroll="false"
                  ItemSelected="OnSelected"
                  Scrolled="OnScrolled">
        <HeroTemplate>
            <div style="height:100%;background:linear-gradient(135deg,#7C3AED,#2563EB,#0EA5E9);
                        color:white;display:flex;align-items:center;justify-content:center;
                        font-size:28px;font-weight:700;">
                Destinations
            </div>
        </HeroTemplate>
        <ItemTemplate Context="item">
            <div style="margin:6px 16px;padding:16px;background:white;border-radius:14px;">
                <strong>@item.Title</strong>
            </div>
        </ItemTemplate>
        <EmptyTemplate>
            <p>No items.</p>
        </EmptyTemplate>
    </ParallaxList>
</div>

@code {
    record DestinationItem(string Title);
    List<DestinationItem> items = [];
    void OnSelected(DestinationItem item) { /* ... */ }
    void OnScrolled(ParallaxScrollEventArgs e) { /* e.HeaderVisibleHeight, ... */ }
}
```

> Blazor: `<ParallaxList>` fills its parent. Place it in a container with a fixed `height` (px, vh, etc.) so it has something to scroll inside.

## Properties

| Property | MAUI Type | Blazor Type | Default | Description |
|---|---|---|---|---|
| `ItemsSource` / `Items` | `IEnumerable` | `IReadOnlyList<TItem>` | — | Collection of items to display |
| `ItemTemplate` | `DataTemplate` | `RenderFragment<TItem>` | — | Template for each row |
| `HeaderTemplate` / `HeroTemplate` | `DataTemplate` | `RenderFragment` | — | Template for the parallax hero |
| `EmptyView` / `EmptyTemplate` | `object` / `DataTemplate` | `RenderFragment` | — | Shown when the source is null/empty |
| `HeaderHeight` | `double` | `double` | 240 | Height of the hero in px |
| `MinHeaderHeight` | `double` | `double` | 0 | Minimum visible hero height when `CollapseToSticky` is true |
| `ParallaxFactor` | `double` | `double` | 0.5 | Fraction of scroll offset applied to hero translation (0 = pinned, 1 = scrolls with content) |
| `CollapseToSticky` | `bool` | `bool` | false | Clamp hero to `MinHeaderHeight` once it has scrolled that far |
| `FadeHeaderOnScroll` | `bool` | `bool` | false | Fade hero 100% → 0% opacity as it scrolls past `HeaderHeight` |
| `ItemsLayout` (MAUI) | `IItemsLayout` | — | `LinearItemsLayout.Vertical` | Passthrough to inner `CollectionView` — use `GridItemsLayout` for multi-column lists |
| `SelectionMode` (MAUI) | `SelectionMode` | — | `None` | Passthrough to inner `CollectionView` |
| `SelectedItem` (MAUI) | `object` | — | — | TwoWay selected item |
| `ItemSelectedCommand` (MAUI) | `ICommand` | — | — | Fired on selection change |
| `ItemSelected` (Blazor) | — | `EventCallback<TItem>` | — | Fired when a row is clicked |
| `Height` (Blazor) | — | `string` | — | CSS height for the scroll container; omit to fill parent |
| `CssClass` (Blazor) | — | `string` | — | Extra class names on the root element |

## Events

Both hosts fire a `Scrolled` event with `ParallaxScrollEventArgs`:

- `VerticalOffset` — current scrollTop in px
- `HeaderTranslation` — negative px translation currently applied to the hero (clamped if `CollapseToSticky`)
- `HeaderVisibleHeight` — how many px of the hero are still visible (`HeaderHeight + HeaderTranslation`, floored at `MinHeaderHeight`)

Use this to drive a sticky title that fades in once the hero is mostly hidden, dim a nav bar, etc.

## Code-Generation Rules

- **MAUI**:
  - XAML namespace `xmlns:shiny="http://shiny.net/maui/controls"` (already registered for `Shiny.Maui.Controls.ParallaxCollectionView`).
  - The hero is whatever you put in `HeaderTemplate`. A `Grid` with a `LinearGradientBrush` background reads well and renders cheaply during the translation.
  - For multi-column item layouts, set `ItemsLayout` to a `<GridItemsLayout Span="2" Orientation="Vertical" />`. The hero stays full width.
  - Don't try to use a regular `CollectionView.Header` for the hero — `Header` scrolls with content at 1× and cannot be transformed independently. Use `HeaderTemplate` on `ParallaxCollectionView` instead; the control already handles the transparent placeholder header internally so the list scrolls over the hero correctly.
- **Blazor**:
  - The control's parent must have a fixed `height` (px, vh) — `<ParallaxList>` fills its container so it has something to scroll inside.
  - The JS interop module is loaded once per component instance from `./_content/Shiny.Blazor.Controls/parallax-list.js`. The scroll handler uses `requestAnimationFrame` to avoid re-rendering Razor on every scroll event.
  - To drive a sticky title once the hero collapses, listen to `Scrolled` and toggle a sibling element's visibility when `e.HeaderVisibleHeight <= MinHeaderHeight + threshold`.
- **Both**:
  - `ParallaxFactor = 0` pins the hero (no parallax). `ParallaxFactor = 1` makes it scroll with content (also no parallax). Use values in between.
  - `CollapseToSticky` requires `MinHeaderHeight > 0` to be meaningful — set both together.
  - `FadeHeaderOnScroll` and `CollapseToSticky` are independent and combine: the hero collapses to its minimum and then fades out.

## Common Pitfalls

- **Blazor: parent has no height** — the list won't scroll. Wrap it in a div with `style="height:600px"` (or `100vh`, etc.).
- **MAUI: nesting inside another `ScrollView`** — `CollectionView` does its own scrolling. Don't put `ParallaxCollectionView` inside a `ScrollView`; place it directly in a `Grid` row or as the page `Content`.
- **Setting both `ParallaxFactor=1` and `CollapseToSticky=True`** — produces a no-op (header scrolls 1:1 with content so it never reaches its collapsed state from the parallax math). Lower `ParallaxFactor` to let the collapse engage.

## When to Use What

- **Hero image + scrolling content, half-speed translation** → `ParallaxFactor = 0.5`, `CollapseToSticky = false`.
- **App-store-style collapsing header that becomes a sticky title bar** → `ParallaxFactor = 0.5–0.7`, `CollapseToSticky = true`, `MinHeaderHeight = 56–96`.
- **Header that fades out as you scroll, replaced by your own toolbar** → `FadeHeaderOnScroll = true`, listen to `Scrolled` and toggle the toolbar at the right offset.
- **Pure decorative parallax with no header collapse** → default settings (`HeaderHeight = 240`, `ParallaxFactor = 0.5`).
