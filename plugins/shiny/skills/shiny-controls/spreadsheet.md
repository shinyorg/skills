# Spreadsheet (xlsx viewer & editor)

`SpreadsheetView` opens, renders and edits `.xlsx` workbooks on both MAUI and Blazor.

**Packages**

| Package | Host |
|---|---|
| `Shiny.Controls.Office.Shared` | the document kernel — no UI dependency |
| `Shiny.Controls.Office.Skia` | the shared SkiaSharp paint layer |
| `Shiny.Maui.Controls.Office` | `SpreadsheetView` for MAUI |
| `Shiny.Blazor.Controls.Office` | `<SpreadsheetView>` for Blazor **WebAssembly** |

## Two hard constraints

1. **Blazor WASM only.** The grid repaints on every keystroke; a Blazor Server round-trip per key makes
   editing unusable. Also, SkiaSharp on WASM forces native relinking, so consumers need the
   `wasm-tools` workload installed.
2. **MAUI needs `UseShinyOffice()`** in `MauiProgram`, or the canvas never renders. It calls
   `UseSkiaSharp()` for you and, on the macOS AppKit head (`net10.0-macos`), registers the Skia canvas
   SkiaSharp does not ship for that head — without it every Office control renders blank there.

```csharp
builder
    .UseMauiApp<App>()
    .UseShinyControls()
    .UseShinyOffice();        // required - registers SkiaSharp + the AppKit canvas
```

## Opening a workbook

```csharp
using var workbook = await Workbook.OpenAsync("/path/to/book.xlsx");
// or from a stream:
using var workbook = await Workbook.OpenAsync(stream);
// or start empty:
using var workbook = Workbook.Create("Sheet1");
```

`Workbook` is `IDisposable` and holds the package open. Dispose it when the page goes away.

### MAUI

```xml
<office:SpreadsheetView x:Name="Sheet"
                        Workbook="{Binding Workbook}"
                        SheetName="Budget"
                        CellChanged="OnCellChanged" />
```

### Blazor

```razor
<div style="height:420px">
    <SpreadsheetView Workbook="workbook"
                     Theme="SpreadsheetTheme.Dark"
                     CellChanged="OnCellChanged" />
</div>
```

The Blazor host paints to a canvas that fills its container, so **the container needs an explicit
height** — without one it collapses to zero and nothing appears.

## Editing

All edits go through the undo stack. Never mutate cells directly.

```csharp
workbook.Execute(new SetCellValueCommand("Budget", CellRef.Parse("B2"), CellValue.FromNumber(42)));
workbook.Execute(new SetCellFormulaCommand("Budget", CellRef.Parse("D2"), "B2*C2"));
workbook.Execute(new ClearRangeCommand("Budget", CellRange.Parse("A1:C3")));

workbook.Undo.Undo();
workbook.Undo.Redo();
```

`ClearRangeCommand` is one undo step for the whole range, not one per cell.

## Formulas

The calc engine indexes formulas lazily on the first edit or the first read of a calculated value.

```csharp
workbook.GetEffectiveValue("Budget", CellRef.Parse("D5"));   // computed result
workbook.Evaluate("SUM(A1:A9)", "Budget", CellRef.Parse("Z1")); // ad-hoc, not stored
workbook.Calc.CircularCells;   // non-empty when the sheet has a circular reference
```

Roughly 80 functions are implemented across math, statistics, logic, text, lookup, date and
information categories. Unknown functions evaluate to `#NAME?` rather than throwing.

**Reading a value: use `GetEffectiveValue` / `GetDisplayValue`, not `Worksheet.GetValue`.**
`GetValue` returns what is stored in the file, which for a formula cell is the cached result and is
stale the moment anything upstream changes.

## Reading and formatting

```csharp
var sheet = workbook["Budget"];
sheet.UsedRange;                       // bounding box of populated cells, or null
sheet.GetFormula(cell);                // formula text without the leading '='
sheet.GetDisplayValue(cell);           // computed value

var format = workbook.Styles.Resolve(sheet.GetStyleIndex(cell));
var text = workbook.Styles.Format(sheet.GetDisplayValue(cell), format);
```

## Driving the grid from a toolbar

Both hosts expose the same `SpreadsheetController`:

```csharp
var controller = view.Controller;   // MAUI: Sheet.Controller, Blazor: view.Controller
controller.Selection.Active;        // CellRef
controller.ActiveCellText;          // what a formula bar should show
controller.BeginEdit();
controller.Move(MoveDirection.Down, extend: false, toEdge: true);   // Ctrl+Down
controller.ClearSelection();
controller.Undo();
```

## Saving

```csharp
await workbook.SaveAsync();                 // back over the path it was opened from
await workbook.SaveAsAsync("/new/path.xlsx");
await workbook.SaveToAsync(stream);
var bytes = workbook.ToArray();
```

Saving writes atomically (temp file plus move), refreshes the cached results of every formula, and
sets `fullCalcOnLoad` so Excel re-verifies the numbers when it opens the file.

**An unmodified workbook is never rewritten** — opening and saving without an edit produces a
byte-identical file.

## What is preserved, and what is not

Edits are applied surgically to the open OOXML package. Parts the editor does not model — macros,
tracked changes, custom XML, pivot caches, embedded objects, conditional formatting, charts — are
never touched and survive a save intact.

Pass an `UnsupportedFeatureCollector` when opening to find out what a document contains that the
editor cannot show or edit:

```csharp
var collector = new UnsupportedFeatureCollector();
using var workbook = await Workbook.OpenAsync(path, collector);

foreach (var feature in collector.Features)
    Console.WriteLine($"{feature.Part}: {feature.Feature} ({feature.Severity})");
```

## Not implemented yet

Do not generate code that assumes these exist:

- **Insert/delete rows and columns.** Deliberately deferred: it requires rewriting references across
  formulas, merged cells, conditional formatting, defined names, data validation, charts and tables.
- Adding or removing merged cells (existing merges render, but cannot be changed).
- Editing charts, pivot tables or conditional formatting.
- Multi-range ("Ctrl-click") selection.
- Copy/paste and the fill handle's drag-to-fill behaviour (the handle is drawn but inert).
- Physical-key navigation on MAUI — MAUI has no portable key-down event, so arrow keys work on
  Blazor only. On MAUI, call `Move`/`BeginEdit`/`ClearSelection` from your own platform key hook.
