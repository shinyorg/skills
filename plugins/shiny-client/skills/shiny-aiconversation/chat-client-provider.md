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

## Custom Implementation

Implement `IChatClientProvider` only for advanced scenarios:
- On-demand authentication and token management
- Client construction and configuration
- Token refresh and re-authentication on expiry

## Implementation Pattern

```csharp
using Microsoft.Extensions.AI;
using Shiny.AiConversation;

public class MyChatClientProvider(INavigator navigator) : IChatClientProvider
{
    string? accessToken;

    public async Task<IChatClient> GetChatClient(CancellationToken cancelToken = default)
    {
        if (this.accessToken == null)
            await this.RequestAuthentication();

        try
        {
            // Build and return the chat client
            return BuildChatClient(this.accessToken!);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Token expired — re-authenticate
            this.accessToken = null;
            await this.RequestAuthentication();
            return BuildChatClient(this.accessToken!);
        }
    }

    async Task RequestAuthentication()
    {
        // Navigate to login page on-demand
        var tcs = new TaskCompletionSource();
        await navigator.NavigateTo<LoginViewModel>(vm => vm.AuthenticationCompleted = tcs);
        await tcs.Task; // wait for login to complete
    }

    IChatClient BuildChatClient(string token)
    {
        // Return your IChatClient implementation
        // e.g., OpenAIChatClient, Azure OpenAI, GitHub Copilot, etc.
        throw new NotImplementedException();
    }
}
```

## Key Design Decisions

1. **On-demand authentication** — Don't force login at startup. Let the user see the app first, then authenticate when an AI feature is first used.

2. **Token expiry handling** — Catch 401/Unauthorized responses and re-authenticate transparently.

3. **Navigator injection** — Use `INavigator` from Shiny.Maui.Shell to navigate to login pages. The `NavigateTo<TViewModel>` overload accepts a configuration action to pass data (like a `TaskCompletionSource`) to the login ViewModel.

4. **No reflection** — Register the provider explicitly with `SetChatClientProvider<T>()`.

## Registration

For most apps, just register an `IChatClient` in DI — the default provider handles it:

```csharp
builder.Services.AddChatClient(new OpenAIClient("your-api-key").GetChatClient("gpt-4o").AsIChatClient());
builder.Services.AddShinyAiConversation(opts => { });
```

For custom providers:

```csharp
builder.Services.AddShinyAiConversation(opts =>
{
    opts.SetChatClientProvider<MyChatClientProvider>();
});
```

If you need to resolve the concrete provider type elsewhere (e.g., for device flow polling), register it explicitly:

```csharp
builder.Services.AddSingleton<MyChatClientProvider>(
    sp => (MyChatClientProvider)sp.GetRequiredService<IChatClientProvider>()
);
```
