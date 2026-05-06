# ChatLookupAITool

## Overview

An optional AI tool that allows the AI to search past conversations stored in `IMessageStore`. When registered, the AI can autonomously decide to look up previous chats when the user asks about past discussions.

**Namespace**: `Shiny.AiConversation.Infrastructure`

## How It Works

`ChatLookupAITool` wraps an `IMessageStore.Query()` call as a Microsoft.Extensions.AI `AITool`. When the AI receives a prompt that references past conversations (e.g., "What did I ask about yesterday?"), it can invoke this tool to search the history.

The tool is named `lookup_chat_history` and accepts:
- `query` (string?) — text to search for in past messages
- `fromDate` (string?) — start date in ISO 8601 format
- `toDate` (string?) — end date in ISO 8601 format

Results are limited to 50 messages and formatted as timestamped lines.

## Registration

The tool is registered automatically when `addAiLookupTool: true` is passed to `SetMessageStore()`:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetChatClientProvider<MyChatClientProvider>();
    opts.SetMessageStore<MyMessageStore>(addAiLookupTool: true); // default is true
});
```

This registers:
1. `ChatLookupAITool` as a singleton
2. An `AITool` singleton (resolved via `ChatLookupAITool.AsTool()`)

The `AiConversationService` implementation collects all registered `AITool` instances via `IEnumerable<AITool>` and passes them to the chat client's `ChatOptions.Tools`.

## Disabling

To use IMessageStore without the lookup tool:

```csharp
opts.SetMessageStore<MyMessageStore>(addAiLookupTool: false);
```
