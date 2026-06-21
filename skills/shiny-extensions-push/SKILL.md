---
name: shiny-extensions-push
description: Generate code using Shiny.Extensions.Push, a server-side push notification dispatch library for .NET with a provider-agnostic core and transports for APNs (.p8/ES256), FCM (HTTP v1), Web Push (VAPID + RFC 8291) and WNS (Windows App SDK / Entra auth), plus Shiny.DocumentDb persistence, structured targeting, topics, interceptors, dead-token pruning, multi-app keyed registration, and System.Diagnostics.Metrics + tracing — fully AOT/trim-safe.
auto_invoke: true
triggers:
  - AddPushNotifications
  - IPushManager
  - IPushProvider
  - IPushBatchProvider
  - IPushRepository
  - IPushInterceptor
  - PushNotification
  - DeviceRegistration
  - PushFilter
  - PushSendResult
  - PushDeliveryResult
  - AddApns
  - ApnsProvider
  - ApnsOptions
  - AddFcm
  - FcmProvider
  - FcmOptions
  - AddWebPush
  - WebPushProvider
  - WebPushOptions
  - AddWns
  - WnsProvider
  - WnsOptions
  - WindowsPushOptions
  - WnsNotificationType
  - UseDocumentDb
  - DocumentDbPushRepository
  - PushMetrics
  - PushDiagnostics
  - SubscribeToTopic
  - SendToTopic
  - DevicePlatform
  - PushEnvironment
  - SendToUser
  - SendToTags
  - Shiny.Extensions.Push
  - server push notification
  - APNs server
  - FCM server
  - FCM multicast
  - push batching
  - web push VAPID
  - WNS server
  - Windows push
  - send push from server
---

# Shiny.Extensions.Push Skill

You are an expert in **Shiny.Extensions.Push**, a server-side push notification dispatch library for
.NET. It has a provider-agnostic core, a direct **APNs** transport (token-based `.p8` / ES256 over
HTTP/2 — no FCM dependency for Apple), a **Shiny.DocumentDb** persistence option, structured targeting,
an interceptor pipeline, automatic dead-token pruning, multi-app keyed registration, and
`System.Diagnostics.Metrics` telemetry. The whole library is AOT/trim-safe.

## When to Use This Skill

Invoke this skill when the user wants to:
- Send push notifications from a .NET **server/backend** (ASP.NET Core, worker, Aspire) to mobile/desktop devices
- Register/unregister device tokens and store them (in-memory or Shiny.DocumentDb)
- Send to a user across all their devices, to tags/segments, to explicit tokens, or broadcast
- Talk to **APNs directly** with a `.p8` auth key (iOS/macOS), **FCM** (HTTP v1) for Android, **Web Push** (VAPID) for browsers, or **WNS** (Windows App SDK / Entra auth) for Windows
- Subscribe devices to **topics** and send to a topic
- Mutate, localize, personalize, or suppress notifications per-device via interceptors
- Automatically prune expired/invalid device tokens and apply rotated tokens
- Serve **multiple apps** from one server (keyed registrations per provider)
- Emit push delivery metrics + traces for OpenTelemetry
- Send silent/background (content-available) pushes

> This is the **server** counterpart. For the on-device client (registering for push, receiving), use
> `Shiny.Push` (the MAUI client library) — a different package.

## Library Overview

- **Core namespace**: `Shiny.Extensions.Push`
- **NuGet packages**:
  - `Shiny.Extensions.Push` — core **plus all four transports**: models,
    `IPushManager`/`IPushProvider`/`IPushRepository`/`IPushInterceptor`, builder/DI, and the APNs, FCM,
    Web Push and WNS (Windows) providers. The implementation types (`PushManager`, `InMemoryPushRepository`,
    `DebugPushProvider`, `PushMetrics`) live in the `Shiny.Extensions.Push.Infrastructure` namespace; each
    transport keeps its own namespace (`Shiny.Extensions.Push.Apns` / `.Fcm` / `.WebPush` / `.Wns`).
  - `Shiny.Extensions.Push.DocumentDb` — `IPushRepository` backed by Shiny.DocumentDb (any provider)
- **Namespaces**: inject the **interfaces** from `Shiny.Extensions.Push` (`IPushManager`, etc.). Add a
  transport with `using Shiny.Extensions.Push.Apns;` (or `.Fcm` / `.WebPush` / `.Wns`) to surface its
  `AddApns` extension. You only need `using Shiny.Extensions.Push.Infrastructure;` to name a concrete impl
  directly (e.g. `AddProvider<DebugPushProvider>()` or casting a repo in tests) — normal app code shouldn't.
- **Transports are no longer separate packages.** Do **not** `dotnet add package Shiny.Extensions.Push.Apns`
  (or `.Fcm` / `.WebPush` / `.Wns`) — they ship inside `Shiny.Extensions.Push`.
- **Target**: `net10.0`. AOT/trim-safe.

## Setup (Dependency Injection)

```csharp
using Shiny.Extensions.Push;
using Shiny.Extensions.Push.Apns;
using Shiny.Extensions.Push.DocumentDb;
using Shiny.DocumentDb.Sqlite;

services.AddPushNotifications(push =>
{
    // Direct APNs (single app)
    push.AddApns(o =>
    {
        o.TeamId = "ABCDE12345";
        o.KeyId  = "KEY1234567";
        o.BundleId = "com.example.app";
        o.PrivateKeyPath = "AuthKey_KEY1234567.p8";   // or o.PrivateKey = "<PEM contents>"
        // o.ForceEnvironment = PushEnvironment.Sandbox;  // default: honour each registration
    });

    // FCM (Android) — Google service-account JSON
    push.AddFcm(o => o.ServiceAccountJsonPath = "firebase-service-account.json");

    // Web Push (browsers) — VAPID keys in base64url (web-push format)
    push.AddWebPush(o =>
    {
        o.PublicKey  = "<vapid-public-key>";
        o.PrivateKey = "<vapid-private-key>";
        o.Subject    = "mailto:you@example.com";
    });

    // WNS (Windows) — modern Windows App SDK / Entra (Azure AD) app registration
    push.AddWns(o =>
    {
        o.TenantId     = "<entra-tenant-id>";
        o.ClientId     = "<app-registration-client-id>";
        o.ClientSecret = "<client-secret>";
    });

    // Persistence (defaults to in-memory if omitted)
    push.UseDocumentDb(o => o.DatabaseProvider = new SqliteDatabaseProvider("Data Source=push.db"));

    // Optional
    // push.AddInterceptor<LocalizationInterceptor>();
    // push.Configure(m => { m.MaxDegreeOfParallelism = 25; m.AutoPruneDeadTokens = true; });
});
```

Defaults when not configured: an in-memory `IPushRepository` and the built-in `PushManager`.
`IMeterFactory`/metrics are wired automatically.

## Registering Devices

Resolve `IPushManager` and register the device when your client app reports its token:

```csharp
await pushManager.RegisterDevice(new DeviceRegistration
{
    DeviceToken    = "<apns-device-token>",
    Platform       = DevicePlatform.iOS,        // iOS, MacOS, Android, Windows, WebBrowser
    DeviceId       = "install-guid",            // stable identity across token rotation (recommended)
    UserIdentifier = "user-42",                 // enables "send to user across all devices"
    Tags           = ["beta", "sports"],
    Locale         = "en-US",
    Environment    = PushEnvironment.Production, // APNs sandbox vs production (tokens are env-specific!)
    AppId          = null                        // set when using multi-app keyed providers
});

await pushManager.UnregisterDevice("<apns-device-token>", DevicePlatform.iOS);
```

## Sending

`IPushManager` returns a `PushSendResult` (`BatchId`, `Sent`, `Failed`, `TokensRemoved`, `Skipped`, `Results`).
One device failing never aborts the batch.

```csharp
var result = await pushManager.SendToUser("user-42", new PushNotification
{
    Title    = "Goal!",
    Message  = "Your team just scored",
    Badge    = 1,
    Sound    = "default",
    DeepLink = "app://match/123",
    Priority = PushPriority.High,
    Data     = new Dictionary<string, string> { ["matchId"] = "123" }
});

await pushManager.SendToTags(["sports"], notification, TagMatch.Any);
await pushManager.SendToTokens(["tokenA", "tokenB"], notification);
await pushManager.Broadcast(notification);

// Full structured targeting (all set clauses are AND-combined):
await pushManager.Send(notification, new PushFilter
{
    Platforms      = [DevicePlatform.iOS],
    Tags           = ["beta"],
    TagMatch       = TagMatch.All,
    Environment    = PushEnvironment.Production
});
```

### Apple-specific options & silent push

```csharp
new PushNotification
{
    Title = "Title",
    Apple = new ApplePushOptions
    {
        Subtitle        = "Subtitle",
        Category        = "MESSAGE_CATEGORY",
        ThreadId        = "thread-1",
        MutableContent  = true,                 // for a Notification Service Extension
        // ContentAvailable = true,             // background push
        // TopicOverride / PushTypeOverride for VoIP/complication topics
    },
    CollapseId  = "score-update",               // apns-collapse-id
    TimeToLive  = TimeSpan.FromHours(1)         // apns-expiration
};

// Silent / background push (no visible alert):
new PushNotification
{
    Apple = new ApplePushOptions { ContentAvailable = true },
    Data  = new Dictionary<string, string> { ["sync"] = "inbox" }
};
```

## Interceptors (localize / personalize / suppress / report)

Implement `IPushInterceptor` and register with `push.AddInterceptor<T>()` (additive; run in order).
Mutate by replacing `context.Notification`; return `InterceptorResult.Skip` to drop a device.

```csharp
public sealed class LocalizationInterceptor : IPushInterceptor
{
    public Task<InterceptorResult> BeforeSend(PushSendContext context, CancellationToken ct = default)
    {
        if (context.Registration.Tags.Contains("opted-out"))
            return Task.FromResult(InterceptorResult.Skip);

        var title = Localize(context.Notification.Title, context.Registration.Locale);
        context.Notification = context.Notification with { Title = title };
        return Task.FromResult(InterceptorResult.Continue);
    }

    public Task OnSent(PushSendContext c, PushDeliveryResult r, CancellationToken ct = default) => Task.CompletedTask;
    public Task OnFailed(PushSendContext c, PushDeliveryResult r, CancellationToken ct = default) => Task.CompletedTask;
}
```

## Dead-token pruning & rotation (automatic)

Providers return a normalized `PushDeliveryStatus`. The manager auto-removes tokens reported
`TokenExpired`/`InvalidToken` (APNs `410 Unregistered` / `BadDeviceToken`) and applies rotated tokens
(`PushDeliveryResult.UpdatedToken`) to the repository. Disable via `PushManagerOptions.AutoPruneDeadTokens = false`.
`RateLimited` is surfaced on the result but **not** retried — backoff is the caller's responsibility.
On success, providers set `PushDeliveryResult.ProviderMessageId` (APNs `apns-id`, FCM message `name`,
WebPush `Location`).

## Topics

Topics are server-side subscriptions (work across every provider, independent of FCM-native topics):

```csharp
await pushManager.SubscribeToTopic(token, DevicePlatform.iOS, "sports");
await pushManager.SendToTopic("sports", new PushNotification { Title = "Goal!" });
await pushManager.UnsubscribeFromTopic(token, DevicePlatform.iOS, "sports");
```

## Platform routing (FCM / Web Push / WNS)

The manager routes a registration to the provider that claims its `Platform`: APNs → `iOS`/`MacOS`,
FCM → `Android`, Web Push → `WebBrowser`, WNS → `Windows`. For **WNS**, the registration's `DeviceToken` is
the WNS **channel URI**. For **Web Push**, the `DeviceToken` is the subscription endpoint and the
`p256dh`/`auth` keys go in `Data`:

```csharp
await pushManager.RegisterDevice(new DeviceRegistration
{
    DeviceToken = subscription.Endpoint,
    Platform    = DevicePlatform.WebBrowser,
    Data        = new Dictionary<string, string>
    {
        ["p256dh"] = subscription.Keys.P256dh,
        ["auth"]   = subscription.Keys.Auth
    }
});
```

FCM uses `AndroidPushOptions` on the notification for `android.notification` fields (channel id, icon,
color, image); Web Push uses `WebPushOptions` (icon, urgency); WNS uses `WindowsPushOptions`
(`Type` = Toast/Tile/Badge/Raw, a verbatim `Payload` for tile/badge, and a toast `Launch` arg — default is
a `ToastGeneric` toast built from title/body + `DeepLink`). All providers honour the cross-cutting fields
(title/body, badge, sound, data, deep link, collapse id, TTL, priority).

## Batching / FCM multicast

`FcmProvider` implements `IPushBatchProvider` (`MaxBatchSize` 500): when `PushManagerOptions.EnableBatching`
is on (the default), the manager packs devices that share the same notification into one multipart `/batch`
request instead of one request per device — far fewer round trips for broadcasts and topic fan-out. This is
automatic; no API change at the call site. Notes:
- Grouping is by the **identical notification instance**, so an interceptor that rewrites the notification
  per device (localization) naturally falls back to per-device sends for those devices.
- Per-device dead-token pruning, token rotation, metrics, and `OnSent`/`OnFailed` still happen per device.
- Set `push.Configure(m => m.EnableBatching = false)` to force per-device delivery everywhere.
- To make a custom transport batchable, implement `IPushBatchProvider` (`MaxBatchSize` + `SendBatch`
  returning one result per registration, in input order). Web Push has no multicast endpoint, so it stays
  one request per device (fanned out concurrently by the manager).

## Multiple apps (keyed registration)

Register one keyed provider per app (works for `AddApns`/`AddFcm`/`AddWebPush`/`AddWns`); devices carry the
matching `AppId`; the manager routes by it.

```csharp
services.AddPushNotifications(push =>
{
    push.AddApns("consumer", o => { o.BundleId = "com.example.consumer"; /* … */ });
    push.AddApns("driver",   o => { o.BundleId = "com.example.driver";   /* … */ });
});

await pushManager.RegisterDevice(new DeviceRegistration
{
    DeviceToken = "<token>", Platform = DevicePlatform.iOS, AppId = "driver"
});

await pushManager.Send(notification, new PushFilter { AppId = "driver", Tags = ["on-shift"] });
```

## Metrics

Telemetry is emitted via `System.Diagnostics.Metrics` under the meter `Shiny.Extensions.Push`
(`PushMetrics.MeterName`): counters `push.notifications.sent` / `.failed` / `.skipped`,
`push.tokens.pruned`, and histogram `push.send.duration` (ms) — tagged by `platform`, `provider`,
`status` (never by `BatchId`, which is high-cardinality).

Distributed tracing uses the `ActivitySource` `PushDiagnostics.ActivitySourceName` (same name): a
`push.send` span per batch and `push.deliver` per device (batched sends emit one `push.deliver.batch`
span with a `push.batch_size` tag instead).

```csharp
services.AddOpenTelemetry()
    .WithMetrics(m => m.AddMeter(PushMetrics.MeterName))
    .WithTracing(t => t.AddSource(PushDiagnostics.ActivitySourceName));
```

## Persistence (Shiny.DocumentDb)

`Shiny.Extensions.Push.DocumentDb` implements `IPushRepository` over any Shiny.DocumentDb backend
(SQLite, Postgres, SQL Server, Cosmos, Mongo, …). `UseDocumentDb(Action<DocumentStoreOptions>)`
registers the store + repository; `UseDocumentDb()` assumes an `IDocumentStore` is already registered.
Token-keyed operations (save/remove/rotate) are O(1) point lookups; targeted sends currently scan and
filter in-process. To replace persistence entirely, implement `IPushRepository` and call
`push.UseRepository<T>()`.

## Custom providers

Implement `IPushProvider` (`Identifier`, `CanDeliver(registration)`, `Send(...)` returning a
`PushDeliveryResult`) and register additively via `push.AddProvider<T>()`. Optionally also implement
`IPushBatchProvider` to deliver groups of devices in one call (see *Batching* above). For local dev/tests,
the built-in `DebugPushProvider` logs instead of sending and claims every platform.

## Key Conventions / Gotchas

- `Title`/`Message` are nullable — silent/background pushes carry only `Data` + `ContentAvailable`.
- APNs tokens are **environment-specific** (sandbox vs production) — set `DeviceRegistration.Environment`.
- Prefer a stable `DeviceId` so re-registration upserts instead of duplicating.
- Custom payload `Data` is `string`-keyed/valued to stay AOT-safe.
- The APNs provider caches its ES256 provider JWT (~50 min) and reuses one pooled HTTP/2 connection.
