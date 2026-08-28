# Ribbon (Desktop + Blazor)

An Office-style tabbed command bar: a strip of tabs over a body of titled command groups. For
applications with more commands than a toolbar can hold — editors, IDEs, analysis tools.

| Package | Namespace | Use when |
|---|---|---|
| **`Shiny.Maui.Controls.Desktop`** | `Shiny.Maui.Controls.Desktop.Ribbons` | MAUI desktop — Windows, macOS (AppKit), Mac Catalyst, Linux |
| **`Shiny.Blazor.Controls`** | `Shiny.Blazor.Controls` | Blazor — **no add-on package**, the ribbon is in the main package (same split as docking) |

**No registration on either host.** It is markup, not a service — do not emit a `UseShinyRibbon()` or
`AddShinyRibbon()` call, there is no such thing.

**Do not use it on a phone.** It wants a pointer to hover with and enough width for three rows of small
commands. Generate `ShinyToolbar` / `ShinyTabBar` (Blazor) or `ShinyTabbedPage` (MAUI) for mobile
chrome — see toolbar-tabbar.md and tabbedpage.md.

## The shape

```
Ribbon
└── RibbonTab            one row of the strip
    └── RibbonGroup      a titled box of related commands
        └── items        buttons, toggles, split/menu buttons, separators, hosted content
```

Both hosts are **declarative** — nested elements in XAML, nested components in Razor. There is no
item-list / `ItemsSource` form; never invent one.

## MAUI

`xmlns:shiny="http://shiny.net/maui/controls"` — the ribbon is mapped onto the core namespace URI from
the Desktop assembly, so do **not** emit a `clr-namespace:` prefix for it.

```xml
<shiny:Ribbon ApplicationButtonText="File"
              ApplicationButtonCommand="{Binding OpenFileMenu}"
              DisplayMode="{Binding DisplayMode}"
              SelectedIndex="{Binding TabIndex}">

    <shiny:Ribbon.QuickAccessItems>
        <shiny:RibbonButton Text="Save" Size="Small" Icon="save.png" Command="{Binding Save}" />
        <shiny:RibbonButton Text="Undo" Size="Small" Icon="undo.png" Command="{Binding Undo}" />
    </shiny:Ribbon.QuickAccessItems>

    <shiny:RibbonTab Title="Home" Key="home">

        <shiny:RibbonGroup Title="Clipboard" Priority="30">
            <shiny:RibbonSplitButton Text="Paste" Icon="paste.png" Command="{Binding Paste}">
                <shiny:RibbonMenuEntry Text="Keep source formatting" Command="{Binding PasteKeep}" />
                <shiny:RibbonMenuEntry Text="Text only" Command="{Binding PasteText}" />
                <shiny:RibbonMenuEntry IsSeparator="True" />
                <shiny:RibbonMenuEntry Text="Chart">
                    <shiny:RibbonMenuEntry Text="Bar" Command="{Binding Chart}" CommandParameter="Bar" />
                    <shiny:RibbonMenuEntry Text="Line" Command="{Binding Chart}" CommandParameter="Line" />
                </shiny:RibbonMenuEntry>
            </shiny:RibbonSplitButton>

            <shiny:RibbonButton Text="Cut"  Size="Small" Icon="cut.png"  Command="{Binding Cut}" />
            <shiny:RibbonButton Text="Copy" Size="Small" Icon="copy.png" Command="{Binding Copy}" />
        </shiny:RibbonGroup>

        <shiny:RibbonGroup Title="Font" ShowDialogLauncher="True"
                           DialogLauncherCommand="{Binding OpenFontDialog}">
            <shiny:RibbonToggleButton Text="Bold"   Size="Small" Icon="bold.png"   IsChecked="{Binding Bold}" />
            <shiny:RibbonToggleButton Text="Italic" Size="Small" Icon="italic.png" IsChecked="{Binding Italic}" />

            <shiny:RibbonSeparator />

            <shiny:RibbonContentItem Size="Small">
                <shiny:FontSizePicker SelectedFontSize="{Binding FontSize}" WidthRequest="86" />
            </shiny:RibbonContentItem>
        </shiny:RibbonGroup>
    </shiny:RibbonTab>

    <!-- contextual: ContextTitle makes it contextual, IsVisible makes it come and go -->
    <shiny:RibbonTab Title="Format" Key="picture"
                     ContextTitle="Picture Tools"
                     IsVisible="{Binding PictureSelected}">
        <shiny:RibbonGroup Title="Adjust" CanCollapse="False">
            <shiny:RibbonButton Text="Crop" Icon="crop.png" Command="{Binding Crop}" />
        </shiny:RibbonGroup>
    </shiny:RibbonTab>
</shiny:Ribbon>
```

## Blazor

```razor
@using Shiny.Blazor.Controls

<Ribbon ApplicationButtonText="File"
        ApplicationButtonClicked="OpenFileMenu"
        @bind-DisplayMode="mode"
        @bind-SelectedKey="tab">

    <QuickAccess>
        <RibbonButton Size="RibbonItemSize.Small" Text="Save" Icon="@SaveSvg" Clicked="Save" />
    </QuickAccess>

    <ChildContent>
        <RibbonTab Title="Home" Key="home">
            <RibbonGroup Title="Clipboard" Priority="30">
                <RibbonSplitButton Text="Paste" Icon="@PasteSvg" Clicked="Paste" Menu="@pasteMenu" />
                <RibbonButton Size="RibbonItemSize.Small" Text="Cut" Icon="@CutSvg" Clicked="Cut" />
            </RibbonGroup>

            <RibbonGroup Title="Font" ShowDialogLauncher="true" DialogLauncherClicked="OpenFont">
                <RibbonToggleButton Size="RibbonItemSize.Small" Text="Bold" Icon="@BoldSvg" @bind-Checked="bold" />
                <RibbonSeparator />
                <RibbonContent Size="RibbonItemSize.Small">
                    <select @bind="fontSize">…</select>
                </RibbonContent>
            </RibbonGroup>
        </RibbonTab>

        <RibbonTab Title="Format" Key="picture" ContextTitle="Picture Tools" Visible="@pictureSelected">
            <RibbonGroup Title="Adjust" CanCollapse="false">
                <RibbonButton Text="Crop" Icon="@CropSvg" Clicked="Crop" />
            </RibbonGroup>
        </RibbonTab>
    </ChildContent>
</Ribbon>

@code {
    List<RibbonMenuEntry> pasteMenu = new()
    {
        new() { Text = "Keep source formatting", OnClick = EventCallback.Factory.Create(this, PasteKeep) },
        new() { IsSeparator = true },
        new() { Text = "Text only", OnClick = EventCallback.Factory.Create(this, PasteText) }
    };
}
```

**Rules that trip generation up:**

- `Size` is a **qualified enum** in Razor: `Size="RibbonItemSize.Small"`, never `Size="Small"`.
- Once you use `<QuickAccess>` you must also name `<ChildContent>` — Blazor's rule for a component
  with more than one `RenderFragment`.
- Dropdown entries are a **`List<RibbonMenuEntry>` parameter** on Blazor (`Menu="@list"`), not child
  components. On MAUI they are markup children. This is the same split `ToolbarItem.Children` uses.

## Columns fall out of the sizes

Nothing declares a column, so never try to. A `Large` item (the default) takes a column to itself —
icon over label — and `Small` items stack up to `SmallItemRows` (3) deep in a shared column. A
`RibbonSeparator` or a large item ends the column and starts a fresh one. Reordering the items in a
group re-flows it; that is the only lever.

Set `Size="Small"` on the secondary commands and leave the primary one large. Three smalls per column
is the shape to aim for.

## Item kinds

| Kind | Notes |
|---|---|
| `RibbonButton` | MAUI: `Command`/`CommandParameter` + `Clicked`. Blazor: `Clicked` |
| `RibbonToggleButton` | `IsChecked` (MAUI) / `Checked` (Blazor), **two-way by default** — bind it and skip the handler. A MAUI toggle's command receives the new bool when no `CommandParameter` is set |
| `RibbonSplitButton` | Face runs the default action, chevron opens the menu. Use when one choice is overwhelmingly the common one |
| `RibbonMenuButton` | Whole face opens the menu; no default action |
| `RibbonSeparator` | Full-height rule + a column break |
| `RibbonContentItem` (MAUI) / `RibbonContent` (Blazor) | Hosts any view/markup — a picker, a combo, a swatch strip |

Common item properties: `Text`, `Icon`, `Tooltip`, `Description`, `Size`, `AutomationId`, and
enabled/visible (`IsEnabled`/`IsVisible` on MAUI, `Disabled` on Blazor).

**Bind enabled, do not remove.** A command that disappears when it cannot run makes the bar move under
the pointer. A group dims its whole contents in one place: `IsEnabled` (MAUI) / `Disabled` (Blazor).

## Icons

- **MAUI** — `Icon` is an `ImageSource`: a PNG/SVG asset, or a `FontImageSource` glyph. For a *drawn*
  icon set `IconTemplate` to a `DataTemplate` returning a view (it wins over `Icon`, and is
  instantiated per button — never return a shared instance).
- **Blazor** — `Icon` is a string: inline SVG markup, an image URL, or a glyph. Same convention as
  `ToolbarItem.Icon`.

## Contextual tabs

Setting `ContextTitle` captions the coloured band above the strip and marks the tab contextual; binding
its visibility to what the tab is about is what makes it appear. When the showing tab stops being
selectable the ribbon falls back to the nearest one that still is (`RibbonTabChangeReason.Fallback`),
so a vanished selection never leaves an empty body. `ContextColor` overrides the tertiary accent.

## Display mode

`DisplayMode` is two-way on both hosts:

- `Expanded` — strip + open body (default)
- `Collapsed` — strip only; picking a tab peeks the body back, and the next command puts it away
- `Simplified` — one dense row, every item drawn small, group titles dropped

The trailing chevron and a second click on the showing tab both toggle Expanded ⇄ Collapsed.
`AllowCollapse="false"` removes both gestures (code can still set `DisplayMode`).

## Groups that do not fit

A tab wider than the bar folds groups into single buttons — lowest `Priority` first, rightmost breaking
ties. Raise `Priority` on the groups that should survive longest; `CanCollapse="false"` pins one open;
`AllowGroupCollapse="false"` turns it off and lets the body scroll instead. Set `CollapsedIcon` on
Blazor (MAUI falls back to the first item's icon).

## Selection and events

- MAUI: `SelectedIndex` / `SelectedTab` (both two-way), `SelectTab(key)`, `SelectTab(tab)`;
  `TabChanged`, `ItemInvoked`, `GroupDialogLauncherClicked`, `ApplicationButtonClicked`.
- Blazor: `@bind-SelectedKey`, `TabChanged`, `DisplayModeChanged`, `MenuEntrySelected`,
  `ApplicationButtonClicked`, plus each item's own callback.
- MAUI has **`ribbon.Invoke(item)`** — press an item from code so a keyboard shortcut and the button it
  duplicates go down one path. It is also the seam a test presses through.

## Theming

Follows the Shiny theme with no configuration on either host. `AccentColor`, `HeaderBackgroundColor`
and `BodyBackgroundColor` override those three (a `Color` on MAUI, a CSS colour string on Blazor);
everything else stays on the tokens. See styling.md.

## Platform notes

- Dropdowns are drawn above the page (Blazor: the browser's top layer via the popover API, with Escape
  and click-away closing; MAUI: the shared page overlay), so a body only three rows tall never clips a
  menu.
- **macOS AppKit (`net10.0-macos`)** gives no native view to a child added after layout. The ribbon
  builds every tab's body up front and switches with `IsVisible`, so tabs, collapsing and group folding
  all work there. Adding a tab/group/item *after* the fact rebuilds, and dropdown panels are added on
  demand — those two paths are the limit on that head.
- Key tips (Alt-key letter badges) are not implemented on either host.
