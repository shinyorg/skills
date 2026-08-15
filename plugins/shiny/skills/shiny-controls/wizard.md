# StateView & Wizard

Two related controls, on **both MAUI and Blazor**.

- **`StateView`** shows exactly one of several named branches, chosen by a string. It is the declarative
  form of the `IsVisible` (MAUI) / `@if/else` (Blazor) ladder every app grows.
- **`Wizard`** builds on the same model: named branches, plus an order, a progress indicator, a Back/Next
  bar that knows where it is, and a gate on leaving a step.

Both live in the **core** packages (`Shiny.Maui.Controls`, `Shiny.Blazor.Controls`) — there is nothing extra
to install or register.

---

## StateView

### MAUI

`StateView` derives from `ContentView`. Its `ContentProperty` is `States`, so the states are written as
direct children.

```xml
<shiny:StateView CurrentState="{Binding CurrentState}" Transition="Slide">
    <shiny:StateViewState Name="Empty">
        <Label Text="Nothing loaded" />
    </shiny:StateViewState>

    <shiny:StateViewState Name="Loading">
        <ActivityIndicator IsRunning="True" />
    </shiny:StateViewState>

    <!-- Built the first time this branch is reached, then cached -->
    <shiny:StateViewState Name="Loaded">
        <shiny:StateViewState.ContentTemplate>
            <DataTemplate>
                <local:ExpensiveReportView />
            </DataTemplate>
        </shiny:StateViewState.ContentTemplate>
    </shiny:StateViewState>

    <shiny:StateViewState Name="Error">
        <Button Text="Try again" Command="{Binding RetryCommand}" />
    </shiny:StateViewState>
</shiny:StateView>
```

`StateViewState`'s `ContentProperty` is `Content`, so a single child view needs no wrapper element. MAUI
allows one view per state — wrap several in a layout.

### Blazor

The states go inside `<States>`; each `StateViewState` renders nothing itself and hands its `ChildContent`
to the state view, so a branch you never reach is never built.

```razor
<StateView @bind-CurrentState="state" Transition="StateTransition.Slide">
    <States>
        <StateViewState Name="Empty"><p>Nothing loaded</p></StateViewState>
        <StateViewState Name="Loading"><ProgressBar IsIndeterminate="true" /></StateViewState>
        <StateViewState Name="Loaded"><ExpensiveReport /></StateViewState>
        <StateViewState Name="Error"><button @onclick="RetryAsync">Try again</button></StateViewState>
    </States>
</StateView>
```

`ChildContent` is accepted as an alias for `States`, so the wrapper tag can be dropped when nothing else is
being passed.

### Matching and fallback

`CurrentState` is matched against `StateViewState.Name` **ordinally and case-insensitively**. An empty or
unmatched name falls back to `DefaultState`, then to the **first declared state** — a typo shows something
rather than a blank rectangle. `EmptyView` (MAUI) / `EmptyContent` (Blazor) covers the case where there are
no states at all.

### Transitions

| Value | Behaviour |
|---|---|
| `None` | Swap instantly |
| `Fade` | Fade in (MAUI cross-fades) |
| `Slide` | Direction taken from the move — a later state enters from the right, an earlier one from the left |
| `SlideLeft` / `SlideRight` | Always as if moving forwards / backwards |
| `SlideUp` / `SlideDown` | Vertical |
| `Scale` | Fade while growing into place |

`TransitionDuration` is milliseconds (MAUI `uint`, Blazor `int`); zero swaps instantly. Blazor animates the
**incoming** branch only — rendering the outgoing one as well would mean every component inside a state
existing twice for the duration, with duplicated timers, JS interop and form state.

### StateView properties

| Property | MAUI | Blazor | Default | Description |
|---|---|---|---|---|
| CurrentState | ✅ two-way | ✅ `@bind-CurrentState` | null | The state to show |
| DefaultState | ✅ | ✅ | null | Fallback when `CurrentState` is empty or unmatched |
| Transition | ✅ | ✅ | `Fade` | How the swap animates |
| TransitionDuration | ✅ `uint` | ✅ `int` | 200 | Milliseconds |
| TransitionEasing | ✅ | — | `CubicOut` | MAUI only |
| CacheContent | ✅ | — | true | Keep a `ContentTemplate`-built view alive after its state is left |
| EmptyView / EmptyContent | ✅ `View` | ✅ `RenderFragment` | null | Shown when nothing matches at all |
| States | ✅ `IList<StateViewState>` | ✅ `RenderFragment` | — | The branches |
| StateChangedCommand | ✅ | — | null | Invoked with the new state name |
| CurrentStateView / Current | ✅ | ✅ | — | Read-only: the state on screen |
| CurrentStateIndex / CurrentIndex | ✅ | ✅ | -1 | Read-only |

MAUI adds `GoTo(string)` / `GoTo(int)` and a `StateChanged` event; Blazor adds `GoTo(string)` / `GoTo(int)`
and the `CurrentStateChanged` callback.

### StateViewState properties

| Property | MAUI | Blazor | Description |
|---|---|---|---|
| Name | ✅ | ✅ | What `CurrentState` is matched against |
| Content | ✅ `View` (ContentProperty) | — | Built eagerly with the rest of the markup |
| ContentTemplate | ✅ `DataTemplate` | — | Built on first show, then cached; wins over `Content` |
| ChildContent | — | ✅ | Rendered by the host while this is the current state (lazy by construction) |

---

## Wizard

`WizardStep` **is** a `StateViewState` with extra rules, so everything above about naming, matching and
content applies.

### MAUI

```xml
<shiny:Wizard x:Name="Checkout"
              CurrentStep="{Binding CurrentStep}"
              ShowCancel="True"
              AllowStepSelection="True"
              ProgressStyle="Chevron"
              Transition="Slide"
              FinishedCommand="{Binding SubmitCommand}"
              CancelledCommand="{Binding AbandonCommand}">

    <shiny:WizardStep Name="Account" Title="Account" IsValid="{Binding EmailIsValid}">
        <shiny:TextEntry Text="{Binding Email}" Placeholder="you@example.com" />
    </shiny:WizardStep>

    <!-- IsVisible=False takes the step out of the run entirely -->
    <shiny:WizardStep Name="Delivery" Title="Delivery"
                      IsVisible="{Binding WantsDelivery}" IsOptional="True">
        <shiny:TextEntry Text="{Binding Address}" />
    </shiny:WizardStep>

    <shiny:WizardStep Name="Review" Title="Review" NextText="Place order">
        <Button Text="Start over"
                Command="{Binding Source={x:Reference Checkout}, Path=GoToStepCommand}"
                CommandParameter="Account" />
    </shiny:WizardStep>
</shiny:Wizard>
```

`Steps` is the `ContentProperty`. A custom progress indicator goes in `<shiny:Wizard.Progress>` and a custom
navigation bar in `<shiny:Wizard.NavigationBar>`.

### Blazor

```razor
<Wizard @bind-CurrentStep="step"
        ShowCancel="true"
        AllowStepSelection="true"
        ProgressStyle="WizardProgressStyle.Chevron"
        Finished="OnFinished"
        Cancelled="OnCancelled">
    <Steps>
        <WizardStep Name="Account" Title="Account" IsValid="@emailIsValid">…</WizardStep>
        <WizardStep Name="Delivery" Title="Delivery" IsVisible="@wantsDelivery" IsOptional="true">…</WizardStep>
        <WizardStep Name="Review" Title="Review" NextText="Place order" Validate="ConfirmAsync">…</WizardStep>
    </Steps>
    <Progress>
        <!-- optional: replaces the built-in pointed progress bar -->
    </Progress>
    <NavigationBar>
        <!-- optional: replaces the built-in Back/Next bar -->
    </NavigationBar>
</Wizard>
```

### Validation — pick the cheapest thing that works

1. **`IsValid`** — a bool the step exposes. Bind it to a view-model validity property. Next is disabled and
   refuses to move while it is false.
2. **`IsOptional`** — bypasses `IsValid` entirely for that step.
3. **`ValidateCommand` (MAUI)** — run *before* `IsValid` is read, so a command that validates and sets the
   flag is enough; no event wiring:
   ```csharp
   step.ValidateCommand = new Command(() => step.IsValid = Validate());
   ```
4. **`Validate` (Blazor)** — `Func<Task<bool>>`; returning false keeps the wizard where it is. Async, so a
   server round-trip is a first-class validator.
5. **`StepChanging`** — cancellable, and carries `From`, `To` and `Direction`. For anything the above cannot
   express.

### Conditional and disabled steps

- `IsVisible="False"` removes a step from the run: skipped by Next/Back, dropped from the progress
  indicator, excluded from `StepCount`, and not reachable by name. Bind it and the wizard reshapes itself.
  If the step being hidden is the current one, the wizard moves to the nearest still-visible step rather
  than blanking.
- `IsEnabled="False"` keeps the step drawn (dimmed) but unreachable.

### Progress indicators

`ProgressStyle` is `Chevron` (default), `Dots`, `Bar` or `None`; `ProgressPosition` is `Top` (default),
`Bottom` or `None`. Setting `Progress` replaces all of it with your own view.

- **Chevron** — the pointed breadcrumb, one segment per visible step carrying its title. Completed, current
  and upcoming take `PrimaryContainer`, `Primary` and `SurfaceContainerHighest` from the theme. On MAUI it
  is drawn on a `GraphicsView`, so it renders identically on every head including AppKit and GTK4; on
  Blazor the same shape is a CSS `clip-path`.
- **Dots** — numbered markers joined by a connector, with a tick on completed steps.
- **Bar** — a filled track under a "Step 2 of 5 — Delivery" caption.

`ShowStepTitles="False"` drops to a compact numbered strip. `AllowStepSelection` makes the indicator
clickable; `LinearNavigation` (on by default) limits that to steps already completed, so the user can
review without skipping ahead. Programmatic `GoTo` is never restricted by `LinearNavigation`.

### Navigation from your own markup

The wizard owns commands so a step's own buttons do not need the view-model to re-implement navigation:

| Member | MAUI | Blazor |
|---|---|---|
| Forward, or finish on the last step | `GoNextCommand` / `GoNext()` | `GoNextAsync()` |
| Back a step | `GoBackCommand` / `GoBack()` | `GoBackAsync()` |
| Finish from anywhere | `FinishCommand` / `Finish()` | `FinishAsync()` |
| Abandon | `CancelCommand` / `Cancel()` | `CancelAsync()` |
| Jump (name or visible index) | `GoToStepCommand` / `GoTo(...)` | `GoToAsync(...)` |
| Clear completion, back to the start | `Reset()` | `ResetAsync()` |

On MAUI, reach them from inside a step with `{Binding Source={x:Reference MyWizard}, Path=GoNextCommand}`.
On Blazor, take an `@ref` to the wizard.

### Wizard properties

| Property | MAUI | Blazor | Default | Description |
|---|---|---|---|---|
| Steps | ✅ `IList<WizardStep>` (ContentProperty) | ✅ `RenderFragment` | — | The steps |
| CurrentStep | ✅ two-way | ✅ `@bind-CurrentStep` | null | Name of the step on screen |
| CurrentStepIndex | ✅ two-way | ✅ two-way | -1 | Index among **visible** steps |
| CanGoBack / CanGoNext | ✅ | ✅ | true | Consumer gates, ANDed with the wizard's own checks |
| CanCancel | ✅ | ✅ | true | |
| AllowStepSelection | ✅ | ✅ | false | Clickable progress indicator |
| LinearNavigation | ✅ | ✅ | true | Restrict clicks to completed steps |
| Progress | ✅ `View` | ✅ `RenderFragment` | null | Replaces the built-in indicator |
| ProgressStyle | ✅ | ✅ | `Chevron` | `Chevron` / `Dots` / `Bar` / `None` |
| ProgressPosition | ✅ | ✅ | `Top` | `Top` / `Bottom` / `None` |
| ProgressHeight | ✅ | — | 44 | MAUI reserves a fixed height for the drawn indicator |
| ShowStepTitles | ✅ | ✅ | true | |
| NavigationBar | ✅ `View` | ✅ `RenderFragment` | null | Replaces the built-in Back/Next bar |
| ShowNavigationBar | ✅ | ✅ | true | Turn off when steps carry their own buttons |
| ShowCancel | ✅ | ✅ | false | |
| ShowBackOnFirstStep | ✅ | ✅ | false | Keep Back on screen (disabled) so the bar does not reflow |
| BackText / NextText / FinishText / CancelText | ✅ | ✅ | Back / Next / Finish / Cancel | |
| Transition / TransitionDuration | ✅ | ✅ | `Slide` / 220 | Same values as `StateView` |
| StepCount / StepNumber / IsFirstStep / IsLastStep / ProgressFraction | ✅ read-only bindable | ✅ read-only | — | Position |
| CurrentStepItem | ✅ | ✅ | — | The `WizardStep` on screen |

**Events / callbacks:** `StepChanging` (cancellable), `StepChanged`, `Finishing` (cancellable), `Finished`,
`Cancelled`. MAUI also exposes `StepChangedCommand`, `FinishedCommand` and `CancelledCommand` for
view-model binding.

### WizardStep properties

| Property | MAUI | Blazor | Default | Description |
|---|---|---|---|---|
| Name | ✅ | ✅ | null | Identity, and what `CurrentStep` matches |
| Title | ✅ | ✅ | null | Shown on the indicator; falls back to `Name` |
| Description | ✅ | ✅ | null | Sub-caption for the `Bar` indicator |
| IsVisible | ✅ | ✅ | true | False takes the step out of the run |
| IsEnabled | ✅ | ✅ | true | False leaves it drawn but unreachable |
| IsValid | ✅ | ✅ | true | Gates Next |
| IsOptional | ✅ | ✅ | false | Bypasses `IsValid` |
| IsCompleted | ✅ two-way | ✅ two-way | false | Set by the wizard on the way forward |
| NextText / BackText | ✅ | ✅ | null | Per-step button overrides |
| ValidateCommand | ✅ | — | null | Runs before `IsValid` is read |
| Validate | — | ✅ `Func<Task<bool>>` | null | Async gate on leaving forwards |

---

## Gotchas

- **Assigning an unknown or disabled step name to `CurrentStep` is reverted**, not honoured — the wizard
  puts the property back to where it actually is so a two-way binding reflects reality rather than blanking.
- **`CurrentStepIndex` counts visible steps only**, so it shifts when a conditional step appears or
  disappears. Bind `CurrentStep` when you want a stable identity.
- **`IsCompleted` is set only on forward moves.** Going back does not un-complete the step you came from;
  `Reset()` clears every step.
- **MAUI allows one view per `StateViewState`/`WizardStep`** — wrap several children in a layout. Blazor's
  `ChildContent` takes as many as you like.
- **A `ContentTemplate` branch is cached by default.** Set `CacheContent="False"` on the `StateView` when
  entering a branch should reset it.
