# Context Compaction

Sessions never end, so long conversations are kept inside the model's context window by **rolling
compaction**: the previous summary plus the oldest messages are folded into one new summary. The
originals are marked `Compacted = true` (kept for history and search) and leave the prompt.

## When it runs

- **Manually:** `await session.Compact();` (waits for any active turn; throws if compaction itself fails).
- **Automatically (safety net):** when `Context.PercentUsed >= Compaction.AutoCompactThreshold` (default
  0.95). Checked after every completed turn (runs in the background so the next turn rarely waits) and
  before a turn starts. Needs a known context window (`AiSessionSettings.ContextWindow` or
  `IChatClientProvider.ContextWindow`).
- **Message cap:** `Compaction.MaxMessages` compacts whenever the prompt holds more messages than that,
  even without a known window.

The library only reports pressure (`ContextPressureChanged` at the 0.75 / 0.90 thresholds); the app decides
whether to warn the user, call `Compact()`, or start a new session.

## Settings

```csharp
var session = await manager.Start(new AiSessionSettings
{
    ContextWindow = 128_000,
    Compaction = new CompactionSettings
    {
        AutoCompactThreshold = 0.95,   // null disables automatic compaction
        KeepRecentMessages = 10,       // newest messages that are never compacted
        MaxMessages = null,            // optional hard cap
        FlushToMemory = true           // let the model save notes to memory first
    }
});
```

## What happens

1. State becomes `AiSessionState.Compacting`.
2. If memory is enabled and `FlushToMemory` is true, one silent request (memory tool only) asks the model to
   save what it will need. Nothing from it is persisted except memory writes and usage.
3. `IContextCompactor.Summarize(previousSummary, messages)` produces the new summary.
4. A `MessageRole.Summary` message is appended (a UI marker), the originals and the previous summary
   message are marked compacted, and the header's `Summary` is updated. The summary reaches the model from
   the header, at the top of the prompt.
5. `ContextCompacted(Summary, CompactedCount, WasHardTrim)` is published and `ISessionInterceptor.OnCompacted` runs.

Rules: the reply that asked the current `PendingQuestions` is never compacted; a still-streaming message is
never compacted. If summarizing fails, compaction **hard-trims** instead (`WasHardTrim = true`,
`Summary = null`, previous summary kept) rather than failing the turn.

## Events

```csharp
await foreach (var e in session.GetEvents(ct))
{
    if (e is ContextCompacted c)
        ShowDivider(c.WasHardTrim ? "Older messages removed" : "Earlier conversation summarized");
}
```

## Custom compactor

```csharp
public class MyCompactor : IContextCompactor
{
    public Task<string> Summarize(string? previousSummary, IReadOnlyList<AiMessageDocument> messages, CancellationToken ct)
    {
        // must keep names, preferences, decisions, open tasks and unanswered questions
        ...
    }
}

opts.SetContextCompactor<MyCompactor>();
```

The default `ChatClientContextCompactor` asks the session's own model for a third-person summary under 300 words.
