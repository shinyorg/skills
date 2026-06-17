# DataGrid

A feature-rich data grid for **both MAUI and Blazor**, modeled on MudBlazor's `MudDataGrid`. Blazor
renders a semantic HTML `<table>` via a generic `DataGrid<TItem>`; MAUI is a **pure cross-platform
composite** (a `Grid` header over a virtualized `CollectionView` — no native handlers), so it looks and
behaves the same on iOS/Android/Windows/Mac.

Feature surface (both hosts): typed columns (`PropertyColumn` + `TemplateColumn`), sorting (single +
multi), column **filtering** (menu / row / toolbar quick-search), **grouping** with expandable groups,
footer & group **aggregates**, single/multi **selection** with checkboxes, inline **editing**
(cell + form), **paging**, **virtualization**, column **resize / reorder**, sticky header, loading +
empty states, a `ServerData` delegate, and density/striped/bordered/hover styling. Colors follow the
theme tokens (`var(--shiny-color-*)` on Blazor, `ShinyThemeKeys.Color.*` on MAUI).

## Blazor

`@typeparam TItem`; columns are child components inside `<Columns>`. `TItem` cascades to columns, so
`PropertyColumn` only needs `Property`.

```razor
<DataGrid TItem="Person" Items="people"
          SelectionMode="DataGridSelectionMode.Multiple"
          SortMode="DataGridSortMode.Multiple"
          FilterMode="DataGridFilterMode.Menu"
          Groupable="true"
          EditMode="DataGridEditMode.Form"
          Dense="true" Striped="true" Hover="true" Bordered="true"
          FixedHeader="true" Height="420px"
          ColumnResizeMode="DataGridColumnResizeMode.Column"
          DragDropColumnReordering="true"
          CommittedItemChanges="OnSaved">
    <Columns>
        <PropertyColumn Property="x => x.FirstName" Title="First" />
        <PropertyColumn Property="x => x.Age" Format="N0" />
        <PropertyColumn Property="x => x.Salary" Format="C0">
            <FooterTemplate>Total: @people.Sum(p => p.Salary).ToString("C0")</FooterTemplate>
        </PropertyColumn>
        <TemplateColumn Title="Status" Sortable="false" Filterable="false">
            <CellTemplate>
                <Pill Text="@(context.Item.Active ? "Active" : "Inactive")"
                      Type="@(context.Item.Active ? PillType.Success : PillType.Caution)" />
            </CellTemplate>
            <EditTemplate>
                <input @bind="context.Item.FirstName" />
            </EditTemplate>
        </TemplateColumn>
    </Columns>
    <PagerContent>
        <DataGridPager TItem="Person" />
    </PagerContent>
</DataGrid>
```

- **Columns**: `PropertyColumn<TItem,TProperty>` (`Property="x => x.Name"`, `Format`, derives Title) and
  `TemplateColumn<TItem>` (`CellTemplate`/`EditTemplate`/`HeaderTemplate`/`FooterTemplate` with
  `context.Item`). Per-column flags: `Sortable`, `Filterable`, `Groupable`, `Editable`, `Hidden`,
  `Width`, `Resizable`, `StickyLeft`/`StickyRight`, `Aggregate`.
- **Grid params**: `Items`, `ServerData` (`Func<GridState, Task<GridData<TItem>>>`), `SelectionMode`,
  `SelectedItem(s)`, `SortMode`, `FilterMode`, `QuickFilter`, `Groupable`, `Virtualize`, `EditMode`,
  `EditTrigger`, `ReadOnly`, `RowsPerPage`, `FixedHeader`, `Height`, `Dense`, `Striped`, `Bordered`,
  `Hover`, `Outlined`, `Loading`, `RowClick`, `StartedEditingItem`/`CommittedItemChanges`/
  `CanceledEditingItem`, `ColumnResizeMode`, `DragDropColumnReordering`, `ToolbarContent`,
  `NoRecordsText`/`NoRecordsContent`, `LoadingContent`.
- **Paging**: put `<DataGridPager TItem="..." />` in `<PagerContent>`.

## MAUI

`shiny:DataGrid` with `shiny:DataGridColumn` / `shiny:DataGridTemplateColumn` children (items are
`object`; no generics — XAML-friendly). Bind a column by `PropertyName`.

```xml
<shiny:DataGrid ItemsSource="{Binding People}"
                SelectionMode="Multiple"
                SortMode="Multiple"
                FilterMode="Menu"
                Groupable="True"
                PageSize="20"
                EditMode="Form"
                AllowColumnResize="True"
                AllowColumnReorder="True"
                Striped="True" Bordered="True">
    <shiny:DataGridColumn Title="First" PropertyName="FirstName" Width="*" />
    <shiny:DataGridColumn Title="Age" PropertyName="Age" Width="Auto" />
    <shiny:DataGridColumn Title="Salary" PropertyName="Salary" StringFormat="{}{0:C0}" Width="*">
        <shiny:DataGridColumn.Aggregate>
            <shiny:DataGridAggregateDefinition Type="Sum" Format="C0" />
        </shiny:DataGridColumn.Aggregate>
    </shiny:DataGridColumn>
    <shiny:DataGridTemplateColumn Title="Status" Width="Auto" Editable="False">
        <shiny:DataGridTemplateColumn.CellTemplate>
            <DataTemplate><shiny:PillView Text="{Binding StatusText}" /></DataTemplate>
        </shiny:DataGridTemplateColumn.CellTemplate>
    </shiny:DataGridTemplateColumn>
</shiny:DataGrid>
```

- **Columns**: `DataGridColumn` (`PropertyName`, `Width` as `GridLength` star/auto/abs, `StringFormat`,
  `CellTemplate`/`HeaderTemplate`/`EditTemplate`/`FooterTemplate`, `Sortable`/`Filterable`/`Groupable`/
  `Editable`/`Resizable`/`IsVisible`, `Aggregate`). `DataGridTemplateColumn` for custom-only cells.
  Cell templates bind to the data item directly (e.g. `{Binding StatusText}`).
- **Grid params**: `ItemsSource`, `ServerData`, `SelectionMode`, `SelectedItem`/`SelectedItems`,
  `SortMode`, `FilterMode`, `Groupable`, `PageSize` (0 = no paging), `EditMode`, `EditTrigger`,
  `ReadOnly`, `AllowColumnResize`, `AllowColumnReorder`, `Dense`, `Striped`, `Bordered`,
  `ShowColumnHeaders`, `IsLoading`, `EmptyText`, `RowHeight`, `SelectionChanged`/`SelectionChangedCommand`,
  `StartedEditingItem`/`CommittedItemChanges`/`CanceledEditingItem` events.

## Behavior notes & platform nuances

- **Sorting**: click/tap a header to cycle asc → desc → none. In `Multiple` mode each header adds to the
  sort with an order badge.
- **Filtering**: `Menu` shows a per-column filter popup (type-aware operators); `Row` shows inline
  filter inputs under the header; `Toolbar` shows a single quick-search box that matches any column.
- **Editing**: Blazor `Cell` edits one cell on click (commit on blur/Enter, cancel on Escape); `Form`
  edits the whole row with Save/Cancel. MAUI uses **inline-row editing** (editors for editable columns +
  a Save/Cancel bar) for both modes — the touch-friendly model.
- **Reorder**: Blazor uses native HTML drag-and-drop on headers (`DragDropColumnReordering`); MAUI uses
  ‹ › reorder arrows on headers (`AllowColumnReorder`).
- **Virtualization**: Blazor opt-in via `Virtualize` (uses `<Virtualize>`, best with `FixedHeader`+`Height`,
  not combined with paging/grouping); MAUI gets it free from `CollectionView`.
- **AOT/trimming**: MAUI string-path value access uses reflection (annotated). For full trim/NativeAOT,
  set a column's `ValueGetter`/`ValueSetter`/`Comparer` to avoid reflection.

## Code Generation Guidance

- Prefer `PropertyColumn` (Blazor) / `DataGridColumn` with `PropertyName` (MAUI) for bound data; use
  `TemplateColumn`/`DataGridTemplateColumn` for custom cells, actions, or status badges.
- Enable only the features asked for (sorting/paging/filtering/grouping/editing) — they're independent
  toggles.
- Blazor paging needs `<PagerContent><DataGridPager TItem="..."/></PagerContent>`; MAUI paging is just
  `PageSize`.
- Leave colors unset to inherit the theme; the grid is light/dark aware.
