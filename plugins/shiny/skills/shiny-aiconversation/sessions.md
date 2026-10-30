# Sessions, Turns & Events

## IAiSessionManager

```csharp
public interface IAiSessionManager
{
    IAiMemoryService Memory { get; }                 // see memory.md
    Task<IAiSession> Start(AiSessionSettings? settings = null, CancellationToken ct = default);
    Task<IAiSession?> Get(string sessionId, CancellationToken ct = default);
    Task<IAiSession?> GetLatest(CancellationToken ct = default);          // most recently active
    Task<IReadOnlyList<AiSessionDocument>> Query(SessionQuery query, CancellationToken ct = default);  // headers only
    Task Delete(string sessionId, CancellationToken ct = default);        // cascades messages
}
```

- Stateless: no cache, no "current" session. Every `Get` / `GetLatest` returns a **new instance**
  rehydrated from the store (header + non-compacted messages, pending questions restored, context
  re-estimated). The caller owns and disposes it.
- A session interrupted by a crash mid-reply is repaired lazily on load (its streaming reply is marked
  `Interrupted`).
- Session list UI: `Query(new SessionQuery(Text: "recipe", Take: 20))` returns `AiSessionDocument`
  headers (`Title`, `LastActivity`, `Usage`, `MessageCount`, ...) without loading sessions.

## IAiSession

```csharp
public interface IAiSession : IAsyncDisposable
{
    string SessionId { get; }
    string? Title { get; }
    DateTimeOffset Started { get; }
    AiSessionSettings Settings { get; }
    AiSessionState State { get; }                     // Idle, Thinking, Responding, AwaitingUser, Compacting
    IReadOnlyList<AiQuestion> PendingQuestions { get; }
    SessionUsage Usage { get; }                       // cumulative billed tokens + RequestCount
    ContextSnapshot Context { get; }                  // CurrentTokens, MaxTokens, PercentUsed, Pressure, IsEstimate
    IAiTurn? ActiveTurn { get; }

    IAsyncEnumerable<AiSessionEvent> GetEvents(CancellationToken ct = default);
    IAiTurn Send(AiInput input, CancellationToken ct = default);
    Task<MessagePage> GetMessages(long? beforeSequence, int count, CancellationToken ct = default);
    Task Compact(CancellationToken ct = default);     // see compaction.md
    Task SetTitle(string title, CancellationToken ct = default);
    Task UpdateSettings(Func<AiSessionSettings, AiSessionSettings> update, CancellationToken ct = default);
}
```

## Input

```csharp
session.Send(AiInput.Text("What can I cook with eggs?"));
session.Send(AiInput.Voice(heardText));          // tells the model its reply is spoken: plain prose, one question max
session.Send(AiInput.Answer(question, "large")); // see questions.md

// extra system instructions for this turn only (not stored with the message)
session.Send(AiInput.Text(text).WithInstructions("Answer in French."));
```

`AiInput.Instructions` (`IReadOnlyList<string>`, default empty) are added to the system prompt for that turn
only, after the mode instructions; they are never persisted. The voice conversation uses this for
`AiAcknowledgement.LessWordy`.

```csharp
// images: stored via IAiAttachmentStore, referenced from the user message by an ImagePart
session.Send(AiInput.Text("What is this?").WithImage(bytes, "image/jpeg", "the plant", ImageRetention.DropAfterTurn));
```

See `images.md` for retention and attachment stores.

### Follow-ups within a turn

A tool added in `OnBeforeTurn` can call `TurnContext.QueueFollowUp(AiInput)`. When the model round ends (e.g. the
tool set `FunctionInvokingChatClient.CurrentContext.Terminate = true`), the session stores the reply, appends the
follow-up as a user message and streams another round **in the same turn**: one `IAiTurn`, continuing `Events`,
`Result.Reply` = the last assistant message. Ignored when the round called `ask_user`.

## IAiTurn

```csharp
public interface IAiTurn
{
    string TurnId { get; }
    IAsyncEnumerable<AiTurnEvent> Events { get; }   // replays from the start on every enumeration
    Task<AiTurnResult> Completion { get; }          // never faults
    void Interrupt(string? spokenText = null);
}

public record AiTurnResult(AiTurnOutcome Outcome, AiMessageDocument? Reply,
    IReadOnlyList<AiQuestion> Questions, UsageSnapshot? Usage, Exception? Error = null);

public enum AiTurnOutcome { Completed, AwaitingUser, Interrupted, Superseded, Failed }
```

Turn events, in order: `TurnStarted(UserMessage)`, `TextDelta(Text)`, `ToolCallStarted(CallId, Name, ArgumentsJson, Silent)`,
`ToolCallCompleted(CallId, Name, Result, IsError)`, `QuestionsAsked(Questions)`, then exactly one of
`TurnCompleted(Result)`, `TurnInterrupted(Result, SpokenText)` or `TurnFailed(Result, Error)`.

```csharp
var turn = session.Send(AiInput.Text(text));
await foreach (var e in turn.Events)
{
    switch (e)
    {
        case TextDelta d: reply.Append(d.Text); break;
        case ToolCallStarted t when !t.Silent: ShowWorking(t.Name); break;
    }
}
var result = await turn.Completion;
```

### Interrupting and superseding

- `turn.Interrupt()` stops generation; the partial reply is stored as `MessageStatus.Interrupted`.
- `turn.Interrupt(spokenText)` records how much was actually heard; the model is later sent that text
  with an `[interrupted by user]` marker instead of the full reply. Calling it **after** `Completion`
  (reply finished streaming but was still being spoken) marks the stored reply the same way.
- `session.Send(...)` while a turn is active cancels it with `AiTurnOutcome.Superseded`. That is how a
  "stop and ask something else" UX works - no manual cancel needed.
- Cancelling the `ct` passed to `Send` ends the turn as `Interrupted` (not an error).

### Failures

The user message is stored before the model is called. A failing call ends the turn with
`AiTurnOutcome.Failed`, `Result.Error` set, the reply stored as `MessageStatus.Failed`, and a
`SessionError` session event. Show the error from the result; don't catch exceptions around
`Completion`.

## Session events

`GetEvents(ct)` is a hot broadcast; each call is an independent subscription and **the first item is a
`Snapshot`** (`State`, `PendingQuestions`, `Usage`, `Context`). It ends when `ct` is cancelled or the
instance is disposed. A slow subscriber may miss `TextDelta`s but never lifecycle events; one that
falls hopelessly behind is ended with an exception rather than stalling the session.

| Event | When |
|---|---|
| `Snapshot` | First item of every subscription |
| `MessageAdded(Message)` | User message stored; assistant placeholder stored as `Streaming` |
| `MessageUpdated(Message)` | Assistant reply finalized / marked interrupted |
| `StateChanged(State)` | `Thinking` -> `Responding` -> `Idle` or `AwaitingUser` |
| `TurnEventOccurred(Event)` | Every turn event (including `TextDelta`) |
| `UsageChanged(Usage, Context)` | After each turn |
| `ContextPressureChanged(Context)` | `Normal` / `Warning` / `Critical` level changed |
| `TitleChanged(Title)` | `SetTitle` |
| `ContextCompacted(Summary, CompactedCount, WasHardTrim)` | Compaction finished (see `compaction.md`) |
| `MemoryChanged(Document, Change)` / `MemoryPressureChanged(...)` | Assistant memory writes / scope full (see `memory.md`) |
| `SessionError(Error)` | Turn failure or interceptor error |

Consume from one background loop and marshal to the UI thread; render bubbles from
`MessageAdded`/`MessageUpdated` (one assistant message per turn, `Text` is display text) and stream
`TextDelta` into the in-progress bubble.

## History paging

```csharp
var page = await session.GetMessages(beforeSequence: null, count: 30);   // newest page
// page.Messages are in sequence order; page.HasMore => load older:
var older = await session.GetMessages(page.Messages[0].Sequence, 30);
```

Render `AiMessageDocument.Text` (user and assistant), not `Parts`; `Parts` also contain model-only
tool calls/results.

## Usage and context

- `Usage` (`SessionUsage`) sums every model request including tool-loop round trips; `RequestCount` counts requests.
- `Context.CurrentTokens` = input + output of the **last** request (the real prompt size). When the
  provider reports no usage it is estimated (`IsEstimate = true`).
- `Context.Pressure` is null unless a context window is known. Warn the user at `Warning`, offer a new
  session at `Critical`. The library never shows UI itself.
