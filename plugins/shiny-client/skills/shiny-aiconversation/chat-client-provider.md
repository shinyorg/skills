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

Provides an `IChatClient` (from Microsoft.Extensions.AI) to the AI service. Implementations handle:
- Authentication and token management
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
