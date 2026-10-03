---
name: shiny-actors
description: Generate code using Shiny.Actors — Orleans-style virtual actors without the ceremony, AOT/trim-clean, running anywhere .NET runs including .NET MAUI and Blazor WebAssembly. Covers actor interfaces and source-generated proxies, the ShinyActorBuilder (AddShinyActors), persistent state with ETags, named states and auto-save, event-sourced JournaledActor with snapshots, state and event migrations, typed streams (durable, implicit consumers), timers and persistent reminders (background jobs via Shiny.Jobs, on-time OS notifications via Shiny.Notifications), concurrency control ([Reentrant], [AlwaysInterleave], [ReadOnly], [StatelessWorker]), call filters and request context, OpenTelemetry traces and metrics, remoting over HTTP (Shiny.Net.HttpServer server, RemoteActorSystem client), mDNS discovery of actor servers, Shiny.DocumentDb storage (SQLite, PostgreSQL, IndexedDB...), and the ActorTestHost testing kit.
auto_invoke: true
triggers:
- Shiny.Actors
- actor
- actors
- virtual actor
- grain
- Orleans
- IActor
- Actor<TState>
- JournaledActor
- ActorSystem
- IActorSystem
- AddShinyActors
- ShinyActorBuilder
- IActorState
- ActorState
- AutoSave
- StateVersion
- AddStateMigration
- IActorStateProvider
- IActorEventStore
- IActorStream
- GetStream
- AddDurableStream
- ReadFromAsync
- IActorStreamConsumer
- IRemindable
- RegisterReminderAsync
- ReminderNotification
- UseBackgroundReminders
- UseReminderNotifications
- RegisterTimer
- OneWay
- Reentrant
- AlwaysInterleave
- ReadOnly
- StatelessWorker
- IActorCallFilter
- ActorRequestContext
- ActorTelemetry
- RemoteActorSystem
- MapActors
- ServeOverHttp
- UseDiscovery
- PublishActorsAsync
- BrowseActorPeersAsync
- UseDocumentDb
- ActorTestHost
- ActorDeadlockException
- ActorStateConflictException
---

# Shiny.Actors Skill

You are an expert in Shiny.Actors: Orleans-style virtual actors that run in one process (a phone, a browser tab, a
server). Other processes and devices can call them over HTTP.

**Documentation**: https://shinylib.net/actors

## When to use this skill

- The user wants actors, grains, or "one object per id that handles one call at a time".
- The user wants state that persists per entity without writing a repository, or event sourcing.
- The user wants per-entity timers or reminders, or typed pub/sub between parts of an app.
- The user is building a MAUI / Blazor / console app and mentions Orleans but can't run a cluster.
- The user wants to call objects on another device on the LAN.

**Not for**: Microsoft Orleans itself (a different library with similar names), Akka.NET, or Proto.Actor.

## The rules that never bend

1. **Nothing is discovered by reflection.** The source generator writes proxies and registrations. JSON goes through
   `JsonTypeInfo` from the app's own `JsonSerializerContext`. Every state type, event type, and type crossing a
   remote call must be declared there with `[JsonSerializable(typeof(...))]`. Never use reflection-based
   `JsonSerializer` overloads, `Activator`, or `DispatchProxy`.
2. **Actor interface methods are asynchronous.** They return `Task`, `Task<T>`, `ValueTask` or `ValueTask<T>`. No
   properties, generic methods, or ref/out/in parameters. An optional trailing `CancellationToken` flows to the actor.
3. **Implementations derive from `Actor`, `Actor<TState>` or `JournaledActor<TState, TEvent>`** and implement the
   interface. They're never constructed by hand: `actors.Get<IFoo>(id)` returns a proxy.
4. **One call at a time per actor.** Fields need no locks. Don't use `ConfigureAwait(false)` inside an actor that
   interleaves (`[Reentrant]`, `[AlwaysInterleave]`, `[ReadOnly]`): it leaves the actor's scheduler.
5. **All registration goes through `AddShinyActors(x => ...)`.** Add-on packages are extension methods on
   `ShinyActorBuilder`. Never generate `services.AddActors(...)`, `AddActorReminderJob`,
   `AddActorReminderNotifications` or `AddActorDocumentDbStorage`; those don't exist.

## Packages

| Package | Adds | Builder method |
|---|---|---|
| `Shiny.Actors` | Runtime, generator, remote client (`RemoteActorSystem`) | `AddShinyActors` |
| `Shiny.Actors.DocumentDb` | State, reminders and event logs on Shiny.DocumentDb | `UseDocumentDb(...)` |
| `Shiny.Actors.HttpServer` | Serve actors over Shiny.Net.HttpServer | `ServeOverHttp(...)` |
| `Shiny.Actors.Jobs` | Fire reminders from OS background jobs | `UseBackgroundReminders()` |
| `Shiny.Actors.Notifications` | Reminders become on-time OS notifications | `UseReminderNotifications()` |
| `Shiny.Actors.Discovery` | Find actor servers on the LAN (mDNS) | `UseDiscovery()` |
| `Shiny.Actors.Testing` | `ActorTestHost` | (test code) |

## Setup

```csharp
builder.Services.AddShinyActors(actors => actors
    .UseDocumentDb(new SqliteDatabaseProvider($"Data Source={Path.Combine(FileSystem.AppDataDirectory, "app.db")}"))
    .UseAutoSave()                                  // write changed state after every call
    .Configure(o => o.IdleTimeout = TimeSpan.FromMinutes(5)));
```

- **Repeated calls add up.** Calling `AddShinyActors` twice adds to one configuration.
- **Container-built pieces.** `AddCallFilter<T>()`, `UseStateProvider<T>()` and `AddReminderObserver<T>()` are created
  by the container.
- **Late configuration.** `Configure((options, services) => ...)` is for things that only exist once the container
  is built.
- **No container?** `await using var actors = new ActorSystem(new ActorSystemOptions { ... });`

## Defining an actor

```csharp
public interface ICart : IActor
{
    Task Add(string sku, int quantity);
    Task<IReadOnlyDictionary<string, int>> Items();
    [OneWay] Task Clear();                       // queued; the caller doesn't wait
}

public class CartState { public Dictionary<string, int> Items { get; set; } = []; }

public class CartActor(ILogger<CartActor> logger) : Actor<CartState>, ICart   // constructor DI works
{
    public async Task Add(string sku, int quantity)
    {
        this.State.Items[sku] = this.State.Items.GetValueOrDefault(sku) + quantity;
        await this.WriteStateAsync();            // or [AutoSave] on the class
    }

    public Task<IReadOnlyDictionary<string, int>> Items() => Task.FromResult<IReadOnlyDictionary<string, int>>(this.State.Items);
    public Task Clear() => this.ClearStateAsync().AsTask();
}

[JsonSerializable(typeof(CartState))]
[JsonSerializable(typeof(IReadOnlyDictionary<string, int>))]   // crosses a remote call
public partial class AppJson : JsonSerializerContext;
```

```csharp
var cart = actors.Get<ICart>("user-42");          // nothing activates until the first call
await cart.Add("coffee", 2);
```

- **Lifecycle.** `OnActivateAsync`, `OnDeactivateAsync(reason)`, `DeactivateOnIdle()`, `this.Id`, `this.Actors` (to
  call other actors), `this.Logger`, `this.TimeProvider`.
- **Pinning names.** `[ActorName("cart")]` pins the name that keys stored state; set it before shipping.
- **Ids.** Any string is a valid id, and `Get<T>(Guid)` / `Get<T>(long)` exist.

## State

```csharp
[AutoSave]
public class WalletActor(
    [ActorState("balance")] IActorState<Balance> balance,   // named states, stored separately
    [ActorState("history")] IActorState<History> history
) : Actor, IWallet
{
    public Task Deposit(decimal amount) { balance.State.Amount += amount; return Task.CompletedTask; }
}
```

- **ETags.** Every write is conditional on what was read. A concurrent writer causes `ActorStateConflictException`,
  and the actor deactivates and reloads on its next call. Don't catch and retry the write blindly.
- **`[AutoSave]`** (= `AfterEachCall`) saves changed state before the caller gets the result. A throwing call isn't
  saved. `[AutoSave(AutoSaveMode.OnDeactivate)]` saves only on deactivation.
- **Storage.** In-memory (default), `UseFileStorage(dir)`, or `UseDocumentDb(...)`. Per actor:
  `[StateProvider("name")]`; per state: `[ActorState("x", Provider = "name")]`.
- **Shapes change? Version them:** `[StateVersion(2)]` on the type, plus
  `actors.AddStateMigration<T>(fromVersion, json => ...)`.

## Event sourcing

```csharp
[AutoSave]   // confirms raised events after each call
public class AccountActor : JournaledActor<AccountState, AccountEvent>, IAccount
{
    protected override int SnapshotEvery => 100;
    protected override void Apply(AccountState s, AccountEvent e) { /* deterministic */ }
    public Task Deposit(decimal amount) { this.RaiseEvent(new Deposited(amount)); return Task.CompletedTask; }
}

[JsonPolymorphic, JsonDerivedType(typeof(Deposited), "deposited"), JsonDerivedType(typeof(Withdrawn), "withdrawn")]
public abstract record AccountEvent;
```

- **Polymorphic events** use `[JsonPolymorphic]`/`[JsonDerivedType]` on the base. Both `TState` and `TEvent` go in
  the `JsonSerializerContext`.
- **History** is available through `ReadEventsAsync()`. Stored events are never rewritten; old shapes are upcast on
  read with `AddStateMigration<TEvent>`.

## Streams

```csharp
await actors.GetStream<OrderPlaced>("store-1").PublishAsync(new OrderPlaced("coffee", 2));
await foreach (var o in actors.GetStream<OrderPlaced>("store-1").ReadAllAsync(ct)) { }   // e.g. a view model

// inside an actor: each event is a turn of that actor; the subscription ends with the activation
this.Actors.GetStream<OrderPlaced>("store-1").Subscribe((o, ct) => { /* ... */ return default; });

// implicit: events on key "store-1" activate the actor whose id is "store-1"
public class StoreSales : Actor<Sales>, IStoreSales, IActorStreamConsumer<OrderPlaced> { /* OnNextAsync */ }
```

- **Default delivery** is at most once, with nothing stored.
- **Durable streams.** `actors.AddDurableStream<T>(retain)` keeps the last N events, numbered.
  `ReadFromAsync(afterSequence)` replays them, then continues live; remote readers resume after reconnects.

## Timers and reminders

- **Timers.** `RegisterTimer(callback, due, period)`, called from inside the actor. Ticks run as turns, aren't
  persisted, and stop at deactivation.
- **Reminders** are persistent. The actor implements `IRemindable`, and they activate it when due:

```csharp
await this.RegisterReminderAsync("invoice", TimeSpan.FromHours(1), TimeSpan.FromDays(1),
    notification: new ReminderNotification("Invoice", "Ready to send"));   // optional OS notification
```

- **Mobile.** `actors.UseBackgroundReminders()` fires them from OS jobs, every 15+ minutes and only roughly on time.
  `actors.UseReminderNotifications()` shows the notification exactly on time, app running or not.
- **Persistent storage.** Reminders need persistent storage (`UseDocumentDb`/`UseFileStorage`); in-memory is empty
  after the OS restarts the app.
- **Passing the token.** `notification` comes before the `CancellationToken`, so pass the token by name:
  `cancellationToken: ct`.

## Concurrency control

| | Use |
|---|---|
| default | One call at a time |
| `[Reentrant]` (class) | Calls interleave at awaits; the actor can call itself |
| `[AlwaysInterleave]` (method) | Status, cancel or health checks that must not queue behind long work |
| `[ReadOnly]` (method) | Reads run together, never beside a write. Only mark methods that truly don't change state. |
| `[StatelessWorker(MaxLocalWorkers = n)]` (class) | Parallel activations per id. No state or reminders (SACT011). |

- **Deadlocks.** A call that waits on itself through other actors throws `ActorDeadlockException` naming the path. Fix
  it by restructuring, making a call `[OneWay]`, or marking it `[AlwaysInterleave]`.

## Pipeline

- **Call filters.** `actors.AddCallFilter<T>()` or `AddCallFilter(instance)`. An `IActorCallFilter` sees `ctx.Method`,
  `ctx.Arguments`, `ctx.ActorId`, and a settable `ctx.Result`. It runs for local and remote calls, asks and one-way.
  An actor implementing `IActorCallFilter` filters its own calls.
- **Request context.** `ActorRequestContext.Set("tenant", id)` flows into calls, onward to other actors, and across
  remote calls.
- **Telemetry.** `AddSource(ActorTelemetry.Name)` / `AddMeter(ActorTelemetry.Name)`. Traces stay connected across
  actors and HTTP.

## Remoting

```csharp
// server - nothing is reachable unless exposed; put authentication in front
actors.ServeOverHttp(
    expose => expose.Expose<ICart>().ExposeStream<OrderPlaced>(),
    http => { /* address, AddAuthentication().AddApiKey(...), UseAuthentication/UseAuthorization */ },
    authorizationPolicies: []);

// client - same IActorSystem
IActorSystem remote = new RemoteActorSystem(new HttpClient { BaseAddress = new("http://192.168.1.20:8080/actors/") });
```

- **Errors.** `RemoteActorException.StatusCode` is 400 (bad arguments), 401/403, 404 (not exposed), 409 (deadlock) or
  500 (the actor threw; no details unless `IncludeExceptionDetails`).
- **LAN access.** The server must bind the LAN for other devices (`Address = IPAddress.Any`); Shiny.Net.HttpServer
  binds loopback by default.
- **Discovery.** `actors.UseDiscovery()`, then `mdns.PublishActorsAsync(name, port)` on the server and
  `mdns.BrowseActorPeersAsync(ct)` → `peer.Connect(...)` on the client. Apple platforms need `_shinyactors._tcp` in
  `NSBonjourServices` plus `NSLocalNetworkUsageDescription`.

## Testing

```csharp
await using var host = new ActorTestHost(
    services => services.AddSingleton<IClock, FakeClock>(),
    actors => actors.UseReminderNotifications());

await host.SetStateAsync<CartActor, CartState>(new CartState { Items = { ["tea"] = 1 } }, id: "u1");
await host.Get<ICart>("u1").Add("coffee", 2);
await host.AdvanceAsync(TimeSpan.FromHours(1));   // timers, reminders, idle deactivation - no sleeps
Assert.Contains(host.Calls, c => c.Method == "Add");
```

## Build diagnostics

| Code | Meaning |
|---|---|
| SACT001 | Interface member can't be proxied (property, generic method, ref/out/in) |
| SACT002 | Method isn't Task/ValueTask-returning |
| SACT003 | `[OneWay]` method returns a value |
| SACT004 | Class implements an actor interface but doesn't derive from `Actor` |
| SACT005 | Actor can't be constructed (generic, private, ambiguous constructors) |
| SACT006 | *Warning*: two classes implement one interface; choose with `actors.AddActor<IFoo, Foo>()` |
| SACT007 | *Warning*: a state or event type isn't on any `JsonSerializerContext` |
| SACT008 | Actor interface is generic or not reachable |
| SACT009 | Two actors share an `[ActorName]` |
| SACT010 | `[ActorState]` parameter isn't an `IActorState<T>` |
| SACT011 | `[StatelessWorker]` with persistent state |

## Gotchas to mention when relevant

- **SQLite security pin.** Shiny.DocumentDb.Sqlite pulls in a vulnerable SQLitePCLRaw. Pin
  `SQLitePCLRaw.lib.e_sqlite3.android` on Android, `.ios` on iOS, and the plain package elsewhere. **Never** add the
  plain one to an Android target: its Linux `.so` crashes the app at startup.
- **Blazor IndexedDB.** Register an `IndexedDbDocumentStore` with `options.MapActorDocuments()`, then call
  `actors.UseDocumentDb()` with no arguments.
- **Saving on mobile.** Use `[AutoSave]`, or call `DeactivateAllAsync()` when the app sleeps; the OS kills
  backgrounded apps without warning.
