# VirtualizedGrid

A full-featured grouped grid with sticky section headers, native virtualization, adaptive portrait/landscape column counts, and load-more support. Designed for large datasets where off-screen views must be recycled. Available on both MAUI and Blazor.

## MAUI

**Namespace**: `Shiny.Maui.Controls.VirtualizedGrid`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

Native handlers: Android RecyclerView + GridLayoutManager + StickyHeaderDecoration, iOS UICollectionView + CompositionalLayout with pinned section headers, Windows ItemsRepeater + UniformGridLayout.

Extends `CollectionControlBase` (which extends `View`).

### Basic Usage

```xml
<shiny:VirtualizedGrid ItemsSource="{Binding Products}"
                       ColumnCount="2"
                       PortraitColumnCount="2"
                       LandscapeColumnCount="4"
                       CellPadding="8"
                       IsGroupingEnabled="True"
                       HasStickyHeaders="True"
                       LoadMoreCommand="{Binding LoadMoreCommand}"
                       LoadMoreThreshold="8"
                       ItemSelectedCommand="{Binding SelectProductCommand}">
    <shiny:VirtualizedGrid.ItemTemplate>
        <DataTemplate>
            <Border StrokeShape="{RoundRectangle CornerRadius=10}" Padding="8">
                <VerticalStackLayout>
                    <Image Source="{Binding ImageUrl}" HeightRequest="120" Aspect="AspectFill" />
                    <Label Text="{Binding Name}" FontAttributes="Bold" />
                    <Label Text="{Binding Price, StringFormat='{0:C}'}" TextColor="#6B7280" />
                </VerticalStackLayout>
            </Border>
        </DataTemplate>
    </shiny:VirtualizedGrid.ItemTemplate>
    <shiny:VirtualizedGrid.GroupHeaderTemplate>
        <DataTemplate>
            <Label Text="{Binding Key}" FontAttributes="Bold" FontSize="16"
                   BackgroundColor="{AppThemeBinding Light=#F3F4F6, Dark=#1F2937}"
                   Padding="12,8" />
        </DataTemplate>
    </shiny:VirtualizedGrid.GroupHeaderTemplate>
</shiny:VirtualizedGrid>
```

### Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| ColumnCount | int | 1 | OneWay | Default number of columns |
| PortraitColumnCount | int? | null | OneWay | Column count override when device is in portrait orientation |
| LandscapeColumnCount | int? | null | OneWay | Column count override when device is in landscape orientation |
| IsGroupingEnabled | bool | false | OneWay | Enable grouped data with section headers |
| GroupHeaderTemplate | DataTemplate | null | OneWay | Template for group/section header rows |
| HasStickyHeaders | bool | true | OneWay | Pin section headers to the top while scrolling through the group |
| CellPadding | Thickness | 0 | OneWay | Padding applied inside each cell |
| ShowLoadMoreButton | bool | false | OneWay | Show an explicit load-more button at the bottom instead of auto-triggering |
| LoadMoreButtonTemplate | DataTemplate | null | OneWay | Custom template for the load-more button |
| IsLoadingMore | bool | false | OneWayToSource | Set by the control while a load-more operation is in progress |
| ItemVisibleCommand | ICommand | null | OneWay | Command fired when an item enters the viewport |
| ItemHiddenCommand | ICommand | null | OneWay | Command fired when an item leaves the viewport |
| ItemsSource | IEnumerable | null | OneWay | Collection of items (flat or grouped) |
| ItemTemplate | DataTemplate | null | OneWay | Template for each grid cell |
| ItemTemplateSelector | DataTemplateSelector | null | OneWay | Dynamic template selector per item |
| ItemSelectedCommand | ICommand | null | OneWay | Command fired when a cell is tapped; receives the item as parameter |
| EmptyViewTemplate | DataTemplate | null | OneWay | Template shown when ItemsSource is empty |
| HeaderTemplate | DataTemplate | null | OneWay | Template rendered at the very top of the list |
| FooterTemplate | DataTemplate | null | OneWay | Template rendered at the very bottom of the list |
| ItemSpacing | double | 0 | OneWay | Uniform space between cells |
| LoadMoreCommand | ICommand | null | OneWay | Command invoked when approaching the end of the list |
| LoadMoreThreshold | int | 5 | OneWay | Items from the end that triggers LoadMoreCommand |

### Events

| Event | Args | Description |
|---|---|---|
| ItemSelected | object | Fired when the user taps a cell; args is the bound item |
| LoadMoreRequested | EventArgs | Fired when scroll position crosses LoadMoreThreshold |
| ItemAppearing | object | Fired when a cell enters the viewport |
| ItemDisappearing | object | Fired when a cell leaves the viewport |

### Behavior

- When `PortraitColumnCount` or `LandscapeColumnCount` is set they override `ColumnCount` for that orientation automatically on device rotation
- `IsGroupingEnabled` requires `ItemsSource` to contain grouped data; each group exposes a `Key` property used by `GroupHeaderTemplate`
- `HasStickyHeaders` pins the current group header at the top of the visible area as the user scrolls — supported natively on all three platforms
- `IsLoadingMore` is `OneWayToSource`: the control sets it to `true` while waiting for `LoadMoreCommand` to complete, then resets it; bind it to a ViewModel property to show a spinner
- When `ShowLoadMoreButton` is `true` the auto-trigger is disabled and a button rendered via `LoadMoreButtonTemplate` is shown instead

### Adaptive Columns + Load-More Button Example

```xml
<shiny:VirtualizedGrid ItemsSource="{Binding Items}"
                       PortraitColumnCount="2"
                       LandscapeColumnCount="4"
                       ShowLoadMoreButton="True"
                       IsLoadingMore="{Binding IsBusy, Mode=OneWayToSource}"
                       LoadMoreCommand="{Binding FetchNextPageCommand}">
    <shiny:VirtualizedGrid.LoadMoreButtonTemplate>
        <DataTemplate>
            <Button Text="Load more" Command="{Binding LoadMoreCommand}" />
        </DataTemplate>
    </shiny:VirtualizedGrid.LoadMoreButtonTemplate>
    <shiny:VirtualizedGrid.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding Title}" />
        </DataTemplate>
    </shiny:VirtualizedGrid.ItemTemplate>
</shiny:VirtualizedGrid>
```

## Blazor

**Namespace**: `Shiny.Blazor.Controls`

Generic component `VirtualizedGrid<TItem>` with `@typeparam TItem`. Uses CSS Grid for layout and the Blazor `Virtualize<T>` component for DOM-level virtualization when `EnableVirtualization` is `true`.

`VirtualizedGridGroup<TItem>` class: `Key` (object), `Items` (IReadOnlyList\<TItem\>).

### Basic Usage

```razor
<VirtualizedGrid TItem="Product"
                 Items="products"
                 ColumnCount="3"
                 ItemSpacing="12"
                 IsGroupingEnabled="false"
                 EnableVirtualization="true"
                 LoadMoreThreshold="8"
                 LoadMoreRequested="OnLoadMore"
                 ItemSelected="OnProductSelected">
    <ItemTemplate Context="product">
        <div class="product-card">
            <img src="@product.ImageUrl" style="width:100%;" />
            <p>@product.Name</p>
            <p>@product.Price.ToString("C")</p>
        </div>
    </ItemTemplate>
    <EmptyViewTemplate>
        <p>No products found.</p>
    </EmptyViewTemplate>
</VirtualizedGrid>

@code {
    List<Product> products = [];

    async Task OnLoadMore() { /* fetch next page */ }
    void OnProductSelected(Product p) { /* handle */ }
}
```

### Grouped Usage

```razor
<VirtualizedGrid TItem="Contact"
                 IsGroupingEnabled="true"
                 GroupedItems="groupedContacts"
                 HasStickyHeaders="true"
                 ColumnCount="1"
                 ItemSelected="OnContactSelected">
    <ItemTemplate Context="contact">
        <div>@contact.Name</div>
    </ItemTemplate>
    <GroupHeaderTemplate Context="key">
        <div style="position:sticky; top:0; background:#f3f4f6; padding:4px 12px; font-weight:bold;">
            @key
        </div>
    </GroupHeaderTemplate>
</VirtualizedGrid>

@code {
    List<VirtualizedGridGroup<Contact>> groupedContacts = [];
    void OnContactSelected(Contact c) { /* handle */ }
}
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| Items | IReadOnlyList\<TItem\>? | null | Flat collection of items (used when IsGroupingEnabled is false) |
| ItemTemplate | RenderFragment\<TItem\> | — | Render fragment for each cell |
| HeaderTemplate | RenderFragment? | null | Content rendered above the grid |
| FooterTemplate | RenderFragment? | null | Content rendered below the grid |
| EmptyViewTemplate | RenderFragment? | null | Content shown when items collection is empty |
| ColumnCount | int | 1 | Number of CSS grid columns |
| ItemSpacing | double | 8 | Gap between cells in px (applied to both row and column gaps) |
| CellPaddingLeft | double | 0 | Left padding inside each cell in px |
| CellPaddingRight | double | 0 | Right padding inside each cell in px |
| CellPaddingTop | double | 0 | Top padding inside each cell in px |
| CellPaddingBottom | double | 0 | Bottom padding inside each cell in px |
| IsGroupingEnabled | bool | false | Render data as grouped sections with headers |
| GroupedItems | IReadOnlyList\<VirtualizedGridGroup\<TItem\>\>? | null | Grouped data source (used when IsGroupingEnabled is true) |
| GroupHeaderTemplate | RenderFragment\<object\>? | null | Render fragment for each group header; context is the group Key |
| HasStickyHeaders | bool | true | Pin group headers using CSS `position: sticky` |
| EnableVirtualization | bool | false | Use Blazor Virtualize component to limit rendered DOM nodes |
| LoadMoreThreshold | int | 5 | Items from the end that triggers LoadMoreRequested |
| ShowLoadMoreButton | bool | false | Show an explicit load-more button instead of auto-triggering |
| IsLoadingMore | bool | false | Indicates a load-more operation is in progress (shows spinner or disables button) |
| LoadMoreButtonTemplate | RenderFragment? | null | Custom content for the load-more button; defaults to a standard button |
| LoadMoreRequested | EventCallback | — | Callback invoked when more data should be fetched |
| ItemSelected | EventCallback\<TItem\> | — | Callback invoked when a cell is clicked |
| AdditionalAttributes | IDictionary\<string, object\>? | null | Splatted onto the root element |

### Code Generation Guidance

- The grid container uses `display: grid; grid-template-columns: repeat({ColumnCount}, 1fr); gap: {ItemSpacing}px`
- When `IsGroupingEnabled` is `true` render each `VirtualizedGridGroup<TItem>` as a section: a full-width header row (`grid-column: 1 / -1`) followed by the group's items
- `HasStickyHeaders` adds `position: sticky; top: 0; z-index: 1` to each group header element; ensure the scroll container has `overflow-y: auto` and a defined height
- When `EnableVirtualization` is `true` wrap the item loop with `<Virtualize Items="Items" Context="item">` — note that `Virtualize` requires a flat list; for grouped data virtualize at the group level or flatten before passing
- `ShowLoadMoreButton` disables threshold-based auto-loading; render the `LoadMoreButtonTemplate` (or a default `<button>`) at the end of the list and wire its click to `LoadMoreRequested`
- `IsLoadingMore` can be used to show a spinner adjacent to or replacing the load-more button while the fetch is pending
- Cell padding parameters map to inline `padding: {Top}px {Right}px {Bottom}px {Left}px` on each cell wrapper
- `EmptyViewTemplate` is rendered in place of the grid when `Items` (or all groups) contain zero elements
