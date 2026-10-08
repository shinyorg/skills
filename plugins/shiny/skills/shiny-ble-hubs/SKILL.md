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
  - AddBleHub
  - AddBleHubClient
  - ConfigureBleHubHost
  - ConfigureBleHubProtocol
  - BleHubConnectOptions
  - BleHubHostInfo
  - BleHubCallerContext
  - BleHubConnectedClient
  - EnableFileTransfers
  - IBleHubFileHandler
  - IBleHubSerializer
  - BleHubRemoteException
  - BleHubDisconnectedException
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
- **Libraries target `net10.0`**. The app registers the platform BLE stacks itself (`AddBluetoothLE()` /
  `AddBluetoothLeHosting()`).

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

    public override Task OnDisconnectedAsync(string? reason)
    {
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
- **Exceptions** thrown in a hub method reach the caller as `BleHubRemoteException` (with `RemoteErrorType` and
  `Message`).

### 3. Host registration and lifetime

```csharp
builder.Services.AddBluetoothLeHosting();
builder.Services.AddBleHub<GameHub>(ServiceUuid, GameHubCharacteristicUuid, o =>
{
    o.MaxClients = 6;
    o.ValidateClient = info => String.IsNullOrWhiteSpace(info.Name) ? "A name is required" : null;  // return a reason to refuse
});
builder.Services.ConfigureBleHubHost(o =>
{
    o.LocalName = "TTT";                                                  // keep short - see best practices
    o.EnableFileTransfers(Path.Combine(FileSystem.AppDataDirectory, "files"), ft =>
    {
        ft.MaxUploadSize = 1024 * 1024;
        ft.Authorize = req => req.Request.FileName.EndsWith(".jpg");
    });
});

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

- **UUIDs**: always use full 128-bit UUIDs. Every hub needs its **own characteristic UUID**. Hubs should **share one
  service UUID**, because more than one advertised 128-bit UUID overflows the 31 byte advertisement.
- **Stopping a hub** disconnects its clients and refuses new handshakes. A service shared by several hubs stays up
  while any of them runs, and the advertisement follows the running hubs.

### 4. Pushing from outside a hub

```csharp
public class Ticker(IHubContext<GameHub> hub)
{
    public Task Tick(GameState state) => hub.Clients.All.StateChanged(state);   // generated C# 14 extension property
    public Task Kick(string id) => hub.Disconnect(id, "Removed by host");
    public IReadOnlyList<BleHubConnectedClient> Players => hub.ConnectedClients;
}
```

`IHubContext<THub>` also exposes `ClientConnected` / `ClientDisconnected` events and `Groups`.

### 5. Client

```csharp
builder.Services.AddBluetoothLE();
builder.Services.AddBleHubClient<IGameHub>(ServiceUuid, GameHubCharacteristicUuid);
// inject IBleHubClient<IGameHub> (connection + .Hub), the generated GameHubClient, or IGameHub

public class JoinViewModel(IBleHubClient<IGameHub> client)
{
    IDisposable? scan;

    public void StartScan() => this.scan = client.Discover().Subscribe(host => /* host.Name, host.Rssi */);

    public async Task Join(BleHubHostInfo host)
    {
        this.scan?.Dispose();
        client.Hub.StateChanged += state => MainThread.BeginInvokeOnMainThread(() => this.Apply(state));
        client.Disconnected += (_, reason) => { /* host stopped, kicked us, or link lost */ };

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
  and `Disconnected` (with the reason).
- **Failures**:
  - Calls made while not connected throw `BleHubDisconnectedException`.
  - Calls in flight when the link drops fail with the same exception.
  - A call times out with `TimeoutException` after `BleHubProtocolOptions.RequestTimeout` (default 30s). Streams aren't
    timed out.
  - Cancelling a call's `CancellationToken` cancels the hub method on the host.
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
use **Shiny.SwitchboardR**. The hub and contract stay as they are; the host adds `AddShinyHttpServer(http => http.AddSwitchboardR().AddHub<THub>())`
and starts with `ISwitchboardRHost.Start(HubTransports.All)`, the client registers `AddSwitchboardRClient<TContract>(...)` and
uses `ISwitchboardRClient<TContract>.DiscoverAll()` / `Connect(HubHostInfo)`.

## Best Practices

1. **Put the contract where both sides can see it.** Use one shared project, or the same app if a device can be
   either role.
2. **Use stable names.** Methods and events travel by name: renaming one breaks older clients. Add new members instead
   of changing existing ones.
3. **Register every wire type** in a `JsonSerializerContext` and call `Json.AddContext` at startup. A missing type
   fails at runtime, not at compile time.
4. **Keep messages small.** Hub messages are chunked over GATT (a few KB/s, 256 KB max by default). Move anything
   large, such as images or logs, with `UploadFile` / `DownloadFile`.
5. **Keep `LocalName` short** (about 8 characters) and share one service UUID across hubs.
6. **Marshal hub events to the UI thread.**
7. **Disconnecting is cooperative.** iOS peripherals can't drop a central, so `Abort` / `Disconnect` /
   `Stop` ask the client library to leave. Clients built with Shiny.BluetoothLE.Hubs always comply.
8. **Never store state on the hub instance.** It is recreated for every call.

## Reference Files

- `reference/api-reference.md` - full public API surface
