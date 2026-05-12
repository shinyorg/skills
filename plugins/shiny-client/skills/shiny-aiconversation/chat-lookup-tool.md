# ChatLookupAITool

## Overview

An AI tool that allows the AI to search past conversations stored in `IMessageStore`. When a message store is registered, the `ContextProvider` automatically adds this tool to every request — the AI can autonomously decide to look up previous chats when the user asks about past discussions.

**Namespace**: `Shiny.AiConversation.Infrastructure`

## How It Works

`ChatLookupAITool` wraps an `IMessageStore.Query()` call as a Microsoft.Extensions.AI `AITool`. When the AI receives a prompt that references past conversations (e.g., "What did I ask about yesterday?"), it can invoke this tool to search the history.

The tool is named `lookup_chat_history` and accepts:
- `query` (string?) — text to search for in past messages
- `fromDate` (string?) — start date in ISO 8601 format
- `toDate` (string?) — end date in ISO 8601 format

Results are limited to 50 messages and formatted as timestamped lines.

## Registration

The tool is added automatically by the built-in `ContextProvider` when an `IMessageStore` is registered:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetMessageStore<MyMessageStore>();
});
```

The `ContextProvider` creates a `ChatLookupAITool` instance during its `Apply(AiContext)` method and adds it to the context's `Tools` list. No separate DI registration is needed.

## Disabling

If you want to use `IMessageStore` for persistence without the AI lookup tool, implement a custom `IContextProvider` that does not add the tool, and do not register the built-in `ContextProvider`.
