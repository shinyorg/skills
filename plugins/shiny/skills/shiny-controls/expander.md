# Expander & Accordion

A header you tap and content that animates in beneath — or above — it. `Accordion` stacks expanders and
decides how many may be open at once. Both controls ship in the core packages on **both hosts**:
`Shiny.Maui.Controls` and `Shiny.Blazor.Controls`.

Use it for FAQ lists, settings groups, filter panels, "show advanced options", or any disclosure where
a `IsVisible` / `@if` toggle would otherwise appear.

## Motion

`Animation` is a **flags** enum — `None`, `Fade`, `Slide`, `Height` — so the effects combine. The
default is `Height | Fade`.

- **`Height`** grows and shrinks the panel between zero and the content's size, so everything below moves
  with the reveal. This is what makes an accordion read as an accordion.
- **`Fade`** cross-fades the content.
- **`Slide`** translates the content in from the edge named by `SlideFrom`: `Top`, `Bottom`, `Left`, `Right`.

`AnimationDuration` (ms, default 250) and `AnimationEasing` set the pace; a duration of `0` or
`Animation="None"` snaps. `ExpandDirection` is `Down` (header on top, the default) or `Up` (content above
the header — what a panel pinned to the bottom of a page wants).

**MAUI**: `Height` measures the content and animates a clipped panel. On the very first reveal there may
be nothing measurable yet; when that happens `Height` stands down for that one transition and fade/slide
carry it. **Blazor**: the reveal is pure CSS — the panel is a grid transitioning `grid-template-rows`
between `0fr` and `1fr` — so there is no measuring, no JS interop, and content that changes size while
open still lays out normally. Blazor also honours `prefers-reduced-motion`.

## Header, content and indicator

| Concern | MAUI | Blazor |
| --- | --- | --- |
| Built-in header | `HeaderText` + `HeaderDetail` | same |
| Custom header | `Header` (View) or `HeaderTemplate` (DataTemplate) | `<Header>` render fragment |
| Content | `Content` (the `ContentProperty`) or `ContentTemplate` | `<ChildContent>` / the tag body |
| Lazy content | `LoadContentOnDemand="True"` | same |
| Custom indicator | `IndicatorView` (View) | `<IndicatorContent>` |

`IndicatorMode` is `Rotate` (one glyph, turned a quarter turn — the default), `Swap` (between
`CollapsedIcon` and `ExpandedIcon`) or `None`. `IndicatorPosition` is `End` or `Start`. The default glyphs
are `▶` and `▼` — written `"\u25B6\uFE0E"` / `"\u25BC\uFE0E"`, because without the U+FE0E
text-presentation selector iOS and WebKit draw U+25B6 as the glossy blue play-button **emoji**. Keep that
selector on any replacement glyph that has an emoji form.

## Chrome

`BorderColor`, `BorderThickness`, `CornerRadius`, `HasShadow`, `HeaderBackgroundColor`,
`HeaderTextColor`, `HeaderDetailColor`, `HeaderFontSize`, `HeaderFontFamily`, `HeaderFontAttributes`,
`HeaderPadding`, `HeaderHeight`, `ContentBackgroundColor`, `ContentPadding`, `IndicatorColor`,
`IndicatorSize`, `ShowSeparator`, `SeparatorColor`.

Leave any of them unset and it follows the active Shiny theme. On Blazor the same properties are CSS
strings (`BorderThickness="2px"`, `HeaderBackground="#EDE9FE"`) and the fills are named `HeaderBackground`
and `ContentBackground` rather than `…BackgroundColor`.

## State

`IsExpanded` is two-way. `Expand()`, `Collapse()` and `Toggle()` drive it from code (`ExpandAsync`,
`CollapseAsync`, `ToggleAsync` on Blazor).

MAUI raises `Expanding` and `Collapsing` **before** the change, both cancelable via `e.Cancel`, then
`Expanded` / `Collapsed` / `ExpandedChanged` and the `ExpandedChangedCommand`. Blazor raises
`IsExpandedChanged`, `OnExpanded` and `OnCollapsed`.

`IsToggleEnabled="False"` stops the header responding to taps without disabling the control.
`CanCollapse="False"` lets an open expander refuse to close on a tap — `Accordion` sets it for you.

## MAUI

```xml
<shiny:Expander HeaderText="Shipping"
                HeaderDetail="Arrives Tuesday"
                Animation="Height,Slide,Fade"
                SlideFrom="Top"
                AnimationDuration="250"
                BorderColor="#7C3AED"
                BorderThickness="2"
                CornerRadius="18">
    <VerticalStackLayout Spacing="4">
        <Label Text="123 Fake Street" />
        <Label Text="Springfield" />
    </VerticalStackLayout>
</shiny:Expander>
```

A custom header, and content built on first open:

```xml
<shiny:Expander LoadContentOnDemand="True">
    <shiny:Expander.Header>
        <HorizontalStackLayout Spacing="10">
            <shiny:PillView Text="LIVE" Type="Critical" />
            <Label Text="Custom header" FontAttributes="Bold" VerticalOptions="Center" />
        </HorizontalStackLayout>
    </shiny:Expander.Header>
    <shiny:Expander.ContentTemplate>
        <DataTemplate>
            <local:ExpensiveReportView />
        </DataTemplate>
    </shiny:Expander.ContentTemplate>
</shiny:Expander>
```

`Expander` derives from `Grid`, and `Content` is its `ContentProperty` — write the content between the
tags. Note `Type`, not `PillType`, on `PillView`.

## Blazor

```razor
<Expander HeaderText="Shipping"
          HeaderDetail="Arrives Tuesday"
          Animation="ExpanderAnimation.Height | ExpanderAnimation.Slide | ExpanderAnimation.Fade"
          SlideFrom="ExpanderSlideFrom.Top"
          AnimationDuration="250"
          BorderColor="#7C3AED"
          BorderThickness="2px"
          CornerRadius="18px">
    <p>123 Fake Street</p>
</Expander>
```

With a custom header, both slots must be named:

```razor
<Expander>
    <Header>
        <strong>Custom header</strong>
    </Header>
    <ChildContent>
        <p>Body.</p>
    </ChildContent>
</Expander>
```

The header is a `role="button"` div carrying `aria-expanded` and `aria-controls`, answering Enter and
Space; the collapsed panel is `inert` and `aria-hidden`, so nothing inside it takes focus.

## Accordion

`SelectionMode` is `Single` (opening one closes the rest — the default) or `Multiple`.
`AllowCollapseAll="False"` refuses to end up with nothing open: the last open item stops responding to
taps, and a list that starts closed opens its first item.

`ExpandedIndex` is two-way; `ExpandedIndexes` reports every open item. `ExpandAll()`, `CollapseAll()` and
`ExpandItem(index)` drive it from code. MAUI raises `ItemExpanded` / `ItemCollapsed` /
`ItemExpandedChanged` with the item, its data, its index and the new state, plus an
`ItemExpandedCommand`; Blazor raises `OnItemExpanded` / `OnItemCollapsed`. Items closed *because*
another opened do not raise events of their own.

The accordion's motion and chrome properties — `Animation`, `SlideFrom`, `AnimationDuration`,
`AnimationEasing`, `ExpandDirection`, `IndicatorMode`, `IndicatorPosition`, `BorderColor`,
`BorderThickness`, `CornerRadius`, `HeaderBackgroundColor`, `ContentBackgroundColor`,
`LoadContentOnDemand` — are **defaults**. They reach every item that did not set the same property
itself, so one odd expander in the list stays odd. MAUI additionally takes an `ItemStyle`
(`TargetType="shiny:Expander"`) for anything the shortcuts do not cover.

```xml
<shiny:Accordion SelectionMode="Single"
                 AllowCollapseAll="False"
                 ExpandedIndex="{Binding ExpandedIndex}"
                 Animation="Height,Fade"
                 CornerRadius="14">
    <shiny:Expander HeaderText="Account" HeaderDetail="Name, email, password">
        <Label Text="Everything about who you are." />
    </shiny:Expander>
    <shiny:Expander HeaderText="Billing" HeaderDetail="Cards and invoices">
        <Label Text="Everything about who pays." />
    </shiny:Expander>
</shiny:Accordion>
```

Data-driven on MAUI — bind `ItemsSource` with a `HeaderTemplate` and `ContentTemplate` (or an
`ItemTemplate` returning a whole `Expander` when an item needs to set expander properties of its own).
Generated items are appended after anything declared in markup, so the two can be mixed.

```xml
<shiny:Accordion ItemsSource="{Binding Faqs}" LoadContentOnDemand="True">
    <shiny:Accordion.HeaderTemplate>
        <DataTemplate x:DataType="local:FaqItem">
            <Label Text="{Binding Question}" FontAttributes="Bold" />
        </DataTemplate>
    </shiny:Accordion.HeaderTemplate>
    <shiny:Accordion.ContentTemplate>
        <DataTemplate x:DataType="local:FaqItem">
            <Label Text="{Binding Answer}" />
        </DataTemplate>
    </shiny:Accordion.ContentTemplate>
</shiny:Accordion>
```

Data-driven on Blazor — a plain `@foreach` of expanders is the natural form, and the models stay strongly
typed. There is also an `Items` + `ItemHeader` / `ItemContent` trio for when the shape is only known at
runtime; set `Item` on an expander so `OnItemExpanded` can hand the model back.

```razor
<Accordion SelectionMode="AccordionSelectionMode.Single"
           @bind-ExpandedIndex="index"
           LoadContentOnDemand="true"
           OnItemExpanded="@(e => Log(e.Data))">
    @foreach (var faq in faqs)
    {
        <Expander @key="faq" Item="faq" HeaderText="@faq.Question" HeaderDetail="@faq.Category">
            <p>@faq.Answer</p>
        </Expander>
    }
</Accordion>
```

## Gotchas

- `Animation` is flags, not a single value. `Animation="Slide"` alone gives you a slide with **no** height
  change, so the content pops the layout open and then slides within it. `Height,Slide` is usually what
  is wanted.
- `SlideFrom` does nothing unless the `Slide` flag is on.
- On MAUI `Expander` derives from `Grid`, so `BackgroundColor` paints *behind* the border. Use
  `HeaderBackgroundColor` and `ContentBackgroundColor` for the fills you can see.
- `CanCollapse` guards taps only; setting `IsExpanded` in code still wins. That is what lets an accordion
  move "the one that must stay open" to a different item.
- An `Accordion` default only reaches an item that never set that property itself — including via a style
  setter on the item.
- Setting `AutomationId` on a MAUI `Expander` also names its header row `<AutomationId>_Header` (the
  constant is `Expander.HeaderAutomationIdSuffix`). The tap gesture lives on that row, so UI automation
  driving the expander's own id finds nothing to tap — target the header id instead.
