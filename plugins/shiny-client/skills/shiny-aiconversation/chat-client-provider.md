# IChatClientProvider

## Interface

**Namespace**: `Shiny.AiConversation`

```csharp
public interface IChatClientProvider
{
    Task<IChatClient> GetChatClient(CancellationToken cancelToken = default);
}
```

## Purpose

Provides an `IChatClient` (from Microsoft.Extensions.AI) to the AI service.

## Default Behavior

A default `InjectedChatClientProvider` is registered automatically. It resolves `IChatClient` from the DI container. For most cases, simply register an `IChatClient` in DI:

```csharp
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());
```

If no `IChatClient` is registered and no custom provider is set, an `InvalidOperationException` is thrown at runtime.

## Built-in Providers

### Shiny.AiConversation.OpenAi

**NuGet**: `Shiny.AiConversation.OpenAi`
**Namespace**: `Shiny.AiConversation.OpenAi`
**Class**: `OpenAiStaticChatProvider`

A static provider that creates an OpenAI-compatible chat client once at startup. Works with any OpenAI-compatible endpoint (OpenAI, Azure OpenAI, Ollama, etc.). Includes logging and function invocation middleware.

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

### Shiny.AiConversation.Maui.GithubCopilot

**NuGet**: `Shiny.AiConversation.Maui.GithubCopilot`
**Namespace**: `Shiny.AiConversation.Maui.GithubCopilot`
**Class**: `GitHubCopilotChatClientProvider`

A MAUI-specific provider that uses the GitHub device code flow for authentication and the Copilot API for chat completions.

**Features**:
- Self-contained authentication — shows a popup with the device code, copies it to the clipboard, opens the browser
- Tokens stored in `SecureStorage`
- Copilot API token exchange with caching and automatic refresh
- Re-authentication on 401 responses
- `AccessTokenChanged` event for monitoring auth state
- `SignOut()` to clear stored tokens
- `CancelAuthentication()` to cancel an in-progress auth flow

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddGithubCopilotChatClient();
});
```

No additional configuration is needed — no token storage, no login page, no event wiring. The provider handles the entire lifecycle.

## Custom Implementation

Implement `IChatClientProvider` only for scenarios not covered by the built-in providers:
- Custom authentication flows
- Client construction and configuration
- Token refresh and re-authentication on expiry

## Key Design Decisions

1. **On-demand authentication** — Don't force login at startup. Let the user see the app first, then authenticate when an AI feature is first used.

2. **Token expiry handling** — Catch 401/Unauthorized responses and re-authenticate transparently.

3. **No reflection** — Register the provider explicitly with `SetChatClientProvider<T>()`.

## Registration

For most apps, just register an `IChatClient` in DI — the default provider handles it:

```csharp
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());
builder.Services.AddShinyAiConversation(opts => { });
```

For built-in providers, use the extension methods:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.AddStaticOpenAIChatClient("key", "https://api.openai.com/v1", "gpt-4o");
    // OR
    opts.AddGithubCopilotChatClient();
});
```

For custom providers:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetChatClientProvider<MyChatClientProvider>();
});
```
