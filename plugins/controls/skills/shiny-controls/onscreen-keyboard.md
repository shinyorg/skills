# On-Screen Keyboard (Touch / Kiosk)

A focused on-screen keyboard for touch tablets and kiosks. Auto-shows when an `<input>` or
`<textarea>` gains focus, docks along the bottom edge, and types into whatever's focused — without
ever taking the caret off it.

> **Blazor only.** The keyboard is implemented in **`Shiny.Blazor.Controls`**
> (`Shiny.Blazor.Controls.OnScreenKeyboard` namespace), with no add-on package — it sits in the main
> Blazor package alongside [Docking](./docking.md).
>
> **There is no MAUI implementation.** `Shiny.Maui.Controls.Desktop.OnScreenKeyboard`,
> `UseOnScreenKeyboard`, `IOnScreenKeyboard` and `OnScreenKeyboardView` do **not** exist and will not
> compile. Never generate them. If the user asks for an on-screen keyboard on MAUI, say it is not
> built yet rather than emitting code for it.

Scope is intentionally narrow: English US-QWERTY, dispatch into the host app's own DOM text fields,
no IME / dead keys / language switching. The 80% case for kiosks, not a replacement for the OS
on-screen keyboard.

## Setup

`Program.cs` — prefer the umbrella call, which also covers Toast, Dialogs, SplashScreen, Walkthrough
and Docking:

```csharp
using Shiny.Blazor.Controls;

builder.Services.AddShinyControls(cfg => cfg
    .ConfigureKeyboard(opts =>
    {
        opts.AutoShowOnFocus = true;
        opts.AutoHideOnBlur  = true;
        opts.HeightPx        = 280;
        opts.PushContent     = true;
        opts.Theme           = OnScreenKeyboardTheme.Auto;
    })
);
```

Or à la carte, when the app only wants the keyboard:

```csharp
using Shiny.Blazor.Controls.OnScreenKeyboard;

builder.Services.AddShinyOnScreenKeyboard(opts => opts.HeightPx = 280);
```

Both are `TryAdd`, so calling both is safe and the first registration wins.

`_Imports.razor`:

```razor
@using Shiny.Blazor.Controls.OnScreenKeyboard
```

Place **exactly one** host, near the root of the layout (typically `MainLayout.razor`):

```razor
<OnScreenKeyboardHost />
```

The host is `position: fixed`, so where it sits in the layout does not matter — but it watches focus
for the whole document, so emit one per app, not one per page. (A demo page that wants the keyboard
scoped to itself is the one exception; the Blazor sample does exactly that.)

`OnScreenKeyboardHost` takes one optional parameter, `CssClass`.

## Public surface

```csharp
public interface IOnScreenKeyboardService
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
    public double HeightPx      { get; set; } = 280;
    public bool PushContent     { get; set; } = true;    // false = overlay the content
    public OnScreenKeyboardTheme Theme { get; set; } = OnScreenKeyboardTheme.Auto;
    public bool EnterInsertsNewLine    { get; set; }     // textarea only
    public TimeSpan AutoRepeatDelay    { get; set; } = TimeSpan.FromMilliseconds(400);
    public TimeSpan AutoRepeatInterval { get; set; } = TimeSpan.FromMilliseconds(50);
}

public enum OnScreenKeyboardTheme { Auto, Light, Dark }
```

`OnScreenKeyboardOptions` is registered **scoped** and is **live** — inject it and mutate it at
runtime and the host picks the new values up on its next render. That is the supported way to expose
keyboard settings in a settings screen. Scoped rather than singleton because it is per-user state:
identical under WASM, but on Blazor Server a singleton would apply one user's settings to everyone.
Note that the `configure` delegate therefore runs once per scope, against that scope's own instance.

Drive visibility from any component:

```razor
@inject IOnScreenKeyboardService Keyboard

<button @onclick="() => Keyboard.Show()">Kiosk mode</button>
```

A kiosk screen usually sets `AutoShowOnFocus = false` and `AutoHideOnBlur = false` and pins the
keyboard up — otherwise auto-hide drops it the moment the user taps one of your own buttons, which
genuinely does move focus off the field.

## Layout

US-QWERTY, plus a symbols layer behind the `123` key. Two layers, not three:

| Layer | Keys |
|---|---|
| **Letters** | `` ` 1 2 3 4 5 6 7 8 9 0 - = ⌫`` / ⇥ q…p `[ ] \` / ⇪ a…l `; '` ⏎ / ⇧ z…m `, . /` ⇧ |
| **Symbols** (`123`) | digits + `- = +` / `~ ! @ # $ % ^ & * ( ) _ [ ] \` / `€ £ ¥ ¢ ° ± × ÷ { } \| : ;` ⏎ / `« » " ' < > ? / , . • – — … ¡` |
| **Bottom row** (both) | `123`/`ABC` · `,` · space · `.` · ◀ ▼ ▲ ▶ · ⌄ (hide) |

Shift and Caps Lock are **not** layers — they are state applied to the letter layer at render time.
That is the only way `⇪` can raise the letters while leaving the number row alone, the way a real
keyboard behaves. `⇧` is momentary (drops after one character), `⇪` and `123` are sticky, and all
three light up while engaged.

Holding a character, `⌫`, space or an arrow auto-repeats after `AutoRepeatDelay`. A held key keeps
repeating the character it typed first, so it does not fall out of shift on the second repeat.

There are **no Ctrl or Alt keys.** Modifier chords are not dispatched, and shipping inert keys that
look like they should work is worse than not having them.

## The non-obvious bits

**1. No focus stealing.** Every key cancels `pointerdown` — on the key itself and on the container,
so the gaps between keys count too — which stops the browser running its focus default. The caret
stays in the field the user is typing into and `document.activeElement` remains the typing target.
Without this the target input loses its caret the moment the first key is tapped: the single biggest
cause of "the OSK does nothing" reports.

**2. Caret and selection.** Typing goes in through `execCommand('insertText')`, which replaces the
selection and keeps the undo stack, with a deterministic value splice behind it for browsers that
decline. Because a programmatic value assignment does not set the element's dirty flag, that
fallback raises both `input` and `change` itself — so plain `@bind` and `@bind:event="oninput"` both
see the keystroke. Backspace uses the splice directly on form fields, where `execCommand('delete')`
is unreliable.

**3. Arrows know about lines.** `▲` / `▼` in a `<textarea>` walk to the same column on the adjacent
line rather than moving one character; contenteditable is handed to `Selection.modify` instead of
hand-rolled offset maths.

**4. Two kinds of keyboard avoidance.** `PushContent` pads the document body out from under the
keys. That is only half of it, so the host also measures the focused field against the keyboard's
top edge and scrolls it clear — in whichever container actually scrolls, not just the body. Apps
whose content scrolls inside a shell get the second behaviour whether or not they take the first.

## Enter

On a single-line `<input>`, `Enter` dispatches real `keydown` / `keyup` events and then submits the
containing form — unless a handler cancelled the keydown, in which case it does not. It never types
a newline there, because a newline in a single-line field is meaningless.

In a `<textarea>`, set `EnterInsertsNewLine = true` to type a newline instead.

## Theming

CSS custom properties, settable on any ancestor or on `<OnScreenKeyboardHost>` directly:

`--shiny-osk-bg`, `--shiny-osk-border`, `--shiny-osk-key-bg`, `--shiny-osk-key-fg`,
`--shiny-osk-modifier-bg`, `--shiny-osk-key-pressed-bg`, `--shiny-osk-key-pressed-fg`,
`--shiny-osk-height`, `--shiny-osk-gap`, `--shiny-osk-radius`.

`OnScreenKeyboardTheme.Auto` (the default) resolves those from the app's Shiny theme tokens, so it
tracks a runtime theme switch. `Light` and `Dark` pin a fixed palette regardless of the rest of the
app — usually what a kiosk wants. `prefers-reduced-motion` drops the slide-in transition.

## Limitations

- **DOM inputs only.** No injection into another window, another process, or a cross-origin
  `<iframe>`. For kiosk apps this is the desired behaviour.
- **Shadow DOM** — `focusin` does not pierce shadow roots, so Web Components with an internal
  `<input>` are not detected.
- **Rich editors** (Quill, ProseMirror, Monaco) — best-effort. `insertText` is fine against
  `<input>` / `<textarea>` / simple contenteditable and gets odd inside a full editor framework.
- **No IME, no dead keys, no language switching.**
- **No Ctrl / Alt chords.**

## Accessibility

`role="application"` with an accessible name; each key is a button whose `aria-label` matches what
it will actually type in the current shift state; `⇧` / `⇪` / `123` report `aria-pressed`.

Keys are `tabindex="-1"` on purpose — entering the tab order would mean taking focus, which is the
one thing this control exists to avoid. The ARIA tree makes the board describable, not
tab-navigable; do not describe it as switch-navigable.

## When to use this skill

Invoke the OSK skill when the user wants any of:

- A **touch-screen / kiosk on-screen keyboard** for a Blazor app
- A **soft keyboard** / virtual keyboard / OSK that auto-shows when an `<input>` gains focus
- A **bottom-docked keyboard** that pushes page content up
- A keyboard that types into the focused control without stealing focus
- Code-driven visibility (`keyboard.Show()` / `Hide()` / `Toggle()`)
- A kiosk / point-of-sale app that needs typed input without a hardware keyboard
- A keyboard for a Blazor PWA on a tablet
- A keyboard with sticky Caps Lock and momentary Shift

Do NOT invoke this skill when:

- The target is **.NET MAUI** — it is not implemented there; say so instead of generating code
- The user wants the **OS's** on-screen keyboard (`osk.exe` / TabTip) — a different, simpler wrapper
- The user needs IME / multilingual input / dead-key composition
- The user wants to inject keystrokes into other apps / other windows / other processes
- The user wants to decorate the *mobile OS* keyboard with an accessory bar — that is a different,
  MAUI-side concern and also not built

## Related

- [Docking](./docking.md) — ships in the same `Shiny.Blazor.Controls` package
- [TextEntry](./textentry.md) — a Shiny `TextEntry` is an `<input>` underneath, so the OSK types
  into it with no extra wiring
