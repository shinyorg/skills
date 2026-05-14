# StaggeredGrid

A Pinterest-style masonry/waterfall layout where items occupy variable heights in columns, producing a dense staggered appearance without uniform row heights. Supports item selection, load-more, empty/header/footer templates, and virtualization via native platform recyclers. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls.StaggeredGrid`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

Native handlers: Android RecyclerView + StaggeredGridLayoutManager, iOS UICollectionView + WaterfallLayout, Windows ItemsRepeater + WaterfallVirtualizingLayout.

Extends `CollectionControlBase` (which extends `View`).

### Basic Usage

```xml
<shiny:StaggeredGrid ItemsSource="{Binding Photos}"
                     ColumnCount="2"
                     ColumnSpacing="8"
                     RowSpacing="8"
                     ItemSelectedCommand="{Binding OpenPhotoCommand}"
                     LoadMoreCommand="{Binding LoadMoreCommand}"
                     LoadMoreThreshold="6">
    <shiny:StaggeredGrid.ItemTemplate>
        <DataTemplate>
            <Border StrokeShape="{RoundRectangle CornerRadius=8}">
                <Image Source="{Binding Url}" Aspect="AspectFill" HeightRequest="{Binding CardHeight}" />
            </Border>
        </DataTemplate>
    </shiny:StaggeredGrid.ItemTemplate>
</shiny:StaggeredGrid>
```

### Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| ColumnCount | int | 2 | OneWay | Number of columns (minimum 1) |
| ColumnSpacing | double | 0 | OneWay | Horizontal space between columns |
| RowSpacing | double | 0 | OneWay | Vertical space between items in a column |
| ItemsSource | IEnumerable | null | OneWay | Collection of items to display |
| ItemTemplate | DataTemplate | null | OneWay | Template for each grid item |
| ItemTemplateSelector | DataTemplateSelector | null | OneWay | Dynamic template selector per item |
| ItemSelectedCommand | ICommand | null | OneWay | Command fired when an item is tapped; receives the item as parameter |
| EmptyViewTemplate | DataTemplate | null | OneWay | Template shown when ItemsSource is empty |
| HeaderTemplate | DataTemplate | null | OneWay | Template rendered above the grid |
| FooterTemplate | DataTemplate | null | OneWay | Template rendered below the grid |
| ItemSpacing | double | 0 | OneWay | Uniform spacing applied around each item |
| LoadMoreCommand | ICommand | null | OneWay | Command invoked when approaching the end |
| LoadMoreThreshold | int | 5 | OneWay | Items from end that triggers LoadMoreCommand |

### Events

| Event | Args | Description |
|---|---|---|
| ItemSelected | object | Fired when the user taps an item; args is the bound item |
| LoadMoreRequested | EventArgs | Fired when scroll position crosses LoadMoreThreshold |
| ItemAppearing | object | Fired when an item enters the viewport |
| ItemDisappearing | object | Fired when an item leaves the viewport |

### Behavior

- Each column fills independently; items are placed into whichever column is currently shortest
- Native platform recyclers handle view reuse and off-screen virtualization automatically
- `ColumnCount` must be at least 1; values above the screen width cause layout degradation
- `ItemSpacing` applies uniform margin around each cell; use `ColumnSpacing`/`RowSpacing` for directional control
- `LoadMoreCommand` is rate-limited so it does not fire repeatedly while a load is in progress

### Three-Column Example

```xml
<shiny:StaggeredGrid ItemsSource="{Binding Pins}"
                     ColumnCount="3"
                     ColumnSpacing="4"
                     RowSpacing="4">
    <shiny:StaggeredGrid.EmptyViewTemplate>
        <DataTemplate>
            <Label Text="No pins yet" HorizontalOptions="Center" />
        </DataTemplate>
    </shiny:StaggeredGrid.EmptyViewTemplate>
    <shiny:StaggeredGrid.ItemTemplate>
        <DataTemplate>
            <Image Source="{Binding ImageUrl}" Aspect="AspectFill" />
        </DataTemplate>
    </shiny:StaggeredGrid.ItemTemplate>
</shiny:StaggeredGrid>
```

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

Generic component `StaggeredGrid<TItem>` with `@typeparam TItem`. Uses CSS `column-count` for masonry layout with `break-inside: avoid` on each item so content is never split across columns.

### Basic Usage

```razor
<StaggeredGrid TItem="Photo"
               Items="photos"
               ColumnCount="3"
               ColumnSpacing="12"
               RowSpacing="12"
               ItemSelected="OnPhotoSelected">
    <ItemTemplate Context="photo">
        <div style="border-radius:8px; overflow:hidden;">
            <img src="@photo.Url" style="width:100%; display:block;" />
        </div>
    </ItemTemplate>
</StaggeredGrid>

@code {
    List<Photo> photos = [];

    void OnPhotoSelected(Photo photo) { /* handle */ }
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| Items | IReadOnlyList\<TItem\> | — | Collection of items to render |
| ItemTemplate | RenderFragment\<TItem\> | — | Render fragment for each item |
| EmptyTemplate | RenderFragment? | null | Content shown when Items is null or empty |
| ColumnCount | int | 2 | CSS column-count value |
| ColumnSpacing | double | 16 | Horizontal gap between columns in px |
| RowSpacing | double | 16 | Vertical gap between items in px (applied as bottom margin) |
| ItemSelected | EventCallback\<TItem\> | — | Callback invoked when an item is clicked |
| AdditionalAttributes | IDictionary\<string, object\>? | null | Splatted onto the root element |

### Code Generation Guidance

- The root element uses `column-count: {ColumnCount}; column-gap: {ColumnSpacing}px`
- Each item wrapper uses `break-inside: avoid; margin-bottom: {RowSpacing}px` to prevent column breaks mid-item and to add vertical rhythm
- Items flow top-to-bottom then left-to-right (CSS multi-column order); this differs from the MAUI shortest-column algorithm — the visual result is similar but insertion order matters
- `EmptyTemplate` is rendered when `Items` is null or has zero elements; use it to show a placeholder or call-to-action
- `ItemSelected` fires on the item click event; the `TItem` value is passed directly to the callback
- For variable-height images, ensure `<img>` tags have `width: 100%; height: auto; display: block` so column widths are respected
- Avoid fixed heights on item wrappers — the masonry effect relies on the browser distributing natural content heights across columns
