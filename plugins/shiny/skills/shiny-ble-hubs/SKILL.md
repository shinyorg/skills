---
name: shiny-ble-hubs
description: Generate code using Shiny.BluetoothLE.Hubs - SignalR style hubs over Bluetooth LE for .NET (iOS/Android/MAUI) with a source generated client proxy, typed host-to-client pushes, groups, streaming, per hub start/stop and L2CAP file transfers
auto_invoke: true
triggers:
  - ble hubs
  - bluetoothle hubs
  - bluetooth le hubs
  - Shiny.BluetoothLE.Hubs
  - ble hub
  - bluetooth hub
  - signalr over ble
  - signalr bluetooth
  - device to device ble
  - BleHub
  - BleHubClient
  - BleHubClientAttribute
  - IBleHubClient
  - IBleHubConnection
  - IBleHubHost
  - IHubContext
  - BleHubPush
  - AddBleHubServer
  - BleHubServerBuilder
  - AddBleHubClient
  - BleHubClientOptions
  - BleHubHostOptions
  - BleHubProtocolOptions
  - ServiceUuid
  - DefaultServiceUuid
  - BleHubConnectOptions
  - BleHubHostInfo
  - BleHubCallerContext
  - BleHubConnectedClient
  - EnableFileTransfers
  - IBleHubFileHandler
  - IBleHubSerializer
  - BleHubRemoteException
  - BleHubDisconnectedException
  - HubDisconnect
  - HubDisconnectReason
  - BleHubClientDisconnectedEventArgs
  - BleHubStatusChangedEventArgs
  - OnDisconnectedAsync
  - disconnect reason
  - why a client disconnected
  - rename
  - change name
  - rename client
  - host rename
  - OnRenamedAsync
  - ClientRenamed
  - BleHubClientRenamedEventArgs
  - HostRenamed
  - ClientName
  - RenameRefused
  - IBleHubTransportEndpoint
  - IBleHubClientTransport
  - ConnectExternal
  - ble hubs over wifi
---

# Shiny.BluetoothLE.Hubs Skill

You are an expert in Shiny.BluetoothLE.Hubs, a library that lets one device **host** SignalR style hubs over Bluetooth LE and
other devices **connect** to them as clients. It sits on top of Shiny.BluetoothLE (client) and Shiny.BluetoothLE.Hosting
(host).

- A hub contract is one interface marked `[BleHubClient]`. Its **methods** are client -> host calls and its
  **events** are host -> client pushes.
- The host implements the contract in a `BleHub<TContract>` class.
- A source generator emits a strongly typed client proxy, the hub dispatcher and the typed push methods. The output is
  AOT- and trim-safe, with no reflection.
- Files move over a separate L2CAP channel through dedicated client methods, not through hub calls.

## When to Use This Skill

Invoke this skill when the user wants to:
- Exchange calls, results or events between two or more nearby phones/tablets over BLE
- Build a device-to-device app (games, multiplayer, peer sync, kiosk + companion) without a server
- Define a hub contract, implement a hub, or call a hub from a client
- Push events from the host to all clients, one client, others or groups
- Stream results from the host (`IAsyncEnumerable<T>`)
- Start or stop a hub, disconnect a client, limit or validate clients
- Tell why a connection ended (client left, link lost, kicked, host stopped) with `HubDisconnectReason`
- Rename a client or the host without reconnecting, and tell other clients about it
- Upload or download files between devices over L2CAP

Do NOT use this skill for talking to third party BLE peripherals (use `shiny-bluetoothle`) or for raw GATT servers
(use `shiny-ble-hosting`).

## Library Overview

| NuGet | Purpose |
|---|---|
| `Shiny.BluetoothLE.Hubs` | Protocol, serializer, `[BleHubClient]`, **includes the source generator** |
| `Shiny.BluetoothLE.Hubs.Host` | `BleHub<T>`, `IBleHubHost`, `IHubContext<THub>`, groups, L2CAP file server |
| `Shiny.BluetoothLE.Hubs.Client` | `BleHubClient` (base of generated proxies), discovery, connections, file transfer |

- **Namespace**: `Shiny.BluetoothLE.Hubs`
- **Platforms**: iOS and Android in both roles, and Mac Catalyst. Windows can be a client but can't host. L2CAP needs
  Android API 29+.
- **Platform BLE stacks are registered for you.** On Android, iOS and Mac Catalyst, `AddBleHubServer` calls
  `AddBluetoothLeHosting()` and `AddBleHubClient` calls `AddBluetoothLE()`, so don't add them yourself. On Apple the
  client turns off iOS's background alerts, because hubs are foreground only; to use your own `AppleBleConfiguration`,
  call `AddBluetoothLE(config)` *before* `AddBleHubClient` (the first registration wins). On plain `net10.0`, register an
  `IBleHostingManager` / `IBleManager` yourself.

## Code Generation Instructions

### 1. The contract (shared by host and client)

```csharp
using Shiny.BluetoothLE.Hubs;

[BleHubClient]
public interface IGameHub
{
    // client -> host: Task, Task<T> or IAsyncEnumerable<T> only
    Task<JoinResult> Join(string playerName);
    Task<MoveResult> MakeMove(int cell);
    Task Rematch();
    // a trailing CancellationToken is passed to the host and cancels the hub method there - it is never serialized
    IAsyncEnumerable<int> Countdown(int from, CancellationToken cancellationToken);

    // host -> clients: Action or Action<T1..T4> only, fire-and-forget
    event Action<GameState> StateChanged;
    event Action<string, string> Emote;
}
```

Rules (violations are compile errors):
- **SBH001**: the hub is missing a contract method, or its signature doesn't match.
- **SBH002**: a method returns something other than `Task`, `Task<T>` or `IAsyncEnumerable<T>`.
- **SBH003**: overloaded method names. Methods are called by name, so each name must be unique.
- **SBH004**: an event isn't `Action` / `Action<...>`, or has more than 4 arguments.
- **SBH005**: `BleHub<T>` where `T` isn't an interface marked `[BleHubClient]`.
- **SBH006**: generic methods, `ref`/`out`/`in` parameters, or more than 255 parameters.

Every parameter, result and event argument type must be serializable. The default serializer is Shiny's AOT JSON, so
put every type in a source generated context and register it:

```csharp
[JsonSerializable(typeof(JoinResult))]
[JsonSerializable(typeof(MoveResult))]
[JsonSerializable(typeof(GameState))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
public partial class GameJsonContext : JsonSerializerContext;

Shiny.Json.AddContext(GameJsonContext.Default);   // at startup, before any hub call
```

Primitives used as arguments, such as `string` and `int`, need entries too, because each argument is serialized on its
own.

### 2. The hub (host)

```csharp
public class GameHub(GameEngine engine) : BleHub<IGameHub>
{
    public override Task OnConnectedAsync()
        => this.Groups.AddToGroupAsync(this.Context.ConnectionId, "lobby");

    // or override OnDisconnectedAsync(string? reason) when you only need the text
    public override Task OnDisconnectedAsync(HubDisconnect disconnect)
    {
        if (disconnect.Reason == HubDisconnectReason.ClientTimeout)
            engine.MarkAway(this.Context.ConnectionId);   // the link dropped - they may be back
        else
            engine.Leave(this.Context.ConnectionId);
        return this.Clients.All.StateChanged(engine.Snapshot());
    }

    public Task<JoinResult> Join(string playerName) => ...;

    public async Task<MoveResult> MakeMove(int cell)
    {
        var error = engine.TryMove(this.Context.ConnectionId, cell);
        if (error != null)
            return new MoveResult(false, error);

        await this.Clients.All.StateChanged(engine.Snapshot());   // generated typed push
        return new MoveResult(true, null);
    }

    public Task Rematch() => ...;

    // the hub may leave out the contract's CancellationToken, or take it as the last parameter
    public async IAsyncEnumerable<int> Countdown(int from, [EnumeratorCancellation] CancellationToken ct) { ... }
}
```

- **Hub lifetime**: a new hub instance is created, in its own DI scope, for every call, as in SignalR. Never keep
  state in hub fields. Use singletons, or `Context.Items` for per-connection state.
- **No `partial` needed.** The generator checks the hub against the contract.
- **`Context`**: `ConnectionId`, `Client` (`Name`, `AppVersion`, `Properties`, `Mtu`, `ConnectedAt`, `Items`),
  `ConnectionAborted`, and `Abort(reason)`. When called from a hub method, `Abort` takes effect after that method's
  reply is sent.
- **`Clients`**: `All`, `Others`, `Caller`, `Client(id)`, `Clients(ids)`, `AllExcept(ids)`, `Group(name)`,
  `Groups(names)`, `GroupExcept(name, ids)`, `OthersInGroup(name)`. Each returns a target on which the generated
  event methods can be called (`.StateChanged(state)`).
- **`Groups`**: `AddToGroupAsync`, `RemoveFromGroupAsync` and `GetMembers`. Membership is removed automatically on
  disconnect, after `OnDisconnectedAsync` runs.
- **Disconnect reasons**: `OnDisconnectedAsync(HubDisconnect)` receives `Reason` (a `HubDisconnectReason`) and an
  optional `Message`. `Description` is the message, or a default text for the reason. Its default implementation calls
  `OnDisconnectedAsync(string? reason)` with `Description`, so override only one of them.
  - `ClientDisconnect`: the client called `Disconnect()` or was disposed.
  - `ClientTimeout`: the link dropped (an unsubscribe without a goodbye, or the cleanup sweep).
  - `ServerDisconnect`: `Context.Abort(reason)` or `IHubContext.Disconnect(id, reason)`.
  - `ServerShutdown`: `IBleHubHost.Stop(reason)` or `IHubContext.Stop(reason)`.
- **Renames**: override `OnRenamedAsync(string? previousName)` to react when a connected client calls `Rename`. It
  runs after `ValidateClient` accepted the new name; `Context.Client.Name` is already the new name. Throw to refuse
  (the name is put back and the client gets the message). The library doesn't tell other clients, so push it yourself:
  ```csharp
  public override Task OnRenamedAsync(string? previousName)
      => this.Clients.Others.PlayerRenamed(previousName, this.Context.Client.Name);   // contract: event Action<string?, string?> PlayerRenamed
  ```
- **Exceptions** thrown in a hub method reach the caller as `BleHubRemoteException` (with `RemoteErrorType` and
  `Message`).

### 3. Host registration and lifetime

```csharp
// one call, once - every hub, host settings and protocol limits (adds the platform BLE hosting stack too)
builder.Services.AddBleHubServer(server => server
    .ServiceUuid(ServiceUuid)                                             // optional - clients must use the same one
    .Host(o =>
    {
        o.LocalName = "TTT";                                              // keep short - see best practices
        o.EnableFileTransfers(Path.Combine(FileSystem.AppDataDirectory, "files"), ft =>
        {
            ft.MaxUploadSize = 1024 * 1024;
            ft.Authorize = req => req.Request.FileName.EndsWith(".jpg");
        });
    })
    .Protocol(o => o.RequestTimeout = TimeSpan.FromSeconds(10))          // optional, app-wide
    .AddHub<GameHub>(GameHubCharacteristicUuid, o =>
    {
        o.MaxClients = 6;
        o.ValidateClient = info => String.IsNullOrWhiteSpace(info.Name) ? "A name is required" : null;  // return a reason to refuse
    })
    .AddHub<ChatHub>(ChatHubCharacteristicUuid)
);

// start everything
await services.GetRequiredService<IBleHubHost>().Start();

// or control one hub
public class Lobby(IHubContext<GameHub> hub)
{
    Task Open() => hub.Start();
    Task Close() => hub.Stop("Lobby closed");     // clients get the reason in Disconnected
    bool IsOpen => hub.IsRunning;
}
```

- **UUIDs**: always use full 128-bit UUIDs. Every hub needs its **own characteristic UUID**. There is **one service
  UUID** for every hub: `server.ServiceUuid(...)` on the host and `o.ServiceUuid` in each `AddBleHubClient` (they must
  match). `AddHub` / `AddBleHubClient` take only the characteristic. Leaving the default
  (`BleHubProtocolOptions.DefaultServiceUuid`) means other apps using this library show up in scans, so set your own in
  a real app.
- **Call `AddBleHubServer` once** with every hub. A second call, no hubs, a hub or characteristic added twice, or a
  malformed UUID throws at registration. There is no `AddBleHub` / `ConfigureBleHubHost` / `ConfigureBleHubProtocol`.
- **A scan can't tell which hubs a host runs** (it only sees the one service UUID). Joining a stopped hub is refused
  by the handshake (`BleHubException` "Host refused..."), so handle that when a host runs some hubs only on demand.
- **Stopping a hub** disconnects its clients and refuses new handshakes. The service and the advertisement stay up
  while any hub runs.
- **Renaming the host**: `await host.Rename("TTT 2")` (`IBleHubHost`) changes the name without stopping: it
  re-advertises under the new name and tells every connected client (`HostRenamed`). While stopped it just sets
  `LocalName` for the next `Start`. Don't stop and restart to rename.

### 4. Pushing from outside a hub

```csharp
public class Ticker(IHubContext<GameHub> hub)
{
    public Task Tick(GameState state) => hub.Clients.All.StateChanged(state);   // generated C# 14 extension property
    public Task Kick(string id) => hub.Disconnect(id, "Removed by host");
    public IReadOnlyList<BleHubConnectedClient> Players => hub.ConnectedClients;
}
```

`IHubContext<THub>` also exposes `ClientConnected` / `ClientDisconnected` / `ClientRenamed` events and `Groups`.
`ClientRenamed` gives `BleHubClientRenamedEventArgs(Client, PreviousName)`; `Client.Name` is the new name. `ClientDisconnected` gives
`BleHubClientDisconnectedEventArgs(Client, Disconnect)`; `e.Disconnect.Reason` says why, and `e.Reason` is the text.

### 5. Client

```csharp
builder.Services.AddBleHubClient<IGameHub>(GameHubCharacteristicUuid, o => o.ServiceUuid = ServiceUuid);  // same as the host's; adds the platform BLE stack
// inject IBleHubClient<IGameHub> (connection + .Hub), the generated GameHubClient, or IGameHub

public class JoinViewModel(IBleHubClient<IGameHub> client)
{
    IDisposable? scan;

    public void StartScan() => this.scan = client.Discover().Subscribe(host => /* host.Name, host.Rssi */);

    public async Task Join(BleHubHostInfo host)
    {
        this.scan?.Dispose();
        client.Hub.StateChanged += state => MainThread.BeginInvokeOnMainThread(() => this.Apply(state));
        client.Disconnected += (_, disconnect) =>
        {
            var text = disconnect.Reason switch
            {
                HubDisconnectReason.ServerShutdown => "The host closed the game",
                HubDisconnectReason.ServerDisconnect => disconnect.Message ?? "You were removed",
                HubDisconnectReason.ClientTimeout => "Lost the connection",
                _ => null   // ClientDisconnect: we left
            };
        };

        await client.Connect(host, new BleHubConnectOptions("Allan", AppInfo.Current.VersionString));
        var result = await client.Hub.Join("Allan");
    }
}
```

- **Proxy name**: the interface name without its leading `I`, plus `Client`, so `IGameHub` becomes `GameHubClient`.
  Override it with `[BleHubClient(ProxyName = "...")]`.
- **Hub events are raised on a background thread**, one at a time and in the order the host sent them. Marshal to
  the UI thread yourself.
- **`Status`**: `Disconnected`, `Connecting`, `Connected` or `Disconnecting`. Events: `StatusChanged`, `Connected`,
  and `Disconnected`, an `EventHandler<HubDisconnect>`. `BleHubStatusChangedEventArgs(Status, Disconnect)` carries
  the `HubDisconnect` for `Disconnecting` and `Disconnected`, and has a computed `Reason` string.
- **Client-side reasons**: `ClientDisconnect` (`Disconnect()` / `Dispose()`), `ClientTimeout` (the link dropped),
  `ServerDisconnect` / `ServerShutdown` (from the host), and `ConnectionFailed` (a connect or handshake failed;
  `Disconnected` isn't raised because it never connected). A disconnect from a host older than reason codes arrives
  as `ServerDisconnect`.
- **Failures**:
  - Calls made while not connected throw `BleHubDisconnectedException`.
  - Calls in flight when the link drops fail with the same exception. Its `Disconnect` says why (`Reason` is the
    text).
  - A call times out with `TimeoutException` after `BleHubProtocolOptions.RequestTimeout` (default 30s). Streams aren't
    timed out.
  - Cancelling a call's `CancellationToken` cancels the hub method on the host.
- **Renaming**: `await client.Rename("Allan B")` changes the name the host knows this client by, without reconnecting.
  A refusal (`ValidateClient` or `OnRenamedAsync`, or a host older than renames) throws `BleHubRemoteException`; check
  `RemoteErrorType == BleHubRemoteException.RenameRefused`. Needs a connection. `ClientName` is the current name.
  `HostRenamed` (`EventHandler<string?>`) is raised, in order with hub events, after `HostName` has been updated.
- **Shared connections**: several hub clients connected to the same host share one BLE connection.

### 6. File transfers (L2CAP)

```csharp
if (client.CanTransferFiles)
{
    await client.UploadFile(localPath, "avatar-123.jpg", new Progress<TransferProgress>(p => this.Percent = p.PercentComplete));
    await client.DownloadFile("avatar-host.jpg", Path.Combine(FileSystem.CacheDirectory, "host.jpg"));
}
```

- **Discovery**: the PSM comes from the handshake, so there's nothing to configure on the client.
- **Not served**: `CanTransferFiles` is false when the host doesn't serve files or L2CAP isn't available. File calls
  then throw `BleHubFileTransferNotSupportedException`.
- **Host events**: `IBleHubHost.FileTransferred` and `FileTransferProgress`.
- **Custom handling**: register `IBleHubFileHandler` to serve or receive files yourself, for example from a
  database.

### 7. Platform setup

Android `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" android:usesPermissionFlags="neverForLocation" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />
<uses-permission android:name="android.permission.BLUETOOTH" android:maxSdkVersion="30" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" android:maxSdkVersion="30" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" android:maxSdkVersion="30" />
```

iOS `Info.plist`: `NSBluetoothAlwaysUsageDescription`.

Testing needs **two physical devices**: simulators and emulators have no usable Bluetooth.

## Wi-Fi as well as BLE

When the app should also work over Wi-Fi (players at home rather than on a plane), don't hand-roll a second transport:
use **Shiny.UniversalHubs**. The hub and contract stay as they are; the host registers
`AddUniversalServer(server => server.AddHub<THub>(characteristicUuid, "_myapp._tcp"))` in place of `AddBleHubServer` and
starts with `IUniversalHubHost.Start(HubTransports.All)`, the client registers `AddUniversalHubClient<TContract>(...)` and
uses `IUniversalHubClient<TContract>.DiscoverAll()` / `Connect(HubHostInfo)`.

## Best Practices

1. **Put the contract where both sides can see it.** Use one shared project, or the same app if a device can be
   either role.
2. **Use stable names.** Methods and events travel by name: renaming one breaks older clients. Add new members instead
   of changing existing ones.
3. **Register every wire type** in a `JsonSerializerContext` and call `Json.AddContext` at startup. A missing type
   fails at runtime, not at compile time.
4. **Keep messages small.** Hub messages are chunked over GATT (a few KB/s, 256 KB max by default). Move anything
   large, such as images or logs, with `UploadFile` / `DownloadFile`.
5. **Keep `LocalName` short** (about 8 characters), and set your app's own `ServiceUuid` on both sides
   (`server.ServiceUuid(...)` and `AddBleHubClient(..., o => o.ServiceUuid = ...)`).
6. **Marshal hub events to the UI thread.**
7. **Disconnecting is cooperative.** iOS peripherals can't drop a central, so `Abort` / `Disconnect` /
   `Stop` ask the client library to leave. Clients built with Shiny.BluetoothLE.Hubs always comply.
   `Disconnect()` tells the host it is leaving before it unsubscribes, so the host reports `ClientDisconnect` rather
   than `ClientTimeout`. Switch on `HubDisconnectReason`, never on the message text.
8. **Never store state on the hub instance.** It is recreated for every call.

## Reference Files

- `reference/api-reference.md` - full public API surface
