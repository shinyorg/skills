# Session Interceptors

Interceptors replace v2's `IContextProvider`. They add tools and system prompts and react to turns.

## Interface

```csharp
public interface ISessionInterceptor
{
    Task OnSessionLoaded(SessionLoadedContext context) => Task.CompletedTask;        // Start AND every Get
    Task OnBeforeTurn(TurnContext context) => Task.CompletedTask;                    // this turn only
    Task OnAfterTurn(TurnContext context, AiTurnResult result) => Task.CompletedTask; // any outcome
    Task OnCompacted(CompactionContext context) => Task.CompletedTask;               // new rolling summary (Summary null after a hard trim)
}
```

Contexts (`SessionContextBase`) expose `Session`, `SystemPrompts` (`IList<string>`) and
`AddTool(AITool tool, AiToolFlags flags = AiToolFlags.None)`. `SessionLoadedContext.IsNew` is true
only on `Start`; `TurnContext` has `TurnId` and `Input`.

## Rules

- Tools and prompts are **never persisted**. `OnSessionLoaded` runs every time an instance is created
  (`Start`, `Get`, `GetLatest`), so add session-wide tools there, not only when `IsNew`.
- Use `context.Session.Settings.Profile` to choose tools/prompts per kind of session.
- Tools close over the session (`context.Session`) instead of resolving a global service.
- Exceptions are logged and never fail a turn (a failing `OnBeforeTurn` also raises `SessionError`).
- Prompt order sent to the model: session prompts (`OnSessionLoaded`), turn prompts (`OnBeforeTurn`),
  voice-mode instructions, rolling summary, then history ending with the new input.
- Register with `opts.AddInterceptor<T>()` (singleton). For simple static tools prefer
  `opts.AddTool(tool, flags, profiles)`.

## Example

```csharp
public class CookingInterceptor(IRecipeService recipes) : ISessionInterceptor
{
    public Task OnSessionLoaded(SessionLoadedContext context)
    {
        if (context.Session.Settings.Profile != "cooking")
            return Task.CompletedTask;

        context.SystemPrompts.Add("You are a friendly cooking assistant.");
        context.AddTool(AIFunctionFactory.Create(
            (string query) => recipes.Search(query),
            "search_recipes",
            "Searches the user's saved recipes"
        ));
        return Task.CompletedTask;
    }

    public async Task OnBeforeTurn(TurnContext context)
    {
        var pantry = await recipes.GetPantry();
        context.SystemPrompts.Add($"Ingredients the user has: {String.Join(", ", pantry)}");
    }

    public async Task OnAfterTurn(TurnContext context, AiTurnResult result)
    {
        // e.g. name the session after its first exchange
        if (context.Session.Title == null && result.Outcome != AiTurnOutcome.Failed && context.Input.Parts[0] is TextPart t)
            await context.Session.SetTitle(t.Text.Length > 40 ? t.Text[..40] : t.Text);
    }
}
```

## AiToolFlags

- `Silent` - the tool never triggers a spoken "one moment" acknowledgement and does not move the
  session to `Thinking`; surfaced as `ToolCallStarted.Silent`. The built-in `ask_user` is silent.

## Built-in

`DefaultSessionInterceptor` is always registered: it adds `opts.AddTool` registrations (respecting
profiles) and plain `AITool` DI singletons to each session, and adds the current local time as a
system prompt every turn.
