---
name: shiny-firestore-mobile
description: Generate code using Shiny.DocumentDb.Firestore.Mobile, an on-device (native) Firebase Firestore provider for Shiny.DocumentDb on iOS and Android — offline-first persistence, real-time snapshot listeners, LINQ queries, and managed Firebase Auth identity.
auto_invoke: true
triggers:
  - firestore mobile
  - mobile firestore
  - on-device firestore
  - native firestore
  - offline firestore
  - Shiny.DocumentDb.Firestore.Mobile
  - MobileFirestoreDocumentStore
  - MobileFirestoreOptions
  - AddMobileFirestoreDocumentStore
  - AddFirebaseIdentity
  - IFirebaseIdentity
  - FirebaseRestIdentity
  - FirebaseIdentityOptions
  - FirebaseUser
  - PersistenceEnabled
  - EmulatorHost
  - firestore emulator
  - firestore offline
  - firestore realtime
  - firestore snapshot listener
  - firebase auth maui
  - firebase anonymous sign in
---

# Shiny Mobile Firestore Provider

## Overview

`Shiny.DocumentDb.Firestore.Mobile` is an **on-device** Firebase Firestore provider for
[Shiny.DocumentDb](https://shinylib.net). It binds the **native** Firebase Firestore SDK (via a first-party
Shiny binding, *not* a third-party one) and adapts it to the `IDocumentStore` contract.

The native SDK owns the hard parts — local cache, offline write queue, real-time listeners, backoff,
conflict handling. This provider is a thin typed adapter over it.

### This is NOT `Shiny.DocumentDb.Firestore`

Do not confuse the two packages. Pick by **where the code runs**:

| | `Shiny.DocumentDb.Firestore` | `Shiny.DocumentDb.Firestore.Mobile` (this skill) |
|---|---|---|
| SDK | `Google.Cloud.Firestore` (admin/gRPC) | Native Firebase Firestore SDK (binding) |
| Runs where | Server / backend host | On the client device |
| Auth | Service account (ADC) | Firebase Auth, per end-user |
| Security rules | Bypassed (admin) | Enforced (`request.auth.uid`) |
| Offline | None | Native persistent cache — offline by default |
| Registration | `AddFirestoreDocumentStore(...)` | `AddMobileFirestoreDocumentStore(...)` |

**Never embed a service-account key in a shipped app** — that is what the server package is for.

## Platform support — read this first

| TFM | Behavior |
|---|---|
| `net10.0-android` | Real adapter over the native SDK. Verified end-to-end against the Firestore emulator. |
| `net10.0-ios` | Real adapter over the native SDK (slim Swift binding). Verified end-to-end against the Firestore emulator. |
| `net10.0` | **Throw-stub.** Every operation throws `PlatformNotSupportedException`. Exists so the surface is unit-testable without a device. |

Both mobile heads are at **feature parity** — the same operations work and the same ones throw, and both pass
the same 9-case emulator harness (CRUD, real-time, query/order/count/delete, identity). The API below applies
identically to Android and iOS. Anything other than these two TFMs throws `PlatformNotSupportedException`, so
multi-targeted app code must guard the mobile paths (`#if ANDROID || IOS`) or avoid resolving the store
elsewhere.

## Installation

```bash
dotnet add package Shiny.DocumentDb.Firestore.Mobile
```

The Android head pulls AndroidX / Guava / DataStore from nuget.org. If the consuming app inherits a
user-level private NuGet source, add a `nuget.config` that clears sources back to nuget.org.

## Setup

### 1. Initialize FirebaseApp

The store resolves Firebase in its constructor and **throws `InvalidOperationException` if Firebase is not
initialized**. Bundle the platform config file — `google-services.json` (Android) / `GoogleService-Info.plist`
(iOS) — for auto-init, or initialize explicitly before the store is resolved. On iOS, setting `ProjectId` +
`AppId` on the options lets the provider configure Firebase itself; a config file already loaded by the app
always wins.

### 2. Register the store

```csharp
using Shiny.DocumentDb;

builder.Services.AddMobileFirestoreDocumentStore(o =>
{
    o.ProjectId = "my-project";              // optional when google-services.json is bundled
    o.PersistenceEnabled = true;             // default — offline cache on
    o.MapTypeToCollection<Play>("plays");    // default collection = type name
});
```

This registers one singleton exposed as four contracts — all the **same instance**:

- `IDocumentStore` — CRUD + `Query<T>()`
- `IDocumentMaintenance`
- `IObservableDocumentStore` — `NotifyOnChange<T>()`
- `IChangeFeedDocumentStore` — `SubscribeChanges<T>()`

### 3. Register identity (optional)

```csharp
builder.Services.AddFirebaseIdentity(o => o.ApiKey = "your-web-api-key");
```

## Document requirements

- Each document type maps to its own collection — the type name by default, overridable with
  `MapTypeToCollection<T>`.
- The document **id is the Firestore document id**. It comes from a property named `Id` by default
  (`MapIdProperty<T>` to override), matched **case-insensitively** against the serialized JSON.
- The id must be present and non-empty on every write — an empty id throws `InvalidOperationException`.
  This provider does **not** generate ids for you.
- Documents are stored as native Firestore field maps converted from `System.Text.Json`. **Field names are
  the JSON property names** — if you set a `PropertyNamingPolicy`, queries resolve against the converted
  name automatically, but hand-written field strings must match the JSON name.

```csharp
class Play
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Version { get; set; }
}
```

## CRUD

```csharp
var store = sp.GetRequiredService<IDocumentStore>();

await store.Insert(new Play { Id = "p1", Name = "Slant Left", Version = 1 });
var play = await store.Get<Play>("p1");          // null when absent
await store.Upsert(new Play { Id = "p1", Name = "Slant Right", Version = 2 });
await store.Remove<Play>("p1");                  // always returns true — Firestore delete is idempotent
var cleared = await store.Clear<Play>();         // deletes the collection doc-by-doc, returns the count
```

**`Insert` is an upsert.** `Insert`, `Update`, and `Upsert` all map to native Firestore `set()`, which
overwrites. There is no insert-if-absent semantic yet — `Insert` on an existing id silently replaces it. If a
user needs true insert-if-absent, `Get` first (racy) or wait for the transactional milestone.

## Querying

`Query<T>()` returns an `IDocumentQuery<T>` backed by a native Firestore query. Filters, ordering, and limit
push down to the native query; **aggregates and pagination offset are applied client-side over materialized
results**.

```csharp
var plays = await store
    .Query<Play>()
    .Where(p => p.Version >= 2)
    .OrderBy(p => p.Version)
    .ToList();

var count = await store.Query<Play>().Count();
var any   = await store.Query<Play>().Where(p => p.Name == "Alpha").Any();
var n     = await store.Query<Play>().Where(p => p.Version >= 2).ExecuteDelete();
await store.Query<Play>().ExecuteUpdate(p => p.Name, "Renamed");
```

Supported: `Where`, `OrderBy`, `OrderByDescending`, `Paginate`, `ToList`, `ToAsyncEnumerable`, `Count`,
`Any`, `ExecuteDelete`, `ExecuteUpdate`, `Max`, `Min`, `Sum`, `Average`, `NotifyOnChange`,
`IgnoreQueryFilters`.

Translated predicate operators: `==`, `!=`, `<`, `<=`, `>`, `>=`, `&&`, and `ICollection.Contains`.
Anything else throws.

### Query gotchas

- **Firestore rule: with an inequality filter, the first `OrderBy` must be on the inequality field.**
  `.Where(p => p.Version >= 2).OrderBy(p => p.Version)` is valid; ordering by a different field first fails
  at the native SDK.
- `Count()` on a query **materializes every document** — it is not a native aggregate. `Count<T>()` on the
  store does the same. Avoid on large collections.
- `Paginate(offset, take)` issues a native `Limit(offset + take)` and skips `offset` client-side — Firestore
  has no offset. Deep pagination reads everything up to the offset.
- `Max`/`Min`/`Sum`/`Average` materialize and compute in managed code.
- `Select` projection throws `NotSupportedException` — `ToList` and project client-side.
- `IgnoreQueryFilters(params string[])` ignores the names and drops **all** filters (all-or-nothing today).
- Grouping, cursor paging, vector, and full-text throw.

## Real-time

Both are backed by native snapshot listeners. **The initial snapshot is skipped** — you receive changes from
the point of subscription, not the current contents. Read with `Get`/`ToList` first if you need a baseline.

```csharp
// Callback style — dispose the handle to detach the listener
var sub = await changeFeedStore.SubscribeChanges<Play>((change, ct) =>
{
    Console.WriteLine($"{change.ChangeType}: {change.Id} {change.Document?.Name}");
    return Task.CompletedTask;
});
await sub.DisposeAsync();

// Stream style — the listener detaches when enumeration ends
await foreach (var change in observableStore.NotifyOnChange<Play>(ct))
    Console.WriteLine(change.ChangeType);

// Scoped to a filtered query
await foreach (var change in store.Query<Play>().Where(p => p.Version > 1).NotifyOnChange(ct))
    Console.WriteLine(change.Id);
```

`change.ChangeType` is `DocumentChangeType.Inserted` / `Modified` / `Removed`.

## Offline

`PersistenceEnabled` (default `true`) turns on the native persistent cache — reads are cache-first, writes
queue locally and drain automatically on reconnect. This is the entire point of the mobile provider; leave it
on unless a test needs a clean slate.

Because writes queue offline, an `await store.Insert(...)` that completes does **not** guarantee the server
has the write — only that the native SDK accepted it.

## Identity

`IFirebaseIdentity` is a **managed** implementation over the Firebase Auth REST API (no native binding).

```csharp
var identity = sp.GetRequiredService<IFirebaseIdentity>();

var user = await identity.SignInAnonymouslyAsync();
await identity.SignUpWithEmailPasswordAsync("a@b.com", "Passw0rd!");
await identity.SignInWithEmailPasswordAsync("a@b.com", "Passw0rd!");

var token = await identity.GetIdTokenAsync();   // auto-refreshes within 1 min of expiry
var uid   = identity.CurrentUserId;
identity.AuthStateChanged += (_, u) => { /* u is null on sign-out */ };
identity.SignOut();
```

`FirebaseUser` is a record: `Uid`, `IdToken`, `RefreshToken`, `ExpiresAt`, `IsAnonymous`, `Email`.

### Critical limitation — the token does not reach the native SDK

The identity service obtains and refreshes the user's token **in managed code only**. It is *not* wired into
the native Firestore SDK's request auth, so **Firestore security rules do not see `request.auth.uid` for
native reads and writes yet**. That integration needs a native `signInWithCustomToken` and is deferred with
the native auth binding.

Until then, **scope per-user data by collection path**:

```csharp
o.MapTypeToCollection<Play>($"users/{uid}/plays");
```

Do not tell users their rules are enforced per-user today, and do not generate rules that rely on
`request.auth.uid` for this provider's traffic without flagging this gap.

## Options reference — `MobileFirestoreOptions`

| Member | Notes |
|---|---|
| `ProjectId` / `AppId` / `ApiKey` | Optional when a platform config file is bundled; takes precedence for explicit init |
| `PersistenceEnabled` | Default `true` — native offline cache |
| `EmulatorHost` | `host:port`, points the native SDK at the emulator |
| `TypeNameResolution` | Default `ShortName` |
| `JsonSerializerOptions` | Drives field names and (de)serialization |
| `UseReflectionFallback` | Default `true`; set `false` for iOS full-AOT |
| `Logging` | `Action<string>` diagnostic callback |
| `MapTypeToCollection<T>(name)` | Override the collection (default: type name) |
| `MapIdProperty<T>(x => x.MyId)` | Override the id property (default: `Id`) |
| `MapIdType<TId>(...)` | Custom id CLR types beyond Guid/int/long/string |
| `AddQueryFilter<T>(predicate)` | Global filter, auto-applied to every query; named overload available |
| `MapVersionProperty<T>(...)` | **Registered but not enforced today** — see below |
| `AddInterceptor` / `AddBulkInterceptor` / `OnBeforeWrite<T>` / `OnAfterWrite<T>` | **Registered but not invoked today** — see below |

## Not implemented yet — these throw

Calling any of these on Android throws `NotSupportedException` ("planned for a later milestone"). **Do not
generate code that uses them**; suggest the listed workaround instead.

| API | Workaround |
|---|---|
| `Query<T>(string whereClause, ...)` | Use the LINQ `Query<T>()` overload |
| `QueryStream<T>(string whereClause, ...)` | `Query<T>().ToAsyncEnumerable()` |
| `Count<T>(whereClause: ...)` | `Query<T>().Where(...).Count()` |
| `BatchInsert<T>` | Loop `Insert` (each is a round trip) |
| `SetProperty<T>` / `RemoveProperty<T>` | `Query<T>().ExecuteUpdate(...)`, or read-modify-write |
| `GetDiff<T>` | Compute client-side |
| `ClearAll()` | `Clear<T>()` per type |
| `Select` projection | `ToList()` then project in managed code |

### Configurable but inert (silent — no exception)

These are the dangerous ones: they compile, they configure, they **do nothing** on the Android path today.

- **Write interceptors.** `AddInterceptor`, `AddBulkInterceptor`, `OnBeforeWrite<T>`, `OnAfterWrite<T>` are
  stored on the options and exposed via the base pipeline, but the Android adapter's write path never invokes
  them. Do not use interceptors for auditing, validation, or timestamping on this provider — put that logic in
  the calling code.
- **Optimistic concurrency.** `MapVersionProperty<T>` records the mapping, but the write path is a plain
  `set()` — no version read, no compare, no increment, and `ConcurrencyException` is never thrown. A stale
  write silently wins. (The XML doc on `MapVersionProperty` describes the intended transactional behavior, not
  today's.) Do not present version mapping as working concurrency control here.

## Emulator testing

```csharp
var opts = new MobileFirestoreOptions
{
    ProjectId = "demo-shiny",
    EmulatorHost = "10.0.2.2:8080",   // host loopback from the Android emulator
    PersistenceEnabled = false        // clean slate per run
};
```

`10.0.2.2` is the Android emulator's alias for the host machine — `localhost` will not reach it. **On the iOS
simulator use `localhost` instead**, since it shares the host's network stack. The Auth emulator equivalent is
`FirebaseIdentityOptions.AuthEmulatorHost` (`10.0.2.2:9099` / `localhost:9099`). The Firestore emulator accepts
any demo project id and fake API key.

## Code generation rules

1. **Android and iOS only.** Any other TFM resolves the throw-stub.
2. **Use `AddMobileFirestoreDocumentStore`**, never `AddFirestoreDocumentStore` (that is the server package).
3. **Always set an id** on documents before writing — the provider never generates one.
4. **Never generate calls to the throwing APIs** in the table above.
5. **Never rely on interceptors or version mapping** for correctness on this provider.
6. **Order by the inequality field first** when a query has an inequality filter.
7. **Skip the initial snapshot** — pair a change subscription with an initial read when a baseline is needed.
8. Ensure `FirebaseApp` is initialized before the store resolves.

## Key source files

- `src/Shiny.DocumentDb.Firestore.Mobile/MobileFirestoreServiceCollectionExtensions.cs` — DI registration
- `src/Shiny.DocumentDb.Firestore.Mobile/MobileFirestoreOptions.cs` — options / mapping API
- `src/Shiny.DocumentDb.Firestore.Mobile/FirebaseIdentity.cs` — `IFirebaseIdentity` + REST implementation
- `src/Shiny.DocumentDb.Firestore.Mobile/Platforms/Android/` — Android adapter, LINQ→native query, change feed
- `src/Shiny.DocumentDb.Firestore.Mobile/Platforms/iOS/` — iOS adapter (same shape, over the slim Swift binding)
- `src/Shiny.DocumentDb.Firestore.Mobile/MobileFirestoreDocumentStore.Stub.cs` — `net10.0` throw-stub
- `firebase_ios/native/firestore/ShinyFirebaseFirestore/Firestore.swift` — the iOS slim `@objc` surface
- `src/Shiny.Firebase.Firestore.iOS.Binding/ApiDefinitions.cs` — binds that surface
- `samples/FirestoreEmulatorSample/MainActivity.cs` — end-to-end Android emulator harness
