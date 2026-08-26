# Document Editor (docx editing)

Two controls, on both hosts:

| Control | What it is |
|---|---|
| `DocumentEditor` | the lone editing surface — canvas, caret, selection, typing. No chrome. |
| `DocumentEditorView` | `DocumentEditor` plus a formatting toolbar |

Same packages as the viewers (`Shiny.Maui.Controls.Office` / `Shiny.Blazor.Controls.Office`), same two
constraints: **MAUI needs `UseShinyOffice()`** (it registers SkiaSharp, plus the AppKit canvas on `net10.0-macos`), **Blazor is WASM-only**, and on Blazor the container needs
an **explicit height**.

## Open a document for editing

`editable: true` is required — a read-only document throws on any edit.

```csharp
using var document = await WordDocument.OpenAsync("report.docx", editable: true);
```

### Blazor

```razor
<div style="height:520px">
    <DocumentEditorView Document="document" DocumentChanged="OnChanged" />
</div>

@* or the bare surface, with your own chrome: *@
<div style="height:520px">
    <DocumentEditor @ref="editor" Document="document" />
</div>
```

### MAUI

```xml
<office:DocumentEditorView x:Name="Editor" Document="{Binding Document}" />
<office:DocumentEditor x:Name="BareEditor" Document="{Binding Document}" />
```

## The toolbar is composed from what each host has

This is deliberate and asymmetric, because the two hosts own different halves of the chrome:

- **MAUI** has `FontPickerButton` / `FontSizePickerButton` but **no toolbar** — so `DocumentEditorView`
  builds a scrolling row of MAUI primitives and drops those two pickers into it.
- **Blazor** has `ShinyToolbar` but **no font picker** — so it composes `ShinyToolbar` and uses plain
  `<select>` elements for family and size.

The API and behaviour match on both; only the internals differ. Do not emit `shiny:ShinyToolbar` in
XAML, and do not expect a `FontPicker` component in Blazor.

## Driving it

Everything lives on the shared controller, identical on both hosts:

```csharp
var c = editor.Controller;          // DocumentEditorController

c.InsertText("hello");
c.InsertParagraph();                 // Enter
c.DeleteBackward();                  // Backspace
c.Move(CaretMove.WordRight, extend: true);
c.SelectAll();

c.ToggleBold(); c.ToggleItalic(); c.ToggleUnderline(); c.ToggleStrikethrough();
c.SetFontFamily("Cambria");
c.SetFontSize(14);                   // points
c.SetTextColor(new ArgbColor(255, 0xC0, 0, 0));
c.SetAlignment(TextAlignment.Center);

c.Undo(); c.Redo();
c.CaretFormat;                       // what a toolbar should show as active
c.Selection.Range;
```

Saving is the same as everywhere else — and an unedited document still saves byte-identical:

```csharp
await document.SaveAsAsync("edited.docx");
```

## Keyboard input

**Blazor**: complete. Typing goes through `beforeinput`, so IME composition, autocorrect, dictation and
paste all work. Arrows, Home/End, Ctrl/Cmd+B/I/U, Ctrl/Cmd+Z and Shift+Ctrl/Cmd+Z are wired.

**MAUI**: typing works — a hidden `Entry` gives the platform keyboard and IME somewhere to send text.
**Physical keys do not**, because MAUI exposes no portable key-down event. Route them yourself:

```csharp
Editor.HandleKey(EditorKey.Left, shift: true);
Editor.HandleKey(EditorKey.Undo, control: true);
```

A desktop host adds its own platform hook (`NSEvent` on macOS, `KeyDown` on Windows) and calls that.
Tapping, selection, typing and every toolbar command work without it.

## Spell check

Turned on by default, and on MAUI the checker is the **platform's own** — `UITextChecker` (iOS,
Mac Catalyst), `NSSpellChecker` (macOS AppKit), Android's text-services `SpellCheckerSession`, and the
Windows `ISpellChecker` COM API. Nothing has to be registered: referencing
`Shiny.Maui.Controls.Office` installs it. That matters because it is the *user's* dictionary — words
they taught the keyboard are already known, and **Add to dictionary** writes back to it.

**Blazor has no platform checker and defaults to none.** The browser spell-checks its own editable
elements and exposes neither the results nor the suggestions to script, and a canvas is not an
editable element anyway. Supply one:

```razor
<DocumentEditorView Document="document" SpellChecker="myChecker" SpellCheckEnabled="true" />
```

Misspellings get a red wavy underline; right-click (or long-press on touch) opens corrections plus
**Ignore** and **Add to dictionary**. Applying a correction is a single undo step.

### Supplying your own

Implement `ISpellChecker`, or derive from `SpellCheckerBase` which already handles the ignore list and
language defaulting — you write two methods:

```csharp
public sealed class MyChecker : SpellCheckerBase
{
    public override bool IsAvailable => true;

    protected override ValueTask<IReadOnlyList<SpellingError>> CheckCoreAsync(
        string text, string language, CancellationToken cancellationToken) => ...;

    protected override ValueTask<IReadOnlyList<string>> SuggestCoreAsync(
        string word, string language, CancellationToken cancellationToken) => ...;
}
```

Then either per control (`SpellChecker="..."` / `SpellChecker` bindable property) or globally:

```csharp
SpellCheckers.Default = new MyChecker();   // wins over the platform one
```

Set `SpellCheckers.Default` before the first editor is constructed. Registration uses
`SetDefaultIfUnset`, so an explicit choice is never overwritten.

`SpellingTokenizer` is public and worth reusing in a custom checker: it skips acronyms, camelCase,
numbers, URLs, email addresses and paths — the things every dictionary flags and no reader wants
underlined.

### Notes

- Checking is **per paragraph and cached on the paragraph's text**, and only the paragraphs on screen
  are checked. Scrolling re-checks nothing already seen; editing re-checks one paragraph.
- Calls are debounced (500 ms) — a platform checker is interop, and half-typed words are not errors.
- `IsAvailable` is false when there is no checker *or* no dictionary for the language. Check it before
  telling the user spelling is on.
- Set `SpellCheckEnabled`/`IsSpellCheckEnabled` to `false` to turn it off entirely.

## Not implemented

- **Formatting with an empty selection changes only `CaretFormat`, not the document.** Word applies it
  to whatever is typed next; that needs a pending-format concept the editor does not have yet.
- Editing tables, images, lists (their *text* edits fine; structure does not).
- Cut/copy/paste through the clipboard, find and replace.
- **Grammar** checking. Android reports grammar errors and they are deliberately ignored — only
  `LooksLikeTypo` is treated as an error, so the behaviour matches the other three platforms.
- Inserting new paragraph styles, images or tables.
- Everything the viewer does not render is still not rendered — see `document-viewer.md`.
