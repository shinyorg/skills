# Layout (Blazor only)

Layout primitives and an application shell for `Shiny.Blazor.Controls`. **There is no MAUI equivalent and none is planned** — MAUI already ships `VerticalStackLayout`, `HorizontalStackLayout` and `Grid`, and its layout system is measure/arrange rather than CSS. If a user asks for a `VStack` on MAUI, use `VerticalStackLayout`; for a responsive grid on MAUI, use `Grid` with `OnIdiom`/`VisualStateManager`.

Everything lives in the root `Shiny.Blazor.Controls` namespace, so the usual `@using Shiny.Blazor.Controls` is all that is needed — no extra registration, no service, no JS reference to add (the `AppLayout` imports its own module).

## VStack / HStack

Flexbox stacks. All styling is emitted inline, so there is no stylesheet to load and nothing to override.

| Parameter | Type | Default | Notes |
| --- | --- | --- | --- |
| `Spacing` | `double` | `0` | Gap between children, in pixels |
| `Align` | `StackAlign` | `Stretch` | Cross axis — `Start`/`Center`/`End`/`Stretch`/`Baseline` |
| `Justify` | `StackJustify` | `Start` | Main axis — `Start`/`Center`/`End`/`SpaceBetween`/`SpaceAround`/`SpaceEvenly` |
| `Wrap` | `bool` | `false` | Wrap children onto more lines |
| `Reverse` | `bool` | `false` | `column-reverse` / `row-reverse` |
| `Grow` | `bool` | `false` | Fill the remaining space of a flex parent, and allow shrinking below content |
| `Scrollable` | `bool` | `false` | Scroll along the stack's main axis |
| `Padding` | `string?` | `null` | CSS shorthand; bare numbers are treated as px (`"16"` == `"16px"`, `"8 16"` == `"8px 16px"`) |
| `Background` | `string?` | `null` | CSS background shorthand |

```razor
<VStack Spacing="12" Padding="16">
    <h3>Profile</h3>

    <HStack Spacing="8" Justify="StackJustify.SpaceBetween" Align="StackAlign.Center">
        <span>Notifications</span>
        <input type="checkbox" @bind="notify" />
    </HStack>

    <HStack Spacing="8" Justify="StackJustify.End">
        <ShinyButton Text="Cancel" Appearance="ButtonAppearance.Text" />
        <ShinyButton Text="Save" />
    </HStack>
</VStack>
```

`class` and `style` passed by the consumer are **merged** with the component's own, not replaced — `<VStack Spacing="8" class="card" style="border-radius:12px">` keeps the flex styling.

## Grid / Row / Column

A responsive 12-column grid. `Grid` sets the column count and gutters, `Row` is a wrapping flex line, and `Column` takes a span per breakpoint.

Breakpoints (viewport width): `Xs` &lt; 576px · `Sm` ≥ 576 · `Md` ≥ 768 · `Lg` ≥ 992 · `Xl` ≥ 1200 · `Xxl` ≥ 1400.

**Spans cascade upwards.** A column with only `Md="6"` is full width below 768px and half from 768px up — the same rule as Bootstrap. Set a lower breakpoint explicitly to change what happens below.

### Grid

| Parameter | Type | Default |
| --- | --- | --- |
| `Columns` | `int` | `12` |
| `Gutter` | `double?` | — sets both axes, in px |
| `GutterX` / `GutterY` | `double` | `16` |
| `MaxWidth` | `double?` | `null` — caps the width and centres the grid |
| `Padding` | `string?` | `null` |

### Row

`Columns`, `Gutter`, `GutterX`, `GutterY` (all nullable overrides of the `Grid`), plus `Justify`, `Align` and `NoWrap`. A `Row` works without a `Grid` ancestor — it falls back to 12 columns and a 16px gutter.

### Column

| Parameter | Type | Notes |
| --- | --- | --- |
| `Xs` `Sm` `Md` `Lg` `Xl` `Xxl` | `int?` | Span in grid columns |
| `OffsetXs` … `OffsetXxl` | `int?` | Empty columns to the left |
| `OrderXs` … `OrderXxl` | `int?` | Visual order — drop a sidebar below the content on phones |
| `Fit` | `bool` | Shrink to content instead of taking a share of the row |
| `Padding` | `string?` | Applied inside the column's gutter |

A `Column` with **no span at all** shares the row equally with its other span-less siblings.

```razor
<Grid Gutter="16" MaxWidth="1200">
    <Row>
        <Column Xs="12" Md="6" Lg="3">…</Column>
        <Column Xs="12" Md="6" Lg="3">…</Column>
        <Column Xs="12" Md="6" Lg="3">…</Column>
        <Column Xs="12" Md="6" Lg="3">…</Column>
    </Row>

    <Row>
        @* sidebar last on phones, first from md up *@
        <Column Md="4" OrderXs="2" OrderMd="1"><Sidebar /></Column>
        <Column Md="8" OrderXs="1" OrderMd="2"><Article /></Column>
    </Row>

    <Row>
        <Column Fit>Icon</Column>
        <Column>Takes the rest</Column>
    </Row>
</Grid>
```

Implementation note (relevant if you are editing the CSS): spans are **not** 72 generated classes. `Column` writes one CSS variable per breakpoint the consumer actually set, and `Column.razor.css` chains `var()` fallbacks (`--sc-lg` → `--sc-md` → `--sc-sm` → `--sc-xs` → full width) inside media queries. Adding a breakpoint means adding one media block and extending the fallback chain. The gutter is padding-based (like Bootstrap), not `gap`, so a 6+6 pair still adds up to exactly 100%.

## AppLayout

An application shell: header, footer, a left and a right panel, and the content between them.

Regions are positioned with **CSS grid areas**, so their order in the markup does not matter — write them in whatever order reads best.

```razor
<AppLayout Height="100dvh">
    <AppLayoutHeader Height="56" Padding="0 16">…</AppLayoutHeader>

    <AppLayoutPanel Side="PanelSide.Left"
                    @bind-State="navState"
                    @bind-Size="navWidth"
                    MinSize="180"
                    MaxSize="420"
                    ToolbarSize="56"
                    CollapseBelow="900"
                    PersistKey="nav">
        <HeaderContent><h4>Explorer</h4></HeaderContent>
        <ToolbarContent>
            <VStack Spacing="4" Align="StackAlign.Center" Padding="8 0">
                <button title="Files">🗂</button>
                <button title="Search">🔍</button>
            </VStack>
        </ToolbarContent>
        <ChildContent>
            @* scrolls on its own *@
        </ChildContent>
        <FooterContent>…</FooterContent>
    </AppLayoutPanel>

    <AppLayoutContent Padding="20">@Body</AppLayoutContent>

    <AppLayoutPanel Side="PanelSide.Right" @bind-State="inspectorState" Padding="16">…</AppLayoutPanel>

    <AppLayoutFooter Height="36">Ready</AppLayoutFooter>
</AppLayout>
```

### AppLayout

| Parameter | Type | Default | Notes |
| --- | --- | --- | --- |
| `Height` | `string` | `"100%"` | Use `100dvh` for a full-page shell. The shell needs a definite height — inside a plain `<div>` with no height, `100%` collapses |
| `HeaderSpan` | `LayoutSpan` | `Full` | `Full` = header runs edge to edge; `Content` = it is inset between the panels, which then run the full height |
| `FooterSpan` | `LayoutSpan` | `Full` | Same for the footer |
| `BorderWidth` | `double` | `1` | Default divider width for every region |
| `BorderColor` | `string?` | theme outline | Default divider colour for every region |
| `Background` | `string?` | `null` | |

`HostWidth` (read-only) is the last measured width of the shell, kept current by a `ResizeObserver`.

### AppLayoutHeader / AppLayoutFooter

`Height` (px, null = size to content), `Border` / `BorderWidth` / `BorderColor`, `Background`, `Padding`. The header draws its divider on the bottom, the footer on the top. They do not scroll.

### AppLayoutContent

`Scrollable` (default `true`), `Background`, `Padding`. No border parameters — the header, footer and panels each draw the divider on their own edge.

### AppLayoutPanel

| Parameter | Type | Default | Notes |
| --- | --- | --- | --- |
| `Side` | `PanelSide` | `Left` | `Left` / `Right` |
| `State` | `PanelState` | `Shown` | `Hidden` / `Toolbar` / `Shown`. Two-way via `@bind-State` |
| `Size` | `double` | `260` | Expanded width in px. Two-way via `@bind-Size`; updated after a drag |
| `MinSize` / `MaxSize` | `double` | `140` / `640` | Drag clamps |
| `Resizable` | `bool` | `true` | Drag handle on the panel's inner edge |
| `ToolbarSize` | `double` | `56` | Rail width in `Toolbar` state |
| `CollapseBelow` | `double` | `0` (off) | Shell width under which the panel compacts |
| `CollapsedState` | `PanelState` | `Toolbar` | What an expanded panel collapses to |
| `PersistKey` | `string?` | `null` | Saves state + width to localStorage under `shiny.applayout.<key>` |
| `Scrollable` | `bool` | `true` | Body scroll region |
| `Border` / `BorderWidth` / `BorderColor` | | | Divider on the edge facing the content |
| `Background` / `Padding` | `string?` | | `Padding` applies to the **body**, so a pinned header/footer stays flush |

Slots: `HeaderContent` (pinned above the body), `ChildContent` (the scrolling body), `FooterContent` (pinned below), `ToolbarContent` (replaces the body in `Toolbar` state — put whatever you like in the rail; it is not restricted to icons).

Public members for driving a panel from code via `@ref`: `SetStateAsync(PanelState)`, `ToggleAsync()` (Shown ⇄ `CollapsedState`), and the read-only `CurrentState`, `CurrentSize`, `IsOverlay`.

### Behaviours worth knowing

- **Scroll regions.** The panel body, the toolbar rail and `AppLayoutContent` each scroll independently. The shell header/footer and a panel's `HeaderContent`/`FooterContent` never scroll. This is the point of the control — do not wrap the whole shell in a scrolling `<div>`.
- **Compacting** is measured against the **shell**, not the window, so an `AppLayout` nested in a card compacts when the card gets narrow. Below `CollapseBelow` an expanded panel drops to `CollapsedState`; expanding it while compact floats it over the content as a drawer with a scrim (click the scrim to dismiss). Widening back past the breakpoint restores the state it had before compacting. Compaction changes are **not** persisted — they are a response to the viewport, not a user preference.
- **Borders** resolve through CSS variables (`--shiny-layout-border-width` / `--shiny-layout-border-color`) set on the shell, so a layout-level default reaches every region and theme tokens still apply. Per-region values override; `Border="false"` removes that edge.
- **A `Hidden` panel keeps its element** (so the width animates) but is marked `inert` + `aria-hidden`, so nothing inside it is focusable or announced. It also drops its divider — a zero-width panel that still painted a border would leave a 1px sliver flush against the neighbouring region's border and read as one 2px line.
- **Aligning header buttons with the rail.** A header button that only has the header's padding to position it will not line up with the icons in a `Toolbar` rail below it. Put it in a slot exactly `ToolbarSize` wide and centre it there:

  ```razor
  <AppLayoutHeader Height="52" Padding="0" style="display:flex;align-items:center;">
      <div style="flex:0 0 auto;width:52px;display:flex;justify-content:center;">
          <button style="width:36px;height:36px;padding:0;" @onclick="ToggleNavAsync">☰</button>
      </div>
      <strong>Workspace</strong>
  </AppLayoutHeader>
  ```

  Drive the slot width and the panel's `ToolbarSize` from one constant so they cannot drift apart.
- **Putting a `ShinyToolbar` / `ShinyTabBar` in a region.** Both self-position by default and have to be told not to, because the shell's grid already pins them:
  - `ShinyToolbar` defaults to `Sticky="true"` (`position: sticky`) — pass `Sticky="false"` inside an `AppLayoutHeader` or a panel's `HeaderContent`.
  - `ShinyTabBar` defaults to `Fixed="true"` (`position: fixed`) — pass `Fixed="false"` inside an `AppLayoutFooter`, or it drops out of the grid's flow, leaves the footer row 0px tall and floats over the content.

  Colour them from theme tokens (`BackgroundColor="var(--shiny-color-surface)"`, `TextColor="var(--shiny-color-on-surface)"`) rather than literals, or the bar stays on a light palette when the app switches to dark. Prefer `surface` over the `surface-container-*` tones for an app bar — the container tones are a flat grey that reads as unfinished chrome.
- **Resizing** is done in JS (`app-layout.js`) writing `element.style.width` directly during the drag, and only telling .NET the final value on pointer-up — so a drag never round-trips through the renderer. An `is-resizing` class kills the width transition while dragging.

### Gotchas when editing this control

- `AppLayoutPanel.Layout` (the `[CascadingParameter]`) **must be public**. Blazor only matches cascading values onto public properties and silently skips non-public ones — a private one compiles, runs, and simply never collapses.
- The overlay drawer sets `grid-area: auto`. An absolutely positioned grid item that still has a definite grid position is sized against its (by then zero-width) grid area, not the shell.
- The resize handle lives **inside** the panel. The panel clips its overflow for the width transition, so a handle straddling the edge is cut in half.
