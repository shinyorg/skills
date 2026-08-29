# ShinyToolbar & ShinyTabBar (Blazor only)

Two screen-docked chromes in `Shiny.Blazor.Controls`. `ShinyToolbar` is an action bar docked to the top or
bottom of its scroll container; `ShinyTabBar` is a mobile-style tab bar pinned to the bottom of the viewport.
There is **no MAUI equivalent** — never emit `shiny:ShinyToolbar` in XAML.

Both are plain Razor components: no service registration, no `Use…()` call.

## ShinyToolbar basics

```razor
@using Shiny.Blazor.Controls

<ShinyToolbar Dock="ToolbarDock.Top"
              Title="Inbox"
              Frosted="true"
              Items="@items"
              ItemClicked="OnItemClicked" />

@code {
    List<ToolbarItem> items = new()
    {
        new() { Icon = "<svg viewBox='0 0 24 24'>…</svg>", Text = "Search" },
        new() { Icon = "<svg viewBox='0 0 24 24'>…</svg>", Text = "Alerts", Badge = "3" },
        new() { Icon = "/icons/compose.png", Text = "Compose", Href = "/compose" }
    };

    void OnItemClicked(ToolbarItem item) { /* item.Tag / item.Text identifies it */ }
}
```

`Icon` takes inline SVG/HTML, an emoji/glyph, or an image URL (`.png`/`.svg`/`http…`/`/…`). An item with an
`Href` renders as a link; everything else is a button.

## Overflow — items that don't fit become a dropdown

On by default. A `ResizeObserver` measures each item's intrinsic width against the room the bar actually has
and hands the leftovers to a "more" dropdown; widen the bar and they pop back out. Never hand-roll this.

```razor
<ShinyToolbar Title="Editor"
              Items="@items"
              OverflowEnabled="true"          @* default *@
              OverflowIcon="@myHamburgerSvg"  @* default: hamburger *@
              OverflowText="More"             @* shown under the button when ShowItemLabels is on *@
              MenuBackgroundColor="#1F2937"
              MenuTextColor="#F9FAFB"
              ItemClicked="OnItemClicked" />
```

Only `Items` collapse. `EndContent` sits **beside** them, pinned, and never collapses — so put chrome that
must always be visible (a status pill, an avatar) in `EndContent` and everything else in `Items`. Content in
`EndContent` cannot fold into the menu, which is the usual reason a bar "refuses to collapse": the actions
were markup rather than `Items`.

## Dropdown buttons & submenus

Give a `ToolbarItem` `Children` and it becomes a menu button: it opens a dropdown instead of raising
`ItemClicked`, and it grows a caret (`DropdownIcon`). Children can have `Children` of their own, which fly
out as submenus, as deep as you like. `IsSeparator = true` draws a divider between groups.

```razor
<ShinyToolbar Title="Document" Items="@items" ItemClicked="OnItemClicked" />

@code {
    List<ToolbarItem> items = new()
    {
        new()
        {
            Icon = FileSvg,
            Text = "File",
            Children = new()
            {
                new() { Text = "New" },
                new() { Text = "Open recent", Children = new()
                    {
                        new() { Text = "Roadmap.md" },
                        new() { Text = "Notes.md" }
                    }
                },
                new() { IsSeparator = true },
                new() { Text = "Delete", IconColor = "#EF4444" }
            }
        },
        new() { Icon = SearchSvg, Text = "Search" }
    };
}
```

Rules worth knowing:

- `ItemClicked` fires for the **leaf** that was invoked, never for a parent that only opens a menu. Identify
  it with `Tag` — a leaf's `Text` is not unique across submenus.
- A menu button that lands in the overflow dropdown keeps its children: it renders as a submenu row there.
- A separator on the bar itself is skipped; it only means something inside a dropdown.
- `IsDisabled` on a parent stops it opening at all.
- Panels are drawn in the browser's **top layer** (the popover API), so a toolbar inside a card, an
  `AppLayoutPanel` or any `overflow: hidden` scroller is **not** clipped by that ancestor. Menus reposition
  when the surrounding scroller moves and close on an outside click, on `Escape`, or after a selection.

## ShinyTabBar

```razor
<ShinyTabBar Items="@tabs" @bind-SelectedKey="selectedKey" ActiveColor="#7C3AED" Frosted="true" />

@code {
    string? selectedKey = "home";

    List<TabBarItem> tabs = new()
    {
        new() { Key = "home", Label = "Home", Icon = "<svg…>", ActiveIcon = "<svg…filled…>" },
        new() { Key = "chat", Label = "Chat", Icon = "<svg…>", Badge = "5" },
        new() { Key = "me",   Label = "Profile", Icon = "<svg…>", Href = "/profile" }
    };
}
```

`Badge = ""` renders a dot rather than a count. `ShinyTabBar` is `position: fixed` by default
(`Fixed="false"` makes it sticky inside a container).

## Placement

- `ShinyToolbar` defaults to `Sticky="true"` (`position: sticky`) — pass `Sticky="false"` when the shell
  already pins it, e.g. inside `AppLayoutHeader` or a panel's `HeaderContent`.
- `position: sticky` sticks against the nearest scroll container, and an ancestor with `overflow: hidden`
  silently breaks the *stickiness* (use `overflow: clip` if you must clip). This no longer affects the
  dropdowns, which live in the top layer.

## Key parameters

`ShinyToolbar`: `Dock`, `Sticky`, `Title`, `Items`, `StartContent`/`ChildContent`/`EndContent`,
`OverflowEnabled`, `OverflowIcon`, `OverflowText`, `OverflowAriaLabel`, `DropdownIcon`,
`MenuBackgroundColor`, `MenuTextColor`, `BackgroundColor`, `TextColor`, `Height`, `IconSize`,
`ShowItemLabels`, `Frosted`, `BlurRadius`, `TintColor`, `HasShadow`, `BorderColor`, `BorderThickness`,
`SafeArea`, `ZIndex`, `CssClass`, `Style`, `ItemClicked`.

`ToolbarItem`: `Icon`, `Text`, `Tooltip`, `Href`, `Target`, `Badge`, `IconColor`, `IsDisabled`, `Children`,
`IsSeparator`, `Tag`.

`ShinyTabBar`: `Items`, `SelectedKey` (`@bind-SelectedKey`), `Dock`, `Fixed`, `BackgroundColor`,
`ActiveColor`, `InactiveColor`, `ShowLabels`, `Height`, `IconSize`, `Frosted`, `BlurRadius`, `TintColor`,
`HasShadow`, `BorderColor`, `BorderThickness`, `SafeArea`, `ZIndex`, `CssClass`, `Style`, `ItemClicked`.

`TabBarItem`: `Key`, `Icon`, `ActiveIcon`, `Label`, `Href`, `Badge`, `IsDisabled`, `Tag`.

### Dark mode

Leave `BackgroundColor`, `TextColor`, `MenuBackgroundColor`, `MenuTextColor`, `TintColor`,
`ActiveColor` and `InactiveColor` unset — they default to theme tokens and follow the scheme. They
emit as inline styles, so anything you pass pins permanently; if you pass a background, pass a
`TextColor` with it.
