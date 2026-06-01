# Tray Icon (Desktop)

`Shiny.Maui.Controls.TrayIcon` is a **separate, MAUI-only** add-on package that adds a cross-platform system tray / status-bar icon to your MAUI desktop app.

Supported platforms:

- **Windows** — `Shell_NotifyIcon`
- **macOS AppKit** — `NSStatusBar.SystemStatusBar` (native macOS app, `net10.0-macos`)
- **MacCatalyst** — bridges to AppKit at runtime via the Objective-C runtime
- **Linux** — `libayatana-appindicator3` + GTK 3 (native dependency must be installed: `libayatana-appindicator3-1`, `libgtk-3-0`)

There is **no Blazor equivalent** — tray icons are a desktop OS concept.

## Setup

```bash
dotnet add package Shiny.Maui.Controls.TrayIcon
```

In `MauiProgram.cs`:

```csharp
using Shiny;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseTrayIcon();
```

`UseTrayIcon()` registers `ITrayIconFactory` as a singleton — the correct per-platform factory is picked automatically. On Android, iOS, or any other unsupported runtime, `Create()` throws `PlatformNotSupportedException`, so guard your code with platform checks if the same MAUI app runs on mobile too.

## Creating a tray icon

Resolve the factory from DI and create as many icons as you need. Icons are not owned by any MAUI `Window` — they live for the lifetime of your process until you call `Dispose()`.

```csharp
public class MyTrayHost(ITrayIconFactory factory)
{
    ITrayIcon? icon;

    public void Start()
    {
        this.icon = factory.Create();
        this.icon.Tooltip = "My App";
        this.icon.IsTemplateImage = true; // macOS auto-tint
        this.icon.SetIcon(() => FileSystem.OpenAppPackageFileAsync("trayicon.png").Result);
        this.icon.SetMenu(BuildMenu());

        this.icon.PrimaryClick   += (_, _) => ShowMainWindow();
        this.icon.SecondaryClick += (_, _) => { /* opens menu automatically */ };
        this.icon.DoubleClick    += (_, _) => OpenSettings();
    }

    public void Stop() => this.icon?.Dispose();
}
```

The same **PNG** asset works on every platform — on Windows the implementation auto-wraps PNG bytes in an ICO container so you don't need a separate `.ico` file.

## Building menus

`TrayMenu.Build(b => …)` is the idiomatic builder. Menus are an `ObservableCollection` underneath — mutate items at any time and the platform handler rebuilds automatically.

```csharp
var menu = TrayMenu.Build(b => b
    .Item("Show window",   () => ShowMainWindow())
    .Check("Notifications", true, enabled => SetNotifications(enabled))
    .Separator()
    .Submenu("Status", s => s
        .Item("Available", () => SetStatus(Status.Available))
        .Item("Busy",      () => SetStatus(Status.Busy))
        .Item("Away",      () => SetStatus(Status.Away)))
    .Separator()
    .Item("Quit", () => Application.Current!.Quit()));

icon.SetMenu(menu);
```

Menu item types:

| Type | Builder method | Notes |
|---|---|---|
| `TrayMenuItem` | `.Item(label, action)` | Standard clickable item. Supports optional `Accelerator` display string |
| `TrayCheckMenuItem` | `.Check(label, isChecked, toggled)` | Renders a checked state. `toggled(bool)` receives the new value |
| `TraySeparator` | `.Separator()` | Visual separator |
| `TraySubmenu` | `.Submenu(label, builder)` | Nested menu — same builder API |

All items expose `IsEnabled`, `IsVisible`, and `Label`. Set them at any time:

```csharp
var pauseItem = new TrayMenuItem("Pause sync", () => Pause());
menu.Items.Add(pauseItem);
// later
pauseItem.Label = "Resume sync";
pauseItem.IsEnabled = !syncing;
```

The handler subscribes to the menu's internal `Changed` event and rebuilds the native menu when any item property changes or the collection is mutated.

## ITrayIcon API

| Member | Description |
|---|---|
| `SetIcon(Func<Stream>)` | Set the icon from a stream factory. PNG or ICO bytes both work. Use a factory (not a Stream directly) so the host can re-read for DPI/theme changes |
| `Tooltip` | Hover tooltip (Windows / macOS) or accessible description (Linux) |
| `Title` | Optional text label shown beside or instead of the icon. macOS / Linux only — ignored on Windows |
| `IsVisible` | Show/hide without disposing — `true` by default |
| `IsTemplateImage` | macOS only: when `true`, the icon is treated as a template image and auto-tints for the light/dark menu bar. Supply a flat black-on-transparent PNG |
| `SetMenu(TrayMenu)` | Assign or replace the context menu |
| `ShowMenu()` | Programmatically open the menu — useful from a `PrimaryClick` handler on Windows where left-click doesn't open the menu by default |
| `PrimaryClick` | Left-click. On macOS, primary click opens the menu instead — handler still fires for telemetry |
| `SecondaryClick` | Right-click / control-click |
| `DoubleClick` | Windows + macOS only — Linux has no double-click signal |
| `Dispose()` | Removes the tray icon and frees native resources. Always dispose when you're done — orphaned icons can persist in the Windows tray until reboot |

`TrayClickEventArgs` carries `X` / `Y` screen coordinates (best-effort across platforms).

## Platform notes & gotchas

### Linux
- Hard dependency on `libayatana-appindicator3.so.1` and `libgtk-3.so.0`. Install via your distro:
  - Debian/Ubuntu: `apt install libayatana-appindicator3-1 libgtk-3-0`
  - Fedora: `dnf install libayatana-appindicator-gtk3 gtk3`
- GNOME 40+ needs the AppIndicator extension installed by the user; KDE works out of the box.
- The first tray icon initializes GTK (`gtk_init_check`). Subsequent icons reuse the existing GTK state.
- Icon PNG is written to a temp file (the app-indicator API takes a path, not a stream).

### MacCatalyst
- Catalyst is UIKit and has no `NSStatusItem`. The implementation `dlopen`s AppKit at runtime and goes through `objc_msgSend`.
- Menu callbacks ride a runtime-allocated `NSObject` subclass (`ShinyTrayCB`) with `[UnmanagedCallersOnly]` trampolines — fully AOT-compatible.
- Hardened sandboxes that disallow loading AppKit will reject the `dlopen`. Normal Catalyst apps work fine.

### macOS AppKit
- Uses native MAUI bindings on `net10.0-macos`. Cleanest of the four implementations.
- Click events route via `NSStatusItem.Button.Activated`; left vs right is distinguished by inspecting `NSApplication.SharedApplication.CurrentEvent`.
- On macOS, **primary (left) click opens the menu** when one is assigned. To get a left-click event handler that doesn't open the menu, don't call `SetMenu` and use `ShowMenu()` from within your handler instead.

### Windows
- Uses Win32 `Shell_NotifyIcon` with a hidden message-only window for `WM_TRAYICON` callbacks.
- Right-click opens the menu via `TrackPopupMenuEx`.
- The implementation auto-wraps PNG bytes in a Vista-style ICO container — pass the same PNG you use everywhere else.
- **Windows 11** hides new tray icons in the overflow flyout by default. Users have to drag yours into the always-visible area. Document this for your users.
- Always call `Dispose()` on app shutdown or the icon may persist in the tray until reboot.

## When to use this skill

Invoke the tray icon skill when the user asks for:

- A system tray icon, status bar icon, menu bar icon
- A desktop app that runs in the background with a tray icon
- A tray icon with a right-click context menu, submenus, checkmark items
- Click handlers (left, right, double) on a tray icon
- Showing tooltips on a tray icon
- A "menu bar app" (macOS) — note this also requires `LSUIElement` in `Info.plist`
- Tray icons that auto-adapt to macOS dark/light menu bar (template images)
- Linux app indicators / KDE/GNOME tray icons
- Show/hide / disable / enable a tray icon at runtime
- Updating tray menu items dynamically (e.g. "Pause" ↔ "Resume", check states)
