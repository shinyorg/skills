# IMessageStore

## Interface

**Namespace**: `Shiny.AiConversation`

```csharp
public interface IMessageStore
{
    Task Store(AiChatMessage chatMessage, CancellationToken cancellationToken);
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
Persists a single chat message. Called automatically by AiConversationService after each user message and AI response.

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
    public async Task Store(AiChatMessage chatMessage, CancellationToken cancellationToken)
    {
        await store.Set(chatMessage.Id, chatMessage);
    }

    public async Task Clear(DateTimeOffset? beforeDate = null)
    {
        if (beforeDate == null)
        {
            await store.Clear<AiChatMessage>();
        }
        else
        {
            var old = await store
                .CreateQuery<AiChatMessage>()
                .Where(x => x.Timestamp < beforeDate.Value)
                .ToListAsync();

            foreach (var msg in old)
                await store.Remove<AiChatMessage>(msg.Id);
        }
    }

    public async Task<IReadOnlyList<AiChatMessage>> Query(
        string? messageContains = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = store.CreateQuery<AiChatMessage>();

        if (!String.IsNullOrWhiteSpace(messageContains))
            query = query.Where(x => x.Message.Contains(messageContains, StringComparison.OrdinalIgnoreCase));

        if (fromDate != null)
            query = query.Where(x => x.Timestamp >= fromDate.Value);

        if (toDate != null)
            query = query.Where(x => x.Timestamp <= toDate.Value);

        query = query.OrderBy(x => x.Timestamp);

        if (limit != null)
            query = query.Paginate(0, limit.Value);

        return await query.ToListAsync(cancellationToken);
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
