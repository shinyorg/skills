# Docking (Desktop)

A Visual-Studio-style window-docking system for desktop apps — dockable tool windows, tabbed groups, splitters, auto-hide rails, and tear-off floating windows. Ships in two packages:

| Package | Use when |
|---|---|
| **`Shiny.Maui.Controls.Desktop`** (`Shiny.Maui.Controls.Desktop.Docking` namespace) | .NET MAUI desktop apps. Combined with the [Tray Icon](./tray-icon.md) feature in the same package since both share the desktop TFM matrix (`net10.0;-windows;-maccatalyst;-macos` + Linux GTK4 path) |
| **`Shiny.Blazor.Controls.Kiosk`** (`Shiny.Blazor.Controls.Kiosk.Docking` namespace) | Blazor apps. WASM-supported in v1; Blazor Server / `BlazorWebView` will be supported as the contracts stabilize. The package also hosts a forthcoming on-screen keyboard — both features share the kiosk-shaped TFM-free Blazor surface |

Mobile (iOS / Android) is **out of scope by design**. Tear-off floating windows on macOS Catalyst are limited to in-app floating — native AppKit (`net10.0-macos`) and Windows get full tear-off via `NSPanel` / `AppWindow`; native GTK4 windows on Linux.

## Setup (.NET MAUI)

```bash
dotnet add package Shiny.Maui.Controls.Desktop
```

`MauiProgram.cs`:

```csharp
using Shiny;
using MyApp.Panels;   // SolutionExplorerPanel, OutputPanel — your dockable views

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyDocking()
    .AddDockPanel<SolutionExplorerPanel>("solution-explorer")
    .AddDockPanel<OutputPanel>("output");
```

Each call to `AddDockPanel<TView>("id")`:

1. Registers `TView` as a transient service in DI.
2. Registers an `IDockableContentFactory` keyed to the stable string ID you supply.

Persisted dock layouts reference panels by that ID — so you can rename the C# class without breaking saved layouts, and unknown IDs in a stored layout get parked in a "missing panels" tray rather than silently dropped.

Attach the host inside any existing `ContentPage` — `DockHostView` is a `ContentView`, **not** a `ContentPage` subclass. You keep full control of your Shell / page architecture:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:docking="clr-namespace:Shiny.Maui.Controls.Desktop.Docking;assembly=Shiny.Maui.Controls.Desktop"
             x:Class="MyApp.DockingShellPage"
             Title="IDE">
    <docking:DockHostView InitialLayout="{Binding StartupLayout}" />
</ContentPage>
```

A dockable panel is just a regular `View`:

```csharp
public sealed class SolutionExplorerPanel : ContentView
{
    public SolutionExplorerPanel(/* inject anything you need */)
    {
        Content = new VerticalStackLayout
        {
            Padding = 12,
            Children = { new Label { Text = "Solution Explorer" } }
        };
    }
}
```

## Setup (Blazor)

```bash
dotnet add package Shiny.Blazor.Controls.Kiosk
```

`Program.cs`:

```csharp
using Shiny.Blazor.Controls.Kiosk.Docking;

builder.Services
    .AddShinyDocking()
    .AddDockPanel<SolutionExplorerPanel>("solution-explorer")
    .AddDockPanel<OutputPanel>("output");
```

`_Imports.razor`:

```razor
@using Shiny.Blazor.Controls.Kiosk.Docking
```

Any razor page:

```razor
<DockHost />
```

Each panel is a regular Razor component (`ComponentBase` subclass). Blazor docking supports in-app floating; popping panels out into separate browser windows is not supported (Blazor runtime instances cannot share component instances across windows).

## Public Surface

### Schema (POCO tree — round-trips to JSON)

```
DockRoot
├── int SchemaVersion + int MinReadableVersion
├── DockWindowState MainWindow
└── List<DockWindowState> FloatingWindows         (order = z-order)

DockWindowState
├── DockRect? Bounds                              (screen-coord rectangle — desktop only)
├── bool IsMaximized + bool IsFullScreen
├── DockNode DocumentArea                         (structurally distinct document well)
├── DockNode? LeftRail + TopRail + RightRail + BottomRail
└── string? ActivePanelId                         (for focus restoration on load)

DockNode = DockSplit | DockGroup | DockEmpty       (System.Text.Json polymorphic, $kind discriminator)
DockSplit  { Orientation, Ratio (0..1), First, Second }
DockGroup  { GroupId, List<DockTab> Tabs, int ActiveTabIndex, List<int> FocusHistory (MRU) }
DockEmpty  { }

DockTab    { PanelTypeId, PanelInstanceId, bool IsPinned }
```

`DockSerialization.Serialize(DockRoot)` and `DockSerialization.Deserialize(string)` use a source-generated `JsonSerializerContext` (AOT-safe). Schema versioning is built in from day one: bump `SchemaVersion`, write an `IDockLayoutMigrator` for each step, and stored layouts migrate forward as users upgrade.

### Contracts

```csharp
public interface IDockHost
{
    bool IsLocked { get; set; }                    // layout read-only mode
    IDockEvents Events { get; }
    IDockCommandScope CommandScope { get; }

    Task LoadAsync(DockRoot root, CancellationToken ct = default);
    DockRoot Snapshot();

    Task ShowPanelAsync(string panelTypeId, DockArea preferredArea = DockArea.Left, CancellationToken ct = default);
    Task HidePanelAsync(string panelInstanceId, CancellationToken ct = default);
    Task ActivatePanelAsync(string panelInstanceId, CancellationToken ct = default);
    Task ResetLayoutAsync(CancellationToken ct = default);
}

public interface IDockableContent
{
    string Title { get; }
    object? Icon { get; }
    bool CanClose { get; }
    bool CanFloat { get; }
    void OnActivated();
    void OnDeactivated();

    // Embedded editors return true to claim pointer-down — prevents
    // the dock system from starting a tab drag during caret edits.
    bool WantsPointerDown(double x, double y);
}

public interface IDockableContentFactory
{
    string PanelTypeId { get; }
    Task<View> CreateAsync(string instanceId, CancellationToken ct = default);   // MAUI
    // (Blazor variant returns Task<RenderFragment>)
}

public interface IDockLayoutStore                  // bring-your-own — no default ships
{
    Task<DockRoot?> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(DockRoot root, CancellationToken ct = default);
    int SaveDebounceMs { get; }
}

public interface IDockLayoutMigrator               // forward-only migrations
{
    int FromVersion { get; }
    int ToVersion { get; }
    DockRoot Migrate(DockRoot input);
}

public interface IDockEvents
{
    event EventHandler<LayoutChangedEventArgs>? LayoutChanged;
    event EventHandler<PanelActivatedEventArgs>? PanelActivated;
    event EventHandler<DockDragEventArgs>? DragStarted;
    event EventHandler<DockDragEventArgs>? DragCompleted;
    event EventHandler<DockDragEventArgs>? DragCancelled;
}

public interface IDockCommandScope                 // for routing Ctrl+W / Ctrl+Tab / Ctrl+Alt+PgUp/Dn
{
    bool IsInScope { get; }                        // is keyboard focus inside the dock surface
    string? ActiveGroupId { get; }
    string? ActivePanelInstanceId { get; }
}
```

### Controls

| Type | Purpose |
|---|---|
| `DockHostView` (MAUI) / `<DockHost />` (Blazor) | Root dock surface. Attaches to an existing page; exposes `IDockHost` |
| `DockGroupView` | Tabbed group of panels |
| `DockTabStrip` | Tab strip with overflow (scroll + chevron) and drag-to-reorder/tear-off |
| `DockSplitter` | Draggable splitter between two adjacent dock children; reports position as a 0..1 ratio so layouts survive resize |

## Disposal contract

When a panel is closed, the host disposes the content if it implements `IDisposable`. Tearing off to a floating window or moving between groups in the same window **does not** dispose — the same view instance is moved. This is intentional and documented because consumers will rely on whichever behavior ships first; retrofitting "recreated" → "preserved" would break every panel implementation.

## Persistence — bring-your-own store

There is no default `IDockLayoutStore` in v1 — wire one up against whatever you already use. Two minimal patterns:

```csharp
// File-backed
public sealed class FileDockLayoutStore : IDockLayoutStore
{
    readonly string path;
    public FileDockLayoutStore(string path) => this.path = path;
    public int SaveDebounceMs => 500;

    public async Task<DockRoot?> LoadAsync(CancellationToken ct = default)
        => File.Exists(path) ? DockSerialization.Deserialize(await File.ReadAllTextAsync(path, ct)) : null;

    public Task SaveAsync(DockRoot root, CancellationToken ct = default)
        => File.WriteAllTextAsync(path, DockSerialization.Serialize(root), ct);
}
```

```csharp
// Shiny.Stores-backed (works on every host, syncs across launches)
public sealed class ShinyStoreDockLayoutStore(IKeyValueStore store) : IDockLayoutStore
{
    public int SaveDebounceMs => 500;

    public Task<DockRoot?> LoadAsync(CancellationToken ct = default)
        => Task.FromResult(store.Get<string?>("docking.layout") is { } json
            ? DockSerialization.Deserialize(json) : null);

    public Task SaveAsync(DockRoot root, CancellationToken ct = default)
    {
        store.Set("docking.layout", DockSerialization.Serialize(root));
        return Task.CompletedTask;
    }
}
```

## Theming

CSS custom properties (Blazor) and `ResourceDictionary` keys (MAUI) mirror each other so the same token list themes both hosts. Tokens are locked before styling lands so you don't end up with consumer-side `!important` hacks. Reduced-motion respect and `prefers-reduced-motion` honoring are first-class — animation duration is a theme token, not a constant.

## When to use this skill

Invoke the docking skill when the user asks for any of:

- A **Visual Studio-style** layout with dockable tool windows, tabbed groups, splitters
- A **dock host** / `DockHostView` / `<DockHost />` attached to a page
- A way to **register panels** by ID so layouts can be saved and reloaded
- Saving and restoring dock layouts to/from JSON, with schema versioning and migrations
- Tab tear-off / merge / reorder / drag-drop between groups
- Auto-hide rails on the left/right/top/bottom edges
- Tear-off floating windows on Windows, macOS AppKit, or Linux GTK4
- Layout locking, kiosk mode, read-only layout
- Workspace perspectives (Debugging / Design / Review preset layouts)
- Blazor docking (in-app floating)
- Avoiding global mouse hooks on macOS for cross-window drag (the architecture uses implicit mouse-down capture instead — no Accessibility entitlement required, App Store compatible)
- Adding a docking control to a `Shiny.Maui.Controls`-based app without subclassing `ContentPage`

## Related
- [Tray Icon](./tray-icon.md) — ships in the same `Shiny.Maui.Controls.Desktop` package
- [FloatingPanel](./floating-panel.md) — a mobile-shaped bottom/top sheet, distinct from the desktop docking auto-hide rails
