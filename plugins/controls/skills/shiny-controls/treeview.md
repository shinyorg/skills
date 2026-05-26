# TreeView

Hierarchical tree control with lazy-loaded branches, configurable expand/collapse icons, single or multi-selection, per-item `CanExpand`/`CanSelect` predicates, retry on load failure, optional guide lines, and drag/drop reorder. Available on **MAUI** and **Blazor**.

## Architecture

```
shiny:TreeView (ContentView > Grid > ScrollView > VerticalStackLayout)
  → TreeNodeView (per-row internal): indent + chevron + ItemTemplate content
```

The control flattens the hierarchy into a list on every expand/collapse. Drag/drop is event-only — the TreeView never mutates your data source.

## Data Model

You bind one of two data sources for **root** items:
- `ItemsSource: IEnumerable` — pre-materialized root list
- `RootLoader: Func<Task<IEnumerable<object>>>` — async loader; tree shows a centered spinner until it resolves

And one of two ways to get **children**:
- `ChildrenSelector: Func<object, IEnumerable<object>?>` — synchronous (return `null` to defer to loader)
- `ChildrenLoader: Func<object, Task<IEnumerable<object>>>` — async (cached on the node after first call)

**Both can be set together.** The selector runs first; if it returns `null` and a loader is set, the loader runs. This lets a single tree mix sync and lazy branches.

## Bindable Properties (MAUI)

### Data
| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IEnumerable` | `null` | Root items (ignored when `RootLoader` is set) |
| `RootLoader` | `Func<Task<IEnumerable<object>>>` | `null` | Async loader for roots |
| `ChildrenSelector` | `Func<object, IEnumerable<object>?>` | `null` | Sync children getter |
| `ChildrenLoader` | `Func<object, Task<IEnumerable<object>>>` | `null` | Async children loader (fallback when selector returns null) |
| `HasChildrenSelector` | `Func<object, bool>` | `null` | Whether to render a chevron at all — leaves don't get one |
| `CanExpandSelector` | `Func<object, bool>` | `null` | Gate the expand gesture (chevron renders dimmed when false) |
| `CanSelectSelector` | `Func<object, bool>` | `null` | Gate selection per item |
| `ItemTemplate` | `DataTemplate` | `null` | Row content template |

### Selection
| Property | Type | Default | Description |
|---|---|---|---|
| `SelectionMode` | `TreeSelectionMode` | `Single` | `None` / `Single` / `Multiple` |
| `SelectedItem` | `object?` | `null` | Two-way bindable for `Single` mode |
| `SelectedItems` | `IList<object>?` | `null` | Two-way bindable for `Multiple` mode |

### Icons
| Property | Type | Default | Description |
|---|---|---|---|
| `ExpandedIcon` | `ImageSource?` | `null` (falls back to ▼) | Shown when the node is expanded |
| `CollapsedIcon` | `ImageSource?` | `null` (falls back to ▶) | Shown when the node is collapsed |
| `RetryIcon` | `ImageSource?` | `null` (falls back to ↻) | Shown when a lazy load fails; tap retries |
| `ChevronColor` | `Color` | `Gray` | Color of the default glyph chevron |
| `ChevronSize` | `double` | `16` | Pixel size of the chevron |

### Layout / visuals
| Property | Type | Default | Description |
|---|---|---|---|
| `IndentSize` | `double` | `20` | Horizontal indent per depth level (pixels) |
| `RowPadding` | `Thickness` | `8,6` | Padding inside each row |
| `RowSpacing` | `double` | `0` | Vertical spacing between rows |
| `ShowGuideLines` | `bool` | `false` | Vertical connector lines between parents and children |
| `GuideLineColor` | `Color` | `#E0E0E0` | Color of the guide lines |
| `SelectedBackgroundColor` | `Color` | `#E3F2FD` | Background tint of selected rows |
| `RowBackgroundColor` | `Color` | `Transparent` | Background of unselected rows |

### Drag/drop
| Property | Type | Default | Description |
|---|---|---|---|
| `EnableDragDrop` | `bool` | `false` | Adds drag source + drop target gestures to each row. Drops onto descendants are rejected automatically. |

### Events + Commands (MAUI)
| Event | Command | Args |
|---|---|---|
| `ItemSelected` | `ItemSelectedCommand` | `TreeItemEventArgs` (`Node`, `Item`) |
| `ItemExpanded` | `ItemExpandedCommand` | `TreeItemEventArgs` |
| `ItemCollapsed` | `ItemCollapsedCommand` | `TreeItemEventArgs` |
| `LoadFailed` | `LoadFailedCommand` | `TreeLoadFailedEventArgs` (`Exception`) |
| `ItemDropped` | `ItemDroppedCommand` | `TreeItemDroppedEventArgs` (`Source`, `Target`, `Position`) |

### Public Methods
```csharp
Tree.ExpandAll();                  // sync; skips branches that need ChildrenLoader
await Tree.ExpandAllAsync();       // awaits ChildrenLoader for every node
Tree.CollapseAll();
Tree.Expand(item);
Tree.Collapse(item);
Tree.Refresh(item);                // drops cached children for this node
await Tree.ReloadAsync();          // re-runs RootLoader (or rebinds ItemsSource)
var node = Tree.FindNode(item);    // locate the wrapper node for any source item
```

## Quick Start (MAUI)

```xml
<shiny:TreeView x:Name="Tree"
                IndentSize="22"
                ShowGuideLines="True"
                SelectedBackgroundColor="#EDE9FE"
                ChevronColor="#7C3AED"
                SelectedItem="{Binding Selected, Mode=TwoWay}"
                ItemSelected="OnSelected"
                ItemExpanded="OnExpanded">

    <shiny:TreeView.ItemTemplate>
        <DataTemplate x:DataType="local:FileNode">
            <HorizontalStackLayout Spacing="8" VerticalOptions="Center">
                <Label Text="{Binding Icon}" FontSize="16" />
                <Label Text="{Binding Name}" VerticalTextAlignment="Center" />
            </HorizontalStackLayout>
        </DataTemplate>
    </shiny:TreeView.ItemTemplate>

</shiny:TreeView>
```

```csharp
public TreeViewPage()
{
    InitializeComponent();
    Tree.ItemsSource         = roots;
    Tree.ChildrenSelector    = item => (item is FileNode { LazyLoad: false } f) ? f.Children : null;
    Tree.HasChildrenSelector = item => item is FileNode { IsFolder: true };
    Tree.CanSelectSelector   = item => item is FileNode f && !f.IsLocked;
    Tree.CanExpandSelector   = item => item is FileNode f && f.IsFolder;
    Tree.ChildrenLoader      = LoadRemoteChildrenAsync;   // covers the LazyLoad=true branch
}
```

> **Delegates aren't bindable from XAML** (they're `Func<T>`, not `BindableProperty` value types). Wire them in code-behind, in a custom markup extension, or in the page constructor.

## Lazy Loading

While `ChildrenLoader` is running, the chevron is replaced with an `ActivityIndicator`. On success, children are cached on the node — future expand/collapse uses the cache. To re-run the loader, call `Tree.Refresh(item)`.

If the loader throws:
- `LoadFailed` event fires (with the `Exception`)
- The chevron is replaced with `RetryIcon` (or ↻)
- Tapping the chevron retries the load

```csharp
Tree.ChildrenLoader = async item =>
{
    var children = await myService.GetChildrenAsync(item);
    return children;
};

Tree.LoadFailed += (s, e) =>
    StatusLabel.Text = $"Failed: {e.Exception.Message}";
```

### Lazy Root

```csharp
Tree.RootLoader = async () => await myService.GetTopLevelAsync();
```

A centered spinner covers the whole tree until the loader resolves. On failure, the tree shows a tap-to-retry message.

## CanExpand vs HasChildren

- **`HasChildrenSelector`** — returns false for **leaves** (no chevron rendered at all)
- **`CanExpandSelector`** — returns false for **gated branches** (chevron rendered but dimmed; tap does nothing)

Use both together to distinguish "this is a file" (no chevron) from "this folder is locked, you can see it but can't open it" (dimmed chevron).

## Selection

```xml
<!-- Single -->
<shiny:TreeView SelectionMode="Single"
                SelectedItem="{Binding Selected, Mode=TwoWay}" />

<!-- Multi -->
<shiny:TreeView SelectionMode="Multiple"
                SelectedItems="{Binding Selected}" />
```

`SelectedItem` is two-way bindable — setting it from the VM selects the corresponding node. `CanSelectSelector` prevents specific rows from firing selection (e.g. category headers, locked items). The row still renders, it just won't highlight or fire `ItemSelected`.

## Configurable Icons

```xml
<shiny:TreeView ChevronColor="#7C3AED" ChevronSize="14">
    <shiny:TreeView.ExpandedIcon>
        <FontImageSource Glyph="&#xF078;" FontFamily="FontAwesome" Color="#7C3AED" />
    </shiny:TreeView.ExpandedIcon>
    <shiny:TreeView.CollapsedIcon>
        <FontImageSource Glyph="&#xF054;" FontFamily="FontAwesome" Color="#7C3AED" />
    </shiny:TreeView.CollapsedIcon>
    <shiny:TreeView.RetryIcon>
        <FontImageSource Glyph="&#xF2F1;" FontFamily="FontAwesome" Color="#DC2626" />
    </shiny:TreeView.RetryIcon>
</shiny:TreeView>
```

Without icons set, the control renders ▼ / ▶ / ↻ glyphs colored by `ChevronColor`.

## Drag & Drop Reorder

The TreeView never touches your data — your handler does the move:

```csharp
void OnItemDropped(object? sender, TreeItemDroppedEventArgs e)
{
    var src = (FileNode)e.SourceItem;
    var tgt = (FileNode)e.TargetItem;

    var srcList = FindParentList(src);
    var tgtList = FindParentList(tgt);
    srcList.Remove(src);
    tgtList.Insert(tgtList.IndexOf(tgt) + 1, src);

    // Re-flatten the tree with the new order
    Tree.ItemsSource = null;
    Tree.ItemsSource = data;
}
```

Drops onto descendants of the source are rejected automatically (no cycles).

## Blazor

```razor
<TreeView TItem="FileNode"
          @ref="tree"
          ItemsSource="rootItems"
          ChildrenSelector="@(n => n.IsFolder ? n.Children : null)"
          ChildrenLoader="LoadRemoteAsync"
          HasChildrenSelector="@(n => n.IsFolder)"
          CanSelectSelector="@(n => !n.IsLocked)"
          SelectionMode="BlazorTreeSelectionMode.Single"
          SelectedItem="selected"
          SelectedItemChanged="v => selected = v"
          EnableDragDrop="true"
          ShowGuideLines="true"
          IndentSize="22"
          ChevronColor="#7C3AED"
          ItemExpanded="e => status = $\"Expanded {e.Item.Name}\""
          LoadFailed="e => status = $\"Failed: {e.Exception.Message}\""
          ItemDropped="OnDropped">

    <ExpandedIcon>
        <i class="fa-solid fa-chevron-down"></i>
    </ExpandedIcon>
    <CollapsedIcon>
        <i class="fa-solid fa-chevron-right"></i>
    </CollapsedIcon>
    <ItemTemplate Context="node">
        <span>@node.Icon</span>
        <span>@node.Name</span>
    </ItemTemplate>
</TreeView>
```

### Blazor differences

| MAUI | Blazor |
|---|---|
| `ExpandedIcon`/`CollapsedIcon`/`RetryIcon` are `ImageSource` | They're `RenderFragment` named slots (`<ExpandedIcon>…</ExpandedIcon>`) |
| `LoadingTemplate` not needed (uses `ActivityIndicator`) | `<LoadingTemplate>` slot lets you render a custom spinner |
| Colors are `Color` | `ChevronColor` is a CSS color string (e.g. `"#7C3AED"`) |
| `ICommand` mirrors for every event | `EventCallback<T>` only — no separate command properties |
| No built-in keyboard nav | Built-in: ↑/↓ navigate, ←/→ collapse/expand, Enter/Space select, Home/End jump to ends |
| `CssClass` parameter does not exist | `CssClass` parameter adds a class to the root `<div>` |

### Public methods (Blazor)
```csharp
await tree.ExpandAsync(item);
await tree.CollapseAsync(item);
await tree.ExpandAllAsync();
tree.CollapseAll();
await tree.RefreshAsync(item);
await tree.ReloadAsync();
```

## When to Use TreeView

- File browsers, folder pickers, category browsers
- Org charts, account/permission hierarchies
- Comment threads, conversation trees
- Anywhere users need to drill into nested data with lazy loading

## When NOT to Use TreeView

- Flat lists — use `CollectionView` (MAUI) or `VirtualizedGrid`
- Settings-style screens — use `TableView`
- Fully-loaded simple hierarchies (e.g. a 3-level menu with 20 items total) — a manual nested `VerticalStackLayout` may be simpler

## Best Practices

1. **Set `HasChildrenSelector` for leaves** — distinguishes "this is a file" from "this folder is empty". Without it, every node renders a chevron.
2. **Combine `ChildrenSelector` and `ChildrenLoader`** for mixed trees — selector returns synchronous branches, loader handles the lazy ones.
3. **Always handle `LoadFailed`** — without it, a transient failure leaves the user with a retry icon and no context.
4. **Don't mutate `ItemsSource` from inside `ItemDropped`** without re-binding — the TreeView caches children on `TreeNode` wrappers; rebind `ItemsSource = null; ItemsSource = data;` after a structural change, or call `Tree.Refresh(parent)` for a single subtree.
5. **Use `Refresh(item)` not `Collapse(item) + Expand(item)`** to force a reload — collapse just hides; refresh drops the cache.
6. **Keep templates lightweight** — every visible node is a separate `TreeNodeView` and an instance of your `ItemTemplate`. For very large trees, prefer lazy loading over loading all children up-front.
7. **Use `CanSelectSelector` not `IsEnabled` for non-selectable rows** — `CanSelectSelector` keeps them visually present (just non-interactive) and integrates with the keyboard navigation on Blazor.
