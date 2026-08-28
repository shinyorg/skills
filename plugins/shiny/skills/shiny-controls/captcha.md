# Captcha

A human check in front of a form. One component over four providers, chosen **by name at registration** and swapped without touching the markup.

| | |
| --- | --- |
| Host | **Blazor only** — WebAssembly, Server, Hybrid |
| Package | `Shiny.Blazor.Controls` (core — no add-on) |
| Namespace | `Shiny.Blazor.Controls.Captchas` — add it to the **app's** `_Imports.razor`; the package's own imports do not reach consumers |

**There is no MAUI `Captcha`.** The hosted providers are browser widgets — a script that renders into a DOM element — and the local challenge draws to an HTML canvas. Never emit `<Captcha>` in XAML. A MAUI app hosting Blazor in a `BlazorWebView` uses the local challenge as-is; the hosted providers need a real origin their site key is registered for.

## A token is not a verdict

The component hands you a **response token** in `State.Response`. For the hosted providers it is your **server** that posts that token, with your *secret* key, to the provider's siteverify endpoint. Trusting `IsSolved` alone is trusting the client, which is the thing a captcha exists to avoid.

Never put a secret key in the client project — `UseReCaptcha`/`UseHCaptcha`/`UseTurnstile` take the **public site key** only. If asked for "the captcha key", that is the one.

## Setup

Registration is **optional**. With nothing registered, `<Captcha />` renders the local challenge with its defaults.

```csharp
// umbrella
builder.Services.AddShinyControls(cfg => cfg
    .ConfigureCaptcha(c => c.UseTurnstile("0x4AAA..."))
);

// or on its own
builder.Services.AddShinyCaptcha(c => c.UseTurnstile("0x4AAA..."));
```

Register as many as you like — the component picks one by name, and absent a name uses `SetDefaultProvider`, then the first registered, then the local challenge. That is what makes "Turnstile in production, local challenge in the dev build" a config change rather than a markup change.

```csharp
builder.Services.AddShinyCaptcha(c => c
    .UseTurnstile(siteKey)
    .UseLocal(o => o.Mode = LocalCaptchaMode.Math, name: "math")   // a second, named local challenge
    .SetDefaultProvider("turnstile")
    .SetTheme(CaptchaTheme.Auto)
    .SetSize(CaptchaSize.Normal)
    .SetLanguage("fr")
);
```

| Registration | Provider name | Notes |
| --- | --- | --- |
| `UseLocal(configure?, name?)` | `local`, or the name given | Self-hosted; the un-named one is also the fallback |
| `UseReCaptcha(siteKey)` | `recaptcha` | Google |
| `UseHCaptcha(siteKey)` | `hcaptcha` | |
| `UseTurnstile(siteKey)` | `turnstile` | Cloudflare; the only one supporting `Flexible` |
| `UseProvider<T>()` / `UseProvider(instance)` | whatever `Name` returns | |

A `Provider="..."` naming something that is **not** registered renders a visible "No captcha provider named … is registered" alert rather than silently dropping to a weaker check. That is a wiring mistake, not a fallback.

## Usage

```razor
<Captcha @ref="captcha" ValidChanged="v => canSubmit = v" />

<button disabled="@(!canSubmit)" @onclick="SubmitAsync">Sign up</button>

@code {
    Captcha? captcha;
    bool canSubmit;

    async Task SubmitAsync()
    {
        var token = this.captcha!.Response;   // hand this to your server
        // ... post the form ...

        // a spent token cannot be replayed — start a fresh challenge after a failed submit
        await this.captcha.ResetAsync();
    }
}
```

`ValidChanged` is the property to gate a submit button on. It flips both ways — a solved challenge that later **expires** flips it back — which polling `IsSolved` on render would miss.

## Server-side validation

`Validate` is called with the fresh token the moment the widget solves, and decides whether the state counts as valid. Return `false` and the component stays invalid — and, unless `ResetOnFailedValidation="false"`, throws the challenge away and starts a new one, because a token your server rejected is spent either way. A `Validate` that **throws** is treated as a rejection and the exception message is shown as the widget error.

```razor
<Captcha Validate="VerifyAsync" Solved="OnSolved" />

@code {
    void OnSolved(CaptchaState state) { /* Valid is already true here */ }

    async Task<bool> VerifyAsync(CaptchaState state)
    {
        // your endpoint holds the secret key and posts to the provider's siteverify
        var response = await http.PostAsJsonAsync("api/captcha/verify", new { state.Response });
        return response.IsSuccessStatusCode;
    }
}
```

`Solved` fires **after** `Validate` has agreed, so a handler on it can assume the check passed.

## Invisible mode

An invisible provider scores the session in the background and renders no challenge, so nothing ever solves on its own. Call `ExecuteAsync()` from the submit handler and continue in `Solved`.

```razor
<Captcha @ref="captcha" Size="CaptchaSize.Invisible" Solved="OnSolvedAsync" />

@code {
    Task SubmitAsync() => this.captcha!.ExecuteAsync();   // work continues in OnSolvedAsync
}
```

`BadgePosition` places the provider's badge — `BottomEnd` (default), `BottomStart`, or `Inline` to render it in the flow. The local provider has nothing to score and ignores `Invisible` entirely; `ExecuteAsync()` is a no-op for visible widgets.

## The local challenge

Self-hosted: no account, no site key, no third-party script, works offline and inside a `BlazorWebView`.

**Say this when suggesting it:** it is a speed bump, not a security boundary. The challenge is generated *and checked* in the browser, so anything with a debugger attached can read the answer out of memory. It stops naive form-fill bots and nothing more. For a public form worth attacking, register a hosted provider and verify the token on the server.

```csharp
builder.Services.AddShinyCaptcha(c => c.UseLocal(o =>
{
    o.Mode = LocalCaptchaMode.Math;
    o.ExpirySeconds = 60;
}));
```

| Option | Default | |
| --- | --- | --- |
| `Mode` | `Text` | `Text` draws distorted characters to a canvas; `Math` asks a small sum |
| `Length` | `5` | Characters in the text challenge, clamped to 3–12 |
| `CharacterSet` | `ABCDEFGHJKMNPQRSTUVWXYZ23456789` | Look-alikes (`0 O 1 I L`) already removed |
| `CaseSensitive` | `false` | |
| `Width` / `Height` | `180` / `60` | Canvas size in CSS pixels |
| `ExpirySeconds` | `120` | `Expired` then fires and the widget resets. `0` or less disables expiry |
| `MaxAttempts` | `3` | Wrong answers before the challenge is redrawn |
| `Prompt` / `IncorrectText` / `RefreshText` / `PlaceholderText` | — | Wording, for localisation |

`LocalCaptchaMode.Math` renders **real text**, not a canvas, so a screen reader can read it — that is the whole reason it exists. Prefer it, or offer it alongside, whenever accessibility is in scope. The answer is checked as soon as enough characters are typed (Enter forces a check), a wrong answer shakes the field, and `MaxAttempts` wrong answers redraw the challenge.

Its token is an opaque `local.<guid>` — the shape matches the hosted providers so the calling code does not change, but it proves only that *this browser* solved the challenge. There is nothing to verify it against, so do not write a `Validate` that pretends to.

## Parameters

| Parameter | Type | Default | |
| --- | --- | --- | --- |
| `Provider` | `string?` | `null` | Which registered provider. Null follows the configured default |
| `Theme` | `CaptchaTheme?` | configured | `Auto` (from `prefers-color-scheme`), `Light`, `Dark` |
| `Size` | `CaptchaSize?` | configured | `Normal`, `Compact`, `Invisible`, `Flexible` (**Turnstile only**, falls back to `Normal`) |
| `LanguageCode` | `string?` | `null` | Two-letter code. Null follows the browser |
| `BadgePosition` | `CaptchaBadgePosition` | `BottomEnd` | Invisible mode only |
| `ShowError` | `bool` | `true` | Renders widget failures under the widget |
| `ResetOnFailedValidation` | `bool` | `true` | |
| `Validate` | `Func<CaptchaState, Task<bool>>?` | `null` | |
| `CssClass` | `string?` | `null` | Extra classes on the host element; unmatched attributes splat onto it too |

**Events** — `Solved(CaptchaState)`, `Expired`, `Errored(string)` (script blocked, bad site key, network gone), `ValidChanged(bool)`.

**Members** — `State` (never null), `IsSolved`, `Response`, `ResetAsync()`, `ExecuteAsync()`.

`CaptchaState` is a record: `(bool Valid, string? Response, string ProviderName)`.

## A provider the package does not ship

The built-ins are only `ICaptchaProvider` implementations registered by name. For another hosted one, subclass `RemoteCaptchaProvider` and supply a descriptor — one shared JS driver does script loading, widget lifetime, callbacks, reset and execute.

```csharp
public class MyCaptchaProvider(string siteKey) : RemoteCaptchaProvider
{
    public override RemoteCaptchaDescriptor Descriptor { get; } = new()
    {
        Name = "mycaptcha",
        ScriptUrl = "https://example.com/api.js?render=explicit{lang}",  // {lang} is substituted or dropped
        GlobalName = "mycaptcha",
        SiteKey = siteKey,
        UseReadyCallback = false,          // true when the global exposes ready(cb), as reCAPTCHA does
        SupportsBadge = true,              // render() takes a badge option
        LanguageAsRenderOption = false,    // true puts the language in render() instead of the URL
        SupportedSizes = ["normal", "compact"]
    };
}

builder.Services.AddShinyCaptcha(c => c.UseProvider(new MyCaptchaProvider(siteKey)));
```

For something that is not a script-and-global widget at all, implement `ICaptchaProvider` directly: return a `RenderFragment` from `Render(CaptchaRenderContext)`, raise `OnSolved`/`OnExpired`/`OnErrored`, and call `OnWidgetReady(this)` with an `ICaptchaWidget` so `ResetAsync`/`ExecuteAsync` have something to talk to.

## Gotchas

- **Never ship a secret key to the client.** Site key in the app, secret key on the server.
- `IsSolved` alone is not verification for a hosted provider — pair it with `Validate` and a siteverify call.
- Reset after a failed submit. A token is single-use at the provider.
- Bind a submit button to `ValidChanged`, not to a value read once — challenges expire.
- The local challenge is client-side only; do not present it as protection for a public form.
- Blazor only. No `<Captcha>` in XAML, no `UseShinyCaptcha()` on `MauiAppBuilder`.
