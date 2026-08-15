# IMessageStore

## Interface

**Namespace**: `Shiny.AiConversation`

```csharp
public interface IMessageStore
{
    Task Store(string? userTriggeringMessage, string? assistantMessage, ChatResponse response, CancellationToken cancellationToken);
    Task Clear(DateTimeOffset? beforeDate = null);
    Task<IReadOnlyList<AiChatMessage>> Query(
        string? messageContains = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default
    );
}
```

## Methods

### Store
Persists the exchange. Called once after the AI finishes responding.

**Always store `assistantMessage`, never `response.Text`.** When structured output is enabled (the
default), `response.Text` is the raw JSON turn envelope; persisting it would push JSON into chat history
and into future prompts via `ChatLookupAITool`. `assistantMessage` is the parsed reply — or the raw text
when the turn was unstructured. `response` is still passed for metadata such as token usage.

### Clear
Removes messages from the store:
- `beforeDate = null` — clears all messages
- `beforeDate` specified — removes only messages older than the given date

### Query
Searches the message store with optional filtering:
- `messageContains` — text search against message content
- `fromDate` / `toDate` — inclusive date range filter
- `limit` — maximum number of results to return
- Returns messages ordered by timestamp

## Implementation Example

Using Shiny.DocumentDb:

```csharp
using Shiny.DocumentDb;
using Shiny.AiConversation;

public class DocumentDbMessageStore(IDocumentStore store) : IMessageStore
{
    public Task Store(string? userTriggeringMessage, string? assistantMessage, ChatResponse response, CancellationToken cancellationToken)
    {
        if (assistantMessage is not { } text)   // NOT response.Text - that is the JSON envelope
            return Task.CompletedTask;

        return store.Insert(
            new AiChatMessage(Guid.NewGuid().ToString(), text, DateTimeOffset.UtcNow, ChatMessageDirection.AI),
            cancellationToken: cancellationToken
        );
    }

    public async Task Clear(DateTimeOffset? beforeDate = null)
    {
        var query = store.Query<AiChatMessage>();
        if (beforeDate.HasValue)
            query = query.Where(x => x.Timestamp <= beforeDate.Value);
        await query.ExecuteDelete();
    }

    public async Task<IReadOnlyList<AiChatMessage>> Query(
        string? messageContains = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = store.Query<AiChatMessage>().OrderBy(x => x.Timestamp);

        if (!String.IsNullOrWhiteSpace(messageContains))
            query = query.Where(x => x.Message.Contains(messageContains));

        if (fromDate != null)
            query = query.Where(x => x.Timestamp >= fromDate.Value);

        if (toDate != null)
            query = query.Where(x => x.Timestamp <= toDate.Value);

        if (limit != null)
            query = query.Paginate(0, limit.Value);

        return await query.ToList(cancellationToken);
    }
}
```

## Registration

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetChatClientProvider<MyChatClientProvider>();
    opts.SetMessageStore<DocumentDbMessageStore>();
});
```
