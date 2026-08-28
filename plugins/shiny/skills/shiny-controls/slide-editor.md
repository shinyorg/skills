# Slide Editor (pptx editing)

Two controls on both hosts, over the same packages as the viewers:

| Control | What it is |
|---|---|
| `SlideEditor` | the lone editing surface — canvas, selection, caret, typing. No chrome. |
| `SlideEditorView` | `SlideEditor` plus an editing toolbar and a status line |

Same two constraints as everything else in these packages: **MAUI needs `UseShinyOffice()`** (it registers SkiaSharp, plus the AppKit canvas on `net10.0-macos`), **Blazor is
WASM-only**, and on Blazor the container needs an **explicit height**.

## Open a deck for editing

```csharp
using var deck = await SlideDeck.OpenAsync("deck.pptx", editable: true);
```

Without `editable: true` the deck is read-only and `Execute` throws.

### Blazor

```razor
<div style="height:560px">
    <SlideEditorView Deck="deck" @bind-SlideIndex="index" DeckChanged="OnChanged" />
</div>
```

### MAUI

```xml
<office:SlideEditorView Deck="{Binding Deck}" SlideIndex="{Binding Index}" />
```

## Two gestures — this is the part to get right when generating code

**Shape mode**: a single click/tap selects a shape and draws a **dashed** frame with eight resize
handles. Drag the body to move, a handle to resize.

**Text mode**: a double-click/double-tap puts a caret inside that shape's text; the frame turns
**solid**. A single click inside then moves the caret; a click outside leaves text mode.

Do not expect typing to reach the document while a shape is merely selected — it is deliberately
dropped. `Controller.IsEditingText` is the flag; a toolbar's text-formatting buttons should be
disabled unless it is true.

## Only slide-owned shapes are editable

`SlideShape.IsEditable` is false for anything painted from the layout or master, and for shapes
flattened out of a group. Hit testing skips them. Don't write code that indexes into
`Slide.Shapes` assuming every entry can be selected.

## Driving it

```csharp
var c = editor.Controller!;

c.Select(index);                  // or let PointerDown do it
c.BeginTextEditing(x, y);         // caret into the selected shape's text
c.InsertText("Hello");
c.InsertParagraph();              // Enter — keeps level and bullet
c.Backspace();
c.ToggleBold();
c.SetFontSize(24);                // POINTS, not pixels
c.SetAlignment(TextAlignment.Center);
c.ShiftLevel(+1);                 // indent the bullet
c.SetHighlight(color);            // null clears it; ToggleHighlight(color) for a toolbar button
c.AddTextBox(slideX, slideY);
c.DeleteSelectedShape();
c.Undo();
```

## Adding shapes, pictures and tables

```csharp
c.AddShape(ShapeGeometry.Hexagon, slideX, slideY, width: 240, height: 180, fill: accent);
c.AddPicture(bytes, "image/png", slideX, slideY, width: 400);
c.AddTable(rows: 3, columns: 4, slideX, slideY, width: 480, height: 200);
```

All three place in **slide** coordinates and select what they added, so the next gesture is a drag of
the new object. `AddShape` writes a real drawn shape rather than a text box — no `TextBox` flag on the
non-visual properties — which is what makes PowerPoint give it the theme fill and treat it as a shape.
A table is a `p:graphicFrame`, not a shape, and comes out with a built-in table style applied.

`ShapeGeometry` lives in `Shiny.Controls.Office.Shapes` and is **shared with the document editor** —
the same twenty presets, the same path builder.

Both toolbars already offer these, over the same galleries as the Word side.

## Toolbar icons and tooltips

The slide toolbar draws from the **same `OfficeIcons` set** as the document toolbar — monochrome
stroked artwork on a 24x24 grid, one weight, no colour of its own, shared between MAUI and Blazor. The
slide-only marks are `Previous`, `Next`, `Indent`, `Outdent`, `TextBox` and `Delete`. No glyph, letter
or emoji goes on a toolbar button; add to the enum instead. `ShowToolbarTooltips` controls the hover
tooltips on those icon-only buttons — on for Blazor and for MAUI desktop, off on iOS and Android.

The full rules, and why they are rules, are in **document-editor.md** — one icon strategy, one place.

## Highlighting

`a:highlight` holds a real colour, so nothing is approximated. The pickers still offer
`HighlightPalette.Swatches` — the same sixteen swatches as the document editor, so one highlight
button behaves the same over both file types.

## Dropping files in

Dragging an image file onto a slide inserts it **centred on the drop point**, sized to at most half the
slide. On by default:

```razor
<SlideEditorView Deck="deck" AllowFileDrop="true" DropRejected="OnRejected" />
```

```csharp
Editor.DropRejected += (_, e) => Toast(e.Reason);   // MAUI
```

Where it works, and what is rejected, is identical to the document editor — see `document-editor.md`.

`CaretFormat.FontSize` is reported in **points** to match `SetFontSize`. The model itself carries
pixels — reporting those makes a size box read 24 for text the user set to 18.

## Coordinates

Two spaces, and only `ToSlide` / `ToViewport` cross between them:

- **slide** coordinates — what the model and the OOXML store
- **viewport** coordinates — what a pointer arrives in

`BoundsOf`, `SelectionBounds`, `SelectionHandles`, `CaretRect` and `TextSelectionRects` all return
viewport coordinates, ready to hand to the painter's `SlideEditorChrome`.

## Painting the chrome

Both hosts build a `SlideEditorChrome` and hand it to `SlidePainter` on the paint request. It is drawn
*outside* the slide's fit transform on purpose — inside it, a handle would scale with the slide and a
zoomed-out deck would have grab targets too small to hit.

## Keyboard

**Blazor**: complete, through `beforeinput` (so IME, dictation and paste work). Arrows, Home/End, Tab,
Escape, Delete and Ctrl/Cmd+B/I/U/Z are wired.

**MAUI**: typing works via a hidden `Entry`. **Physical keys do not** — MAUI has no portable key-down
event. Route them with `editor.HandleKey(EditorKey.Left, shift: true)` from a platform hook.

## Not implemented

- **Soft line breaks** (`a:br`) — read and rendered, but they contribute no characters to the offset
  space, so the caret cannot sit on one and Shift+Enter does not insert one.
- Editing **table cells** and **grouped** shapes (rendered, not selectable). A table can be *added*
  and moved or resized as a whole; typing into its cells cannot.
- Adding, removing or reordering **slides**.
- **Rotation** handles — a rotated shape renders rotated and can be moved/resized, but the drag works
  in unrotated slide coordinates.
- Everything the viewer does not render — see `document-viewer.md`.

## Saving

An unedited deck saves **byte-identical**. After an edit, the parts the reader materialised (every
slide, layout, master, theme and notes part) are re-serialised by the SDK's only public flush — nothing
is lost and nothing is added or removed, but their bytes move.
