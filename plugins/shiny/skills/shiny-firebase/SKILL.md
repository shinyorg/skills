---
name: shiny-firebase
description: Guide for implementing Firebase Cloud Messaging push notifications in .NET MAUI apps using Shiny.Push.FirebaseMessaging on iOS and Android.
auto_invoke: true
triggers:
  - firebase
  - FCM
  - firebase cloud messaging
  - push notifications firebase
  - AddPushFirebaseMessaging
  - FirebaseConfiguration
  - GoogleService-Info.plist
  - google-services.json
  - firebase push
  - firebase messaging
  - RequestAccess hangs
  - FCM token never returns
  - duplicate FIRApp
  - Class FIRApp is implemented in both
---

# Shiny Firebase Push Notifications Skill

## Overview

Shiny.Push.FirebaseMessaging provides Firebase Cloud Messaging (FCM) push notification support for .NET MAUI applications on iOS and Android. It wraps the native Firebase iOS SDK (via a Slim Binding) and Android FCM through the Shiny Push infrastructure.

On iOS everything ships in **one** binding — `Shiny.Firebase.iOS.Binding`, pulled in automatically — which embeds a single copy of FirebaseCore alongside the Messaging and Analytics shims. Do **not** add another Firebase iOS binding package (e.g. `AdamE.Firebase.iOS.*`) to the same app: a second FirebaseCore means a second `FIRApp`, and whichever one gets configured is not necessarily the one the token request reads.

## NuGet Package

```
Shiny.Push.FirebaseMessaging
```

## Setup

### MauiProgram.cs Configuration

```csharp
using Shiny;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShiny(); // Required

        // Option 1: Use embedded configuration (GoogleService-Info.plist / google-services.json)
        builder.Services.AddPushFirebaseMessaging();

        // Option 2: Use embedded configuration explicitly
        builder.Services.AddPushFirebaseMessaging(FirebaseConfiguration.Embedded);

        // Option 3: Manual configuration
        builder.Services.AddPushFirebaseMessaging(new FirebaseConfiguration(
            UseEmbeddedConfiguration: false,
            AppId: "your-app-id",
            SenderId: "your-sender-id",
            ProjectId: "your-project-id",
            ApiKey: "your-api-key"
        ));

        // With a custom push delegate
        builder.Services.AddPushFirebaseMessaging<MyPushDelegate>();

        return builder.Build();
    }
}
```

### Custom Push Delegate

```csharp
public class MyPushDelegate : IPushDelegate
{
    public Task OnReceived(PushNotification notification)
    {
        // Handle incoming push notification
        return Task.CompletedTask;
    }

    public Task OnNewToken(string token)
    {
        // Handle FCM token changes - send to your backend
        return Task.CompletedTask;
    }

    public Task OnUnRegistered(string token)
    {
        // Handle when the device is unregistered from push
        return Task.CompletedTask;
    }

    public Task OnEntry(PushNotification notification)
    {
        // Handle when user taps on a notification
        return Task.CompletedTask;
    }
}
```

## FirebaseConfiguration Record

| Parameter | Type | Default | Description |
|---|---|---|---|
| `UseEmbeddedConfiguration` | `bool` | `true` | Use platform config files (GoogleService-Info.plist / google-services.json) |
| `AppId` | `string?` | `null` | Firebase App ID (required if not using embedded config) |
| `SenderId` | `string?` | `null` | Firebase Sender ID (required if not using embedded config) |
| `ProjectId` | `string?` | `null` | Firebase Project ID (required if not using embedded config) |
| `ApiKey` | `string?` | `null` | Firebase API Key (required if not using embedded config) |
| `DefaultChannel` | `NotificationChannel?` | `null` | Android only - default notification channel |
| `IntentAction` | `string?` | `null` | Android only - custom intent action |

## Platform-Specific Requirements

### iOS
- Add `GoogleService-Info.plist` to your iOS project (if using embedded configuration)
- Set Build Action to `BundleResource`
- Enable Push Notifications capability in Entitlements.plist
- Enable Remote Notifications background mode

### Android
- Add `google-services.json` to your Android project root (if using embedded configuration)
- The Shiny Push library handles the Android Firebase initialization automatically

## Topic Subscriptions (Tags)

The iOS implementation supports topic subscriptions through `IPushTagSupport`:

```csharp
// Inject IPushManager
var push = services.GetRequiredService<IPushManager>();

// Cast provider to access tag support
if (push is IPushTagSupport tagSupport)
{
    await tagSupport.AddTag("news");
    await tagSupport.RemoveTag("promotions");
    await tagSupport.SetTags("news", "updates");
    await tagSupport.ClearTags();

    var currentTags = tagSupport.RegisteredTags;
}
```

## Extension Methods

### `AddPushFirebaseMessaging(FirebaseConfiguration? config = null)`
Registers Firebase push notification services. Pass `null` or omit for embedded configuration.

### `AddPushFirebaseMessaging<TPushDelegate>(FirebaseConfiguration? config = null)`
Registers Firebase push with a custom `IPushDelegate` implementation that handles notification events.

## Troubleshooting

### iOS: `RequestAccess()` never completes — no token, no exception, no `OnNewToken`

Check the startup log for:

```
objc[xxxx]: Class FIRApp is implemented in both .../SomeFramework and .../AnotherFramework.
One of the two will be used. Which one is undefined.
```

The app has two copies of FirebaseCore, so `FirebaseApp.configure()` configures one `FIRApp` while the FCM
token request reads the other, still-unconfigured one — the native completion block never fires and the await
hangs forever.

- **On `Shiny.Push.FirebaseMessaging` 5.0.2 or earlier, this is a library bug** — the Core shim shipped in a
  different framework from the Messaging shim. Upgrade to 5.1.0+, where both live in one framework.
- **On 5.1.0+**, it means a *second* Firebase iOS SDK is in the app. Remove the other Firebase binding package,
  or drop `Shiny.Push.FirebaseMessaging` and use that SDK's own provider — do not run both.

From 5.1.0 the native shim also fails fast: if Firebase was never configured, `RequestAccess()` throws
`Firebase has not been configured` instead of hanging.

## Key Source Files
- `src/Shiny.Push.FirebaseMessaging/FirebaseConfiguration.cs` - Configuration record
- `src/Shiny.Push.FirebaseMessaging/Platforms/Shared/ServiceCollectionExtensions.cs` - DI registration
- `src/Shiny.Push.FirebaseMessaging/Platforms/iOS/FirebasePushProvider.cs` - iOS FCM provider
- `src/Shiny.Firebase.iOS.Binding/ApiDefinitions.cs` - the single iOS binding (Core + Messaging + Analytics)
