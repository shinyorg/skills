# Slide Editor (pptx editing)

Two controls on both hosts, over the same packages as the viewers:

| Control | What it is |
|---|---|
| `SlideEditor` | the lone editing surface — canvas, selection, caret, typing. No chrome. |
| `SlideEditorView` | `SlideEditor` plus an editing toolbar and a status line |

Same two constraints as everything else in these packages: **MAUI needs `UseSkiaSharp()`**, **Blazor is
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
c.AddTextBox(slideX, slideY);
c.DeleteSelectedShape();
c.Undo();
```

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
- Editing **table cells** and **grouped** shapes (rendered, not selectable).
- Adding, removing or reordering **slides**.
- **Rotation** handles — a rotated shape renders rotated and can be moved/resized, but the drag works
  in unrotated slide coordinates.
- Everything the viewer does not render — see `document-viewer.md`.

## Saving

An unedited deck saves **byte-identical**. After an edit, the parts the reader materialised (every
slide, layout, master, theme and notes part) are re-serialised by the SDK's only public flush — nothing
is lost and nothing is added or removed, but their bytes move.
