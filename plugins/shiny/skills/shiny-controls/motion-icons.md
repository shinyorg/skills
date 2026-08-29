# Motion Icons

Animated icons that run on a timer, on hover, on tap, when they scroll into view, or on command. 111 built-in icons, each with motion drawn for it, plus generic presets that work on any artwork including your own.

**Both hosts, core packages.** `MotionIconView` is in `Shiny.Maui.Controls`; `<MotionIcon>` is in `Shiny.Blazor.Controls`. There is no separate add-on package to install. The artwork and motion definitions live in `Shiny.Controls.MotionIcons.Shared`, which both core packages reference; the MAUI side also builds on `Shiny.Controls.Keyframe.Shared`, so an icon is a `KeyframeScene` played by the same `Player` a hand-written timeline uses.

**Every MAUI head.** Nothing touches a platform SDK — the view is a `GraphicsView` and the clock is an `IDispatcherTimer` — so iOS, Android, Windows, MacCatalyst, macOS AppKit and Linux GTK4 all work. Never gate motion-icon code behind a platform check.

## The one thing that actually differs between the hosts

Both hosts compile the **same** `MotionSpec`, but into different machinery:

| | MAUI | Blazor |
|---|---|---|
| Runs the animation | a `KeyframeScene` driven by the Keyframe engine's `Player`, on one shared timer per window | the browser, from generated `@keyframes` |
| Cost per frame | evaluate the spec + redraw | nothing — no C# runs at all |
| Stopping | `Player.Stop()` restores the captured baselines | drop a CSS class |

Generate code against the properties, not the machinery. The parameter names are deliberately identical on both sides.

## MAUI

```xml
xmlns:shiny="http://shiny.net/maui/controls"

<shiny:MotionIconView Icon="bell"
                      Trigger="Loop"
                      Interval="0:0:1.5"
                      Color="{AppThemeBinding Light=#2563EB, Dark=#60A5FA}"
                      WidthRequest="32"
                      HeightRequest="32" />
```

## Blazor

```razor
<MotionIcon Icon="bell"
            Trigger="MotionTrigger.Loop"
            Interval="TimeSpan.FromSeconds(1.5)"
            Size="32" />
```

`Color` defaults to `currentColor`, so an icon inside a button or a link inherits its colour — including hover and disabled states — with nothing to wire up. Only set `Color` when you want to override that.

## Properties

Identical on both hosts unless noted.

| Property | Type | Notes |
|---|---|---|
| `Icon` | `string` | A built-in name. Unknown names render nothing rather than throwing. |
| `Source` | `MotionIconDefinition` | Explicit artwork. Beats `Icon` and `PathData`. |
| `PathData` | `string` | Raw SVG path in a 24x24 box — the quickest way to animate your own glyph. |
| `Motion` | `MotionPreset` | Defaults to `Default`. |
| `Trigger` | `MotionTrigger` | `[Flags]`. Defaults to `Hover \| Press`. |
| `Duration` | `TimeSpan` | Overrides one cycle. Zero uses the motion's own. |
| `Interval` | `TimeSpan` | Rests between cycles while looping. |
| `Speed` | `double` | Rate multiplier. Negative reverses **on MAUI only**. |
| `RepeatCount` | `int` | Cycles per triggered play. Zero or less repeats forever. |
| `Color` | `Color` (MAUI) / `string` (Blazor) | MAUI: unset follows the theme pack's on-surface token. Blazor: `currentColor`. |
| `AccentColor` | same | Secondary colour for two-tone artwork. Falls back to `Color`. |
| `StrokeWidth` | `double` | In the icon's own 24-unit space. The set is drawn for `2`. |
| `IsPlaying` | `bool` | Two-way. **This is the one to bind to a busy flag.** |
| `Size` | `double` | Blazor only. MAUI uses `WidthRequest`/`HeightRequest` (both default 24). |
| `Progress` | `double` | MAUI only. Two-way — bind a `Slider` to scrub. |
| `Command` | `ICommand` | MAUI only. Blazor has `OnClick`. |
| `Title` | `string` | Blazor only. Without it the icon is `aria-hidden`. |

Methods on both: `Play()`, `Stop()`, `StopAtCycleEnd()`. MAUI adds `PlayAsync()` and `Reset()`; Blazor adds `Loop()`. Both raise `Completed`.

## Triggers

```csharp
[Flags] enum MotionTrigger { Manual = 0, Loop = 1, Hover = 2, Press = 4, Appear = 8 }
```

- **`Loop`** — runs continuously, resting for `Interval` between cycles.
- **`Hover`** — runs while the pointer is over it, then **finishes the cycle in progress** so it never snaps back mid-pose. Mouse only; pair it with `Press` for touch.
- **`Press`** — one play per tap or click. **Only presses that land on the icon itself count** — an
  icon sitting inside a larger tap target (a button, a row) should be `Manual` and played by its
  host, which is what `ShinyButton` does with its slot icons on both hosts.
- **`Appear`** — plays once when it first becomes visible. On the web this is an `IntersectionObserver`.
- **`Manual`** — nothing automatic; drive it with `Play()` or by binding `IsPlaying`.

For a busy indicator, bind `IsPlaying` rather than toggling `Trigger`:

```xml
<shiny:MotionIconView Icon="loader" Trigger="Manual" IsPlaying="{Binding IsBusy}" />
```

## Presets

`Motion="…"` — works on **any** icon, including artwork the library has never seen:

`Default`, `None`, `Pulse`, `Beat`, `Spin`, `Shake`, `Wobble`, `Bounce`, `Float`, `Pop`, `Tada`, `Flip`, `Swing`, `Blink`, `Draw`, `Nudge`, `Jiggle`.

`Default` plays the motion drawn for that icon, falling back to `Pulse` for artwork that has none — which is why a bell rings rather than merely pulsing. `Draw` is the only preset that reaches into the parts, so it can stagger them.

## The icon set

Names are one flat, case-insensitive namespace; the grouping below is only to make it readable.

- **Actions** — `add-user`, `attach`, `check`, `close`, `copy`, `download`, `edit`, `filter`, `link`, `loader`, `logout`, `menu`, `more`, `pin`, `plus`, `power`, `print`, `redo`, `refresh`, `save`, `search`, `send`, `settings`, `share`, `sort`, `trash`, `undo`, `upload`, `zoom-in`, `zoom-out`
- **Navigation** — `arrow-down`, `arrow-left`, `arrow-right`, `arrow-up`, `chevron-down`, `chevron-left`, `chevron-right`, `chevron-up`, `collapse`, `compass`, `expand`, `external-link`
- **Objects** — `battery`, `bell`, `bookmark`, `calendar`, `camera`, `cart`, `clock`, `cloud`, `coffee`, `credit-card`, `eye`, `flag`, `gift`, `heart`, `home`, `lightbulb`, `location`, `lock`, `mail`, `message`, `rocket`, `shield`, `star`, `sun`, `tag`, `user`
- **Media** — `headphones`, `microphone`, `music`, `mute`, `pause`, `play`, `record`, `repeat`, `shuffle`, `skip-back`, `skip-forward`, `stop`, `video`, `volume`
- **Files** — `book`, `clipboard`, `database`, `file`, `file-text`, `folder`, `folder-open`, `image`
- **Weather** — `cloud-rain`, `cloud-snow`, `droplet`, `lightning`, `moon`, `thermometer`, `umbrella`, `wind`
- **Indicators** — `activity`, `bluetooth`, `chart`, `check-circle`, `help`, `hourglass`, `info`, `signal`, `thumbs-up`, `trending-up`, `warning`, `wifi`, `x-circle`

Directional icons come in matched sets: every arrow travels the way it points and pulls its shaft in
behind the head, and every chevron bounces once in its own direction, so swapping `arrow-right` for
`arrow-left` in a right-to-left layout gets the mirrored motion for free.

**Never invent a name.** An unknown one renders nothing and throws nothing, so a typo is invisible
until someone looks at the screen. Enumerate at runtime with `MotionIconLibrary.Names` / `.All`.

## Your own artwork

Quickest — a single path:

```xml
<shiny:MotionIconView PathData="M12 2L3 20H21z" Motion="Pop" />
```

Split into parts when pieces move differently — this is what makes a hinged icon possible:

```csharp
var icon = new MotionIconDefinition(
    "toggle",
    [
        new MotionIconPart("plate", "M3 8h18v8H3z"),
        new MotionIconPart("knob", "M11 12a3 3 0 1 1 6 0 3 3 0 0 1-6 0z") { Origin = new MotionPoint(14f, 12f) }
    ],
    MotionSpecBuilder.Build(500, m => m
        .MoveX("knob", k => k.At(0d, 0d, MotionEase.BackOut).At(0.5d, -6d).At(1d, 0d))));

MotionIconLibrary.Register(icon);   // now available as Icon="toggle" everywhere
```

`MotionIconLibrary.Register` also **replaces** a built-in, so an app with its own visual language can swap the artwork for `check` once at startup instead of passing a definition in at every call site.

## Authoring rules that actually matter

1. **Write path data with explicit `L` commands.** `Microsoft.Maui.Graphics` does not implement SVG's implicit-lineto rule: `"M6 6 18 18"` becomes two *movetos* and draws nothing. Write `"M6 6L18 18"`. It also cannot read run-together decimals (`l.06.06` — write `l.06 .06`). Browsers handle both, so artwork pasted from a design tool can look perfect on Blazor and render as a dot on MAUI.
2. **End every track at its resting value.** Stopping reverts to the artwork as drawn, so a track that finishes anywhere else ends with a visible jump. A reveal may *start* elsewhere — a check starts undrawn, a pin starts above the icon — it just has to land home. Rotation is exempt when the artwork is symmetric under the angle it stops at.
3. **Set `Origin` on any part that is hinged.** The default pivot is the icon's centre, which is right for a spin and wrong for a bell (crown), a trash lid (hinge) or a growing bar (baseline).
4. **Don't mix `Scale` with `ScaleX`/`ScaleY` on the same part** — the two hosts resolve the conflict differently. Use one or the other.
5. **Rotate a part about its own centre and translate afterwards**, not the reverse — that is the order both hosts apply transforms, and it is what makes the hamburger's bars cross rather than swing round the outside.
6. **Prefer transform and opacity channels.** `Trim` rebuilds geometry per frame on MAUI; it is worth it for a draw-on and wasteful for anything else.
7. **`Interval` is folded into the animation, not run off a timer**, so a looping icon needs nothing but `animation-iteration-count: infinite` on the web. Don't try to schedule replays yourself.
8. **Give Blazor icons a `Title`** when they carry meaning; without one they are correctly hidden from screen readers as decoration.
9. **Nothing plays before the view is loaded on MAUI.** Setting `IsPlaying` from an implicit `Style` — which MAUI applies from inside the base constructor — records the intent and starts it at `Loaded`, so it never reaches for a dispatcher that does not exist yet.

## Channels

If you are hand-authoring a `MotionSpec`: `Opacity`, `TranslateX`, `TranslateY`, `Rotate`, `Scale`, `ScaleX`, `ScaleY`, `StrokeWidth` (a multiplier on the host's), `Trim` (fraction of the stroke drawn), plus colour tracks for `Fill` and `Stroke`.

There is deliberately **no path-morph channel**. Every channel here behaves identically on both hosts; morphing geometry would need hand-written fallbacks the moment anyone opened Firefox. Hinged and "morphing" icons are built from separate parts moved by transforms instead, exactly as they would be in a design tool.

Easing is a closed enum (`MotionEase`) rather than a delegate, because a spec has to survive being compiled into CSS. The curves match `Shiny.Controls.Keyframe.Easings` term for term, so an icon and a hand-written keyframe timeline beside it share a visual language.
