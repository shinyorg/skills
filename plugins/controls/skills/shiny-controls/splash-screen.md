# SplashScreen (Blazor only)

A customizable boot/loading splash screen for Blazor apps — WebAssembly, Blazor Server, and MAUI Blazor Hybrid.

**This is not wired up like a normal control, and it cannot be.** The whole point is to be on screen while Blazor is still booting, so there is no component tree to render it into. It ships as three pieces:

| Piece | Where it lives | What it does |
|---|---|---|
| `splash.js` | `_content/Shiny.Blazor.Controls/splash.js` | Classic (non-module) script exposing a global `shinySplash`. Paints the splash on the first frame. |
| `shiny-splash.css` | `_content/Shiny.Blazor.Controls/css/shiny-splash.css` | Unscoped stylesheet (it cannot be a `.razor.css` — that requires Blazor). |
| `ISplashScreen` / `<SplashScreenHost />` | `Shiny.Blazor.Controls.Splash` | Managed side: status, progress, and the handoff to the app. |

**There is no MAUI equivalent and there should not be** — MAUI already has a native splash (`MauiSplashScreen`). In a MAUI Blazor Hybrid app you use *both*: the native splash covers window creation → WebView ready, and `SplashScreen` covers WebView → Blazor booted.

## The one rule: the host div goes OUTSIDE `#app`

Blazor wipes `#app`'s contents the instant it attaches the root component — which is well before the app has loaded its data. A splash placed inside `#app` therefore disappears too early and produces a blank flash. Put it as a sibling and dismiss it explicitly.

Load order also matters: `splash.js` must come **before** `blazor.webassembly.js` / `blazor.webview.js` so it paints first.

## Setup

```html
<!-- wwwroot/index.html (WASM / Hybrid) or App.razor (Blazor Web App) -->
<link href="_content/Shiny.Blazor.Controls/css/shiny-splash.css" rel="stylesheet" />
...
<body>
    <div id="app">...</div>

    <div id="shiny-splash"
         data-shiny-splash
         data-title="My App"
         data-subtitle="by Contoso"
         data-logo="img/logo.svg"
         data-spinner="ring"
         data-status="Starting up…"
         data-min-duration="600"
         data-fade="300"></div>

    <script src="_content/Shiny.Blazor.Controls/splash.js"></script>
    <script src="_framework/blazor.webassembly.js"></script>
</body>
```

```csharp
// Program.cs
using Shiny.Blazor.Controls.Splash;

builder.Services.AddShinySplashScreen();
```

```razor
@* MainLayout.razor or App.razor — renders nothing, owns the handoff *@
@using Shiny.Blazor.Controls.Splash

<SplashScreenHost />
```

That alone gets you a splash that covers the whole boot and fades out on first render.

## Reporting real startup progress

```razor
<SplashScreenHost Until="StartupAsync" />

@code {
    [Inject] ISplashScreen Splash { get; set; } = default!;

    async Task StartupAsync()
    {
        await Splash.SetStatusAsync("Loading accounts…");
        await Splash.SetProgressAsync(0.3);
        await this.accounts.LoadAsync();

        await Splash.SetStatusAsync("Refreshing…");
        await Splash.SetProgressAsync(0.8);
        await this.repos.RefreshAsync();
    }
}
```

`Until` runs after first render; the splash is dismissed in a `finally`, so a startup exception is still surfaced rather than hidden behind the splash forever.

To hide it from somewhere else entirely, set `AutoHide="false"` and call `ISplashScreen.HideAsync()` yourself (or grab the host with `@ref` and call `HideAsync()`).

## Replacing the stock template loader

The WASM template's loading UI (`<svg class="loading-progress">` + `.loading-progress-text`) lives **inside `#app`**, which is why it only covers the framework download — Blazor wipes it at attach. Replace it: empty `#app`, put the splash host beside it, and set `blazorLoadProgress` to keep the percentage behaviour (both read `--blazor-load-percentage`). The splash then spans the download *and* app startup. Same for the Hybrid template's `<div id="app">Loading...</div>`. Leave `#blazor-error-ui` alone.

```html
<div id="app"></div>
<div id="shiny-splash" data-shiny-splash data-blazor-progress="true" data-title="My App"></div>
```

## Three tiers of customization

Because it renders before Blazor, "customizable" cannot mean `RenderFragment`. It means one of:

### 1. Data attributes (no `<script>` block of your own)

Shown above. Every option below has a `data-` equivalent (`minDurationMs` → `data-min-duration`, `failSafeMs` → `data-fail-safe`, `blazorLoadProgress` → `data-blazor-progress`, and so on).

### 2. A config object

```html
<div id="shiny-splash"></div>
<script src="_content/Shiny.Blazor.Controls/splash.js"></script>
<script>
    shinySplash.show({
        title: 'My App',
        logo: 'img/logo.svg',
        spinner: 'dots',
        accent: '#7C3AED',
        background: '#0F172A',
        foreground: '#F8FAFC',
        minDurationMs: 400,
        fadeMs: 250
    });
</script>
```

### 3. Bring your own markup (preferred for anything bespoke)

If the host `<div>` already has children, the script **leaves them alone** — it only owns show/status/progress/fade/hide and binds three optional hooks:

```html
<div id="shiny-splash" data-shiny-splash>
    <div class="my-brand">
        <svg>…your animated logo…</svg>
        <p data-shiny-splash-status>Starting up…</p>
        <div class="my-track">
            <div data-shiny-splash-progress-fill></div>
        </div>
        <span data-shiny-splash-percent></span>
    </div>
</div>
```

| Hook | Set to |
|---|---|
| `[data-shiny-splash-status]` | `textContent` = the status text |
| `[data-shiny-splash-progress-fill]` | `style.width` = `42%` |
| `[data-shiny-splash-percent]` | `textContent` = `42%` |
| CSS vars on the host | `--shiny-splash-progress` (0–1) and `--shiny-splash-progress-pct` (`42%`) |

## `shinySplash.show` options

| Option | Type | Default | Description |
|---|---|---|---|
| hostId | string | `'shiny-splash'` | Element id to use; created and appended to `<body>` if missing |
| title | string? | null | Headline (skipped when you supply your own markup) |
| subtitle | string? | null | Secondary line |
| logo | string? | null | Image src |
| logoAlt | string | `''` | Image alt text |
| spinner | string | `'ring'` | `ring` \| `dots` \| `bar` \| `pulse` \| `none` |
| status | string? | null | Initial status line |
| progress | number? | null | 0–1; null means indeterminate |
| background / foreground / accent / muted | string? | theme tokens | Written as `--shiny-splash-*` CSS vars on the host |
| cssClass | string? | null | Extra class on the host |
| minDurationMs | number | 0 | Minimum time on screen — stops a warm start flickering |
| fadeMs | number | 250 | Fade-out duration |
| failSafeMs | number | 30000 | Auto-hide if `hide()` is never called (0 disables). Guards against a boot failure pinning the splash forever. |
| blazorLoadProgress | bool | false | **WASM only** — mirrors Blazor's `--blazor-load-percentage` into the progress bar during runtime download. No-op in Hybrid (there is no download phase). |
| removeOnHide | bool | true | Remove the host from the DOM on hide; false hides it so it can be re-shown |
| lockScroll | bool | true | Adds `shiny-splash-locked` to `<html>` while visible |

## `shinySplash` globals

| Call | Notes |
|---|---|
| `shinySplash.show(options)` | No-op if already showing |
| `shinySplash.status(text)` | |
| `shinySplash.progress(value)` | 0–1, or null for indeterminate. Also cancels the `blazorLoadProgress` tracker — an explicit call always wins. |
| `shinySplash.hide(fadeMs?)` | Idempotent; honours `minDurationMs`. Fires a `shiny-splash-hidden` event on `document`. |
| `shinySplash.isVisible()` | |

## `ISplashScreen`

| Member | Notes |
|---|---|
| `ValueTask<bool> IsVisibleAsync()` | |
| `ValueTask SetStatusAsync(string?)` | |
| `ValueTask SetProgressAsync(double?)` | Clamped to 0–1; null = indeterminate |
| `ValueTask HideAsync(int? fadeMs = null)` | Idempotent |

Every call is a best-effort no-op if `splash.js` was never referenced — forgetting the `<script>` tag must not take the app down at startup.

## `<SplashScreenHost />`

| Parameter | Type | Default | Description |
|---|---|---|---|
| AutoHide | bool | true | Hide once the app has rendered |
| Until | `Func<Task>?` | null | Startup work awaited before hiding; exceptions surface, but the splash is dismissed first |
| Delay | int | 0 | Extra ms to hold after ready |
| FadeDuration | int? | null | Overrides the fade configured at show time |
| Hidden | EventCallback | — | Raised after dismissal |

## MAUI Blazor Hybrid notes

There are **two** splashes in a Hybrid app and both need the same background colour or you get a white flash at the seam:

1. the native `MauiSplashScreen` (window creation → WebView ready),
2. this one (WebView → Blazor booted).

Match `<body>`'s background, the `BlazorWebView` background, and the native splash colour. Skip `blazorLoadProgress` — there is no runtime download to report.

## Accessibility

The host is `role="progressbar"` with `aria-live="polite"`, `aria-busy`, and `aria-valuenow` tracking determinate progress. Under `prefers-reduced-motion` every indicator degrades to a static shape rather than being animated to nothing, and the fade is disabled.
