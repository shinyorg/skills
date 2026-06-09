# On-Screen Keyboard (Touch / Kiosk)

A focused on-screen keyboard for touch tablets and kiosks. Auto-shows when an `Entry` / `Editor` (MAUI) or `<input>` / `<textarea>` (Blazor) gains focus, docks along the bottom edge, types into whatever's focused — without stealing focus when keys are tapped.

Two packages, same shape:

| Package | Use when |
|---|---|
| **`Shiny.Maui.Controls.Desktop`** (`Shiny.Maui.Controls.Desktop.OnScreenKeyboard` namespace) | .NET MAUI desktop apps. Bundled with [Tray Icon](./tray-icon.md) and [Docking](./docking.md) under the desktop-only TFM matrix |
| **`Shiny.Blazor.Controls.Kiosk`** (`Shiny.Blazor.Controls.Kiosk.OnScreenKeyboard` namespace) | Blazor apps. Bundled with [Docking](./docking.md) under the kiosk-shaped Blazor add-on |

Scope intentionally narrow: English US-QWERTY, dispatch into the host app's own text fields, no IME / dead keys / language switching. The 80% case for kiosks, not a replacement for the OS on-screen keyboard.

## Setup (.NET MAUI)

```bash
dotnet add package Shiny.Maui.Controls.Desktop
```

`MauiProgram.cs`:

```csharp
using Shiny;
using Shiny.Maui.Controls.Desktop.OnScreenKeyboard;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseOnScreenKeyboard(opts =>
    {
        opts.AutoShowOnFocus = true;
        opts.AutoHideOnBlur  = true;
        opts.Height          = 280;
        opts.PushContent     = true;
        opts.Theme           = OnScreenKeyboardTheme.Light;
    });
```

Inline placement (for kiosk-style pages that always show the keyboard):

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:osk="clr-namespace:Shiny.Maui.Controls.Desktop.OnScreenKeyboard;assembly=Shiny.Maui.Controls.Desktop">
    <Grid RowDefinitions="*,Auto">
        <!-- page content -->
        <osk:OnScreenKeyboardView Grid.Row="1" Height="280" />
    </Grid>
</ContentPage>
```

Code-driven visibility:

```csharp
public class MyPageViewModel(IOnScreenKeyboard keyboard)
{
    public void StartKioskMode() => keyboard.Show();
}
```

## Setup (Blazor)

```bash
dotnet add package Shiny.Blazor.Controls.Kiosk
```

`Program.cs`:

```csharp
using Shiny.Blazor.Controls.Kiosk.OnScreenKeyboard;

builder.Services.AddShinyOnScreenKeyboard(opts =>
{
    opts.AutoShowOnFocus = true;
    opts.AutoHideOnBlur  = true;
    opts.HeightPx        = 280;
    opts.PushContent     = true;
});
```

`_Imports.razor`:

```razor
@using Shiny.Blazor.Controls.Kiosk.OnScreenKeyboard
```

Place once at the root layout (typically `MainLayout.razor`):

```razor
<OnScreenKeyboardHost />
```

Inject `IOnScreenKeyboardService` into any component to drive visibility from code.

## Public Surface

Identical shape on both renderers — only `View` / `RenderFragment` differs.

```csharp
public interface IOnScreenKeyboard            // IOnScreenKeyboardService on Blazor
{
    bool IsVisible { get; }
    event EventHandler<bool>? VisibilityChanged;

    void Show();
    void Hide();
    void Toggle();
}

public sealed class OnScreenKeyboardOptions
{
    public bool AutoShowOnFocus { get; set; } = true;
    public bool AutoHideOnBlur  { get; set; } = true;
    public double Height        { get; set; } = 280;     // HeightPx on Blazor
    public bool PushContent     { get; set; } = true;    // false = overlay above content
    public OnScreenKeyboardTheme Theme { get; set; } = OnScreenKeyboardTheme.Light;
    public TimeSpan AutoRepeatDelay    { get; set; } = TimeSpan.FromMilliseconds(400);
    public TimeSpan AutoRepeatInterval { get; set; } = TimeSpan.FromMilliseconds(50);
}
```

## Layout

US-QWERTY with three switchable layers:

| Layer | Keys |
|---|---|
| **Lowercase** | `` ` 1-0 - = ⌫`` / Tab QWERTYUIOP [ ] \ / Caps ASDFGHJKL ; ' Enter / Shift ZXCVBNM , . / Shift / Ctrl Alt Space Alt ◀ ▼ ▲ ▶ |
| **Shift** | Capitals + shifted symbols; modifier indicator lit until released. Caps Lock makes it sticky |
| **123 / Symbols** | Numeric pad + punctuation + arrows. Sticky toggle |

## The non-obvious bits

Two implementation details that make or break a touch OSK:

**1. No focus stealing.** Every key uses `pointerdown` + `preventDefault()` (Blazor) or `Focusable = false` + intercepted `PointerPressed` (MAUI). Without this, the target input loses its caret the moment the user taps a key — single biggest cause of "the OSK doesn't work" bug reports.

**2. Caret-position tracking.** `Text` mutation at `CursorPosition` (MAUI) and `execCommand('insertText')` (Blazor) both handle the easy case cleanly, but selection-replace (user selects "abc" and types "x" → result "x", not "abcx") needs bookkeeping. The OSK tracks `SelectionStart`/`SelectionLength` and replaces the range when present.

## Limitations

- **MAUI inputs only / DOM inputs only.** OSK dispatches into the host's text inputs. No injection into other-app popups, WebView contents, or other-process windows. For kiosk apps this is the desired behaviour.
- **Shadow DOM** — `focusin` doesn't pierce shadow roots. Web Components with internal `<input>` elements aren't supported.
- **Rich editors** (Quill, ProseMirror, Monaco) — `execCommand('insertText')` works against `<input>` / `<textarea>` / simple contenteditable but gets weird inside complex editor frameworks. Best-effort, no guarantee.
- **Enter key** dispatches `keydown`/`keyup` `Enter` (so form submit fires). Does NOT insert `\n`. Configurable.
- **No IME, no dead keys, no language switching.**

## Theming

**MAUI** — `ResourceDictionary` keys: `OnScreenKeyboardKeyBrush`, `OnScreenKeyboardModifierBrush`, `OnScreenKeyboardPressedBrush`, `OnScreenKeyboardBackgroundBrush`, `OnScreenKeyboardForegroundBrush`.

**Blazor** — CSS custom properties: `--shiny-osk-key-bg`, `--shiny-osk-key-fg`, `--shiny-osk-key-pressed-bg`, `--shiny-osk-modifier-bg`, `--shiny-osk-bg`, `--shiny-osk-height`. Override on any parent or on `<OnScreenKeyboardHost>` directly.

Animation duration is a theme token, not a constant — reduced-motion honoring is built in.

## Accessibility

Every key exposes the appropriate automation role:

- **Windows MAUI**: `AutomationPeer` with `Name = label`, `LocalizedControlType = "key"`
- **macOS AppKit / Catalyst**: `NSAccessibilityRole.Button` + localized description
- **Linux GTK**: ATK role `KEY` via the underlying `GtkButton`
- **Blazor**: ARIA `role="button"` + `aria-keyshortcuts`, contained in `role="application"` so screen readers don't fight the typing flow

The AutomationPeer / ARIA tree is built in from the start, not retrofitted.

## When to use this skill

Invoke the OSK skill when the user wants any of:

- A **touch-screen / kiosk on-screen keyboard** for a MAUI desktop or Blazor app
- A **soft keyboard** / virtual keyboard / OSK that auto-shows when an Entry/Editor or `<input>` gains focus
- A **bottom-docked keyboard** that pushes page content up
- A keyboard that types into the focused control without stealing focus
- Code-driven visibility (`keyboard.Show()` / `Hide()` / `Toggle()`)
- An on-screen keyboard with **switch-input accessibility** (the OSK itself is keyboard-navigable for switch users)
- A kiosk app that needs typed input without a hardware keyboard
- Embedded systems / point-of-sale where the OS keyboard is unavailable or undesirable
- A keyboard for a Blazor PWA on a tablet
- A keyboard with sticky Caps Lock and momentary Shift

Do NOT invoke this skill when:

- The user wants the **OS's** on-screen keyboard (`osk.exe` / TabTip) — that's a different, simpler "launch the system OSK" wrapper, not part of this package
- The user needs IME / multilingual input / dead-key composition
- The user wants to inject keystrokes into other apps / WebView contents / windows in other processes

## Related

- [Tray Icon](./tray-icon.md) — ships in the same `Shiny.Maui.Controls.Desktop` package
- [Docking](./docking.md) — the OSK can be hosted in a floating dock window for "always on top" kiosk use
