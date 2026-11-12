# AG-UI & A2UI (`Shiny.AiConversation.AgUi`)

NuGet: `Shiny.AiConversation.AgUi`. Depends on the official AG-UI .NET SDK (`AGUI.Abstractions`, `AGUI.Client`).
Namespaces: `Shiny.AiConversation.AgUi` (types) and `Shiny` (registration extensions).

Three things:

1. **A session as an AG-UI agent**: `IAiSession.RunAgUiAsync(RunAgentInput, CancellationToken)` returns
   `IAsyncEnumerable<BaseEvent>`, which is one AG-UI run. The signature matches the Shiny controls'
   `IAgentSession.RunAsync`, so the `AgentView` (Shiny.Maui.Controls.Agents / Shiny.Blazor.Controls.Agents)
   takes a session in one line.
2. **`render_ui`**: a tool that lets the model draw A2UI v1.0 surfaces. Each call becomes an `a2ui-surface` activity.
3. **`AddAgUiChatClient(uri)`**: a remote AG-UI agent as the sessions' `IChatClient`.

## Render a session in AgentView

```csharp
// MauiProgram / Program.cs
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddAgUiRenderTool();          // optional: the model can draw A2UI surfaces (Shiny catalog by default)
});

// page / view model
var session = await sessionManager.Start();
agentView.Session = AgentSession.Create(session.SessionId, session.RunAgUiAsync);
```

```razor
<AgentView Session="@(AgentSession.Create(session.SessionId, session.RunAgUiAsync))" />
```

## Sample

`samples/BlazorSample/Pages/Agent.razor` in the Shiny speech repo is a working host:

- the page starts its own session with `new AiSessionSettings { Profile = "agent" }`, separate from the session the chat/voice pages use;
- `Program.cs` registers `opts.AddAgUiRenderTool(catalogInstructions: AgentCatalogPrompt.Build(), profiles: "agent")`, so only those sessions get `render_ui`;
- `Services/AgentCatalogPrompt.cs` builds a compact catalog summary from `A2UICatalog.CreateShiny()`: the basic catalog's rules, the Shiny instructions, and one line of properties per component. The full schema is about 58 KB, which is too much to send with every request;
- the page links `_content/Shiny.Blazor.Controls/css/shiny-theme.css` and the app's `{Assembly}.styles.css` (AgentView uses scoped CSS) through `<HeadContent>`, and wraps the view in `shiny-theme-dark` to match the sample's dark shell.

The sample references `Shiny.Blazor.Controls.Agents` from a sibling `controls` checkout (MSBuild property `ShinyControlsRepo`, constant `SHINY_AGENTS`) until the package ships on NuGet.

## Mapping

| Session | AG-UI |
|---|---|
| `TextDelta` | `TEXT_MESSAGE_START` / `CONTENT` / `END` (a tool call closes the open message) |
| `ToolCallStarted` / `ToolCallCompleted` | `TOOL_CALL_START` / `ARGS` / `END`, `TOOL_CALL_RESULT` |
| `render_ui` call | also an `ACTIVITY_SNAPSHOT` (`activityType: "a2ui-surface"`, `content.a2ui_operations`) |
| Turn ends `AwaitingUser` (`ask_user`) | `RUN_FINISHED` with `outcome: interrupt`. One interrupt per `AiQuestion`: `id` = question id, `reason: "ask_user"`, `message` = question text, `metadata: { choices: [{id,label}], allowMultiple }`, `responseSchema` asks for `choiceIds` (or `text` when free-form) |
| `TurnInterrupted` | `RUN_FINISHED` with `outcome: cancelled` |
| `TurnFailed` | `RUN_ERROR` |

Input: the session keeps its own history, so only the newest input is read from the run:
- the last user message → `AiInput.Text`;
- `forwardedProps.a2uiAction.userAction` (an A2UI button) → a displayed "▸ name" line plus a model-only description with the action's context;
- `resume` → the answers. Payload `{"choiceIds": [...]}` becomes `AiInput.Answer`, `{"text": "..."}` becomes an answer with that text, and a cancelled resume becomes "(dismissed)".

Cancelling the run's token calls `turn.Interrupt()`.

Frontend tools sent in `RunAgentInput.tools` are **not** offered to the model. Session tools are .NET `AITool`s
registered with `opts.AddTool(...)` or interceptors.

## render_ui

```csharp
opts.AddAgUiRenderTool();                                            // Shiny catalog id
opts.AddAgUiRenderTool(AgUiRenderTool.BasicCatalogId);               // basic catalog only
opts.AddAgUiRenderTool(catalogInstructions: myCatalogSummary);       // tell the model what it may place
```

- The tool is `Silent`.
- The arguments are `{ "operations": [ A2UI v1.0 messages ] }`.
- It returns a short "the UI is on screen" result.
- Give the model the catalog: pass `catalogInstructions` (e.g. the catalog document's `instructions` plus its component names). For the Shiny catalog, `A2UICatalog.CreateShiny().Definition` in Shiny.Controls.AgentUI.Shared has the full schema.

## Remote AG-UI agent as the chat client

```csharp
opts.AddAgUiChatClient(new Uri("https://my-agent.example.com/agent"));
```

This uses the SDK's `AGUIChatClient` over HTTP + SSE. The remote agent runs its own tools. The session still stores history, memory and compaction.

## Rules
- `RunAgUiAsync` starts a turn, and a new `Send` supersedes an active turn. Don't drive the same session from voice and AgentView at once.
- Use `AgentSession.Create(session.SessionId, session.RunAgUiAsync)`. Don't write your own `IAgentSession` adapter.
- `AiChatView` (`Shiny.AiConversation.Maui`) is frozen. Use `AgentView` for new agent UIs. Keep `AiChatView` for the voice-first ChatView experience.
