# CarouselGallery

A Netflix-style horizontal carousel with snap-to-center behavior, scale transforms on focused/unfocused items, and configurable peek insets that reveal adjacent items. Supports infinite looping, load-more, empty/header/footer templates, and position tracking. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls.CarouselGallery`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

Native handlers: Android RecyclerView + LinearSnapHelper, iOS UICollectionView + CarouselFlowLayout, Windows ItemsRepeater + ScrollViewer.

Extends `CollectionControlBase` (which extends `View`).

### Basic Usage

```xml
<shiny:CarouselGallery ItemsSource="{Binding Movies}"
                       CurrentPosition="{Binding SelectedIndex, Mode=TwoWay}"
                       ItemWidth="280"
                       ItemHeight="180"
                       FocusedItemScale="1.0"
                       UnfocusedItemScale="0.8"
                       PeekAreaInsets="40,0"
                       ItemSpacing="12"
                       ItemSelectedCommand="{Binding SelectMovieCommand}">
    <shiny:CarouselGallery.ItemTemplate>
        <DataTemplate>
            <Border StrokeShape="{RoundRectangle CornerRadius=12}">
                <Image Source="{Binding Thumbnail}" Aspect="AspectFill" />
            </Border>
        </DataTemplate>
    </shiny:CarouselGallery.ItemTemplate>
</shiny:CarouselGallery>
```

### Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| ItemsSource | IEnumerable | null | OneWay | Collection of items to display |
| ItemTemplate | DataTemplate | null | OneWay | Template for each carousel item |
| ItemTemplateSelector | DataTemplateSelector | null | OneWay | Dynamic template selector per item |
| FocusedItemScale | double | 1.0 | OneWay | Scale applied to the centered/focused item |
| UnfocusedItemScale | double | 0.8 | OneWay | Scale applied to non-focused items |
| ItemWidth | double | — | OneWay | Width of each carousel item |
| ItemHeight | double | — | OneWay | Height of each carousel item |
| CurrentPosition | int | 0 | TwoWay | Index of the currently centered item |
| PeekAreaInsets | Thickness | 0 | OneWay | Insets that reveal edges of adjacent items |
| IsInfinite | bool | false | OneWay | Loop the carousel infinitely |
| ItemSpacing | double | 0 | OneWay | Space between items |
| PositionChangedCommand | ICommand | null | OneWay | Command fired when the centered item changes |
| ItemSelectedCommand | ICommand | null | OneWay | Command fired when an item is tapped; receives the item as parameter |
| HeaderTemplate | DataTemplate | null | OneWay | Template rendered before the first item |
| FooterTemplate | DataTemplate | null | OneWay | Template rendered after the last item |
| EmptyViewTemplate | DataTemplate | null | OneWay | Template shown when ItemsSource is empty |
| LoadMoreCommand | ICommand | null | OneWay | Command invoked when approaching the end of the list |
| LoadMoreThreshold | int | 5 | OneWay | Number of items from the end that triggers LoadMoreCommand |

### Events

| Event | Args | Description |
|---|---|---|
| ItemSelected | object | Fired when the user taps an item; args is the bound item |
| LoadMoreRequested | EventArgs | Fired when the scroll position crosses the LoadMoreThreshold |
| ItemAppearing | object | Fired when an item scrolls into view |
| ItemDisappearing | object | Fired when an item scrolls out of view |

### Behavior

- The centered item is scaled to `FocusedItemScale`; all others use `UnfocusedItemScale`
- `PeekAreaInsets` exposes the edges of the previous/next items to hint scrollability
- When `IsInfinite` is `true` the list wraps seamlessly at both ends
- `LoadMoreCommand` fires when the user is within `LoadMoreThreshold` items of the end
- Use `ItemTemplateSelector` instead of `ItemTemplate` when items have heterogeneous layouts

### Infinite Carousel Example

```xml
<shiny:CarouselGallery ItemsSource="{Binding Banners}"
                       IsInfinite="True"
                       ItemWidth="360"
                       ItemHeight="200"
                       PeekAreaInsets="24,0"
                       CurrentPosition="{Binding BannerIndex, Mode=TwoWay}">
    <shiny:CarouselGallery.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding ImageUrl}" Aspect="AspectFill" />
        </DataTemplate>
    </shiny:CarouselGallery.ItemTemplate>
</shiny:CarouselGallery>
```

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

Generic component `CarouselGallery<TItem>` with `@typeparam TItem`. Uses CSS scroll-snap for horizontal snapping; no JS framework dependency.

### Basic Usage

```razor
<CarouselGallery TItem="Movie"
                 Items="movies"
                 ItemWidth="280"
                 ItemHeight="180"
                 FocusedItemScale="1.0"
                 UnfocusedItemScale="0.85"
                 ItemSpacing="12"
                 PeekAmount="40"
                 ShowIndicators="true"
                 @bind-CurrentPosition="selectedIndex"
                 ItemSelected="OnMovieSelected">
    <ItemTemplate Context="movie">
        <div class="card" style="border-radius:12px; overflow:hidden;">
            <img src="@movie.Thumbnail" style="width:100%; height:100%; object-fit:cover;" />
        </div>
    </ItemTemplate>
</CarouselGallery>

@code {
    List<Movie> movies = [];
    int selectedIndex = 0;

    void OnMovieSelected(Movie movie) { /* handle */ }
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| Items | IReadOnlyList\<TItem\> | — | Collection of items to render |
| ItemTemplate | RenderFragment\<TItem\> | — | Render fragment for each item |
| FocusedItemScale | double | 1.0 | CSS scale applied to the centered item |
| UnfocusedItemScale | double | 0.85 | CSS scale applied to all other items |
| ItemWidth | double | 300 | Item width in px |
| ItemHeight | double | 200 | Item height in px |
| ItemSpacing | double | 16 | Gap between items in px |
| PeekAmount | double | 40 | Pixels of adjacent items visible on each side |
| CurrentPosition | int | 0 | Index of the currently snapped item (two-way via CurrentPositionChanged) |
| ShowIndicators | bool | true | Render dot indicators below the carousel |
| ItemSelected | EventCallback\<TItem\> | — | Callback invoked when an item is clicked |
| AdditionalAttributes | IDictionary\<string, object\>? | null | Splatted onto the root element |

### Code Generation Guidance

- The host element uses `overflow-x: scroll; scroll-snap-type: x mandatory` with `scrollbar-width: none` to hide the native scrollbar
- Each item uses `scroll-snap-align: center; flex-shrink: 0`
- Scale transitions use `transition: transform 0.3s ease` so the focused item animates smoothly as the user scrolls
- `PeekAmount` is applied as left/right padding on the scroll container so edges of neighbours are visible
- `ShowIndicators` renders a row of `<button>` dots; the active dot reflects `CurrentPosition`
- Two-way binding on `CurrentPosition` lets the parent drive the carousel programmatically (e.g. auto-advance timer)
- When `Items` is null or empty no items are rendered; provide an alternative empty state via surrounding markup
