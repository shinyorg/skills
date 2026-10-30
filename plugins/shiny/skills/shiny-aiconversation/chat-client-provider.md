# IChatClientProvider

## Interface

**Namespace**: `Shiny.AiConversation`

```csharp
public interface IChatClientProvider
{
    Task<IChatClient> GetChatClient(CancellationToken cancelToken = default);

    // default interface members - override when known
    int? ContextWindow => null;                    // model context window in tokens
    bool SupportsImagesInToolResults => false;
}
```

## Purpose

Provides an `IChatClient` (from Microsoft.Extensions.AI) to every session turn. The session streams
with `GetStreamingResponseAsync` and runs the tool loop itself: a client **without** function
invocation is wrapped with `UseFunctionInvocation()` automatically; a client that already has it is
used as-is. Either works - don't add it just for this library.

`ContextWindow` is used when `AiSessionSettings.ContextWindow` isn't set. Without either, sessions
still report token counts but `Context.Pressure` stays null. There is no built-in model table.

There is **no** `StructuredOutputMode` any more - questions come from the built-in `ask_user` tool,
which every tool-capable provider supports.

## Default Behavior

`InjectedChatClientProvider` is registered automatically and resolves `IChatClient` from DI:

```csharp
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());
builder.Services.AddShinyAiConversation();
```

If no `IChatClient` is registered and no custom provider is set, an `InvalidOperationException` is
thrown when the first turn runs (reported as `AiTurnOutcome.Failed`).

## Built-in Providers

### Shiny.AiConversation.OpenAi

**NuGet**: `Shiny.AiConversation.OpenAi` · **Class**: `OpenAiStaticChatProvider`

Creates an OpenAI-compatible client once (OpenAI, Azure OpenAI, Ollama, ...).

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddStaticOpenAIChatClient(
        apiToken: "your-api-key",
        endpointUri: "https://api.openai.com/v1",
        modelName: "gpt-4o"
    );
});
```

### Shiny.AiConversation.GithubCopilot

**NuGet**: `Shiny.AiConversation.GithubCopilot` · **Class**: `GithubCopilotStaticChatProvider`

Exchanges a GitHub personal access token for short-lived Copilot tokens and refreshes them.

```csharp
opts.AddStaticGithubCopilotChatClient("github-pat", "gpt-4o");
```

### Shiny.AiConversation.Maui.GithubCopilot

**NuGet**: `Shiny.AiConversation.Maui.GithubCopilot` · **Class**: `GitHubCopilotChatClientProvider`

MAUI provider using the GitHub device code flow: shows the code in a popup, copies it, opens the
browser, stores tokens in `SecureStorage`, refreshes the Copilot token and re-authenticates on 401.
Extra API: `StartAuthentication()`, `CancelAuthentication()`, `SignOut()`, `IsAuthenticated`,
`AccessTokenChanged`.

```csharp
opts.AddGithubCopilotChatClient();
```

## Custom Implementation

```csharp
public class MyChatClientProvider(IAuthService auth) : IChatClientProvider
{
    IChatClient? client;

    public async Task<IChatClient> GetChatClient(CancellationToken cancelToken = default)
    {
        if (this.client == null)
        {
            var token = await auth.GetToken(cancelToken);   // authenticate on demand, not at startup
            this.client = new OpenAIClient(new ApiKeyCredential(token))
                .GetChatClient("gpt-4o")
                .AsIChatClient();
        }
        return this.client;
    }

    public int? ContextWindow => 128_000;
}

builder.Services.AddShinyAiConversation(opts => opts.SetChatClientProvider<MyChatClientProvider>());
```

Guidelines: authenticate on demand (first turn), cache the client, refresh tokens on expiry, and
register explicitly with `SetChatClientProvider<T>()` (no reflection/assembly scanning).
