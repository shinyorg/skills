---
name: shiny-obd
description: Generate code using Shiny.Obd, an OBD-II vehicle communication library for .NET with command-object pattern, adapter auto-detection, and BLE transport
auto_invoke: true
triggers:
  - obd
  - obd-ii
  - obd2
  - vehicle diagnostics
  - elm327
  - obdlink
  - IObdCommand
  - IObdConnection
  - IObdTransport
  - IObdDeviceScanner
  - ObdDiscoveredDevice
  - BleObdDeviceScanner
  - ObdCommand
  - ObdConnection
  - ObdException
  - ObdTimeoutException
  - StandardCommands
  - SupportedPidsCommand
  - MonitorStatusCommand
  - MonitorStatus
  - DtcReadCommand
  - ClearDtcCommand
  - DtcDecoder
  - FuelTrimCommand
  - FuelTypeCommand
  - FuelTypes
  - OdometerCommand
  - MassAirFlowCommand
  - EngineFuelRateCommand
  - HybridBatteryLifeCommand
  - vehicle speed
  - engine rpm
  - coolant temperature
  - throttle position
  - vin command
  - trouble codes
  - diagnostic trouble codes
  - dtc
  - check engine light
  - clear codes
  - supported pids
  - fuel trim
  - odometer
  - fuel type
  - hybrid battery
  - MonitorStatusDecoder
  - MonitorReadiness
  - EmissionMonitor
  - IgnitionType
  - FuelSystemStatusCommand
  - FuelSystemState
  - FreezeFrameCommands
  - AsFreezeFrame
  - CalibrationIdCommand
  - AcceleratorPedalPositionCommand
  - TimingAdvanceCommand
  - BarometricPressureCommand
  - IntakeManifoldPressureCommand
  - AmbientAirTemperatureCommand
  - freeze frame
  - readiness monitors
  - emissions inspection
  - emissions readiness
  - fuel system status
  - closed loop
  - open loop
  - accelerator pedal position
  - timing advance
  - manifold pressure
  - barometric pressure
  - ambient air temperature
  - calibration id
  - ecu tune
  - IVinDecoder
  - VinVehicle
  - VinNumber
  - VpicVinDecoder
  - AddVinDecoder
  - vin decode
  - vin decoder
  - decode vin
  - vpic
  - nhtsa
  - make model year
  - vehicle lookup
  - obd ble
  - BleObdTransport
  - obd adapter
  - obd scan
  - device scanner
  - obd adapter not found
  - ble scan not finding device
  - adapter profile
  - Shiny.Obd
  - AddShinyObdBluetoothLE
---

# Shiny.Obd Skill

You are an expert in Shiny.Obd, a .NET library for communicating with vehicles through OBD-II adapters. It uses a command-object pattern with generic return types, pluggable transports (BLE first), and adapter auto-detection for ELM327 and OBDLink (STN) adapters.

## When to Use This Skill

Invoke this skill when the user wants to:
- Read vehicle data (speed, RPM, coolant temp, VIN, etc.) through OBD-II
- Create custom OBD commands with typed return values
- Connect to an OBD-II adapter over Bluetooth LE
- Scan for / discover available OBD adapters
- Configure ELM327 or OBDLink adapter initialization
- Implement a custom transport (WiFi, USB) for OBD communication
- Send raw AT commands to an OBD adapter
- Handle OBD response parsing and error handling
- Build a MAUI app with OBD integration

## Library Overview

- **Repository**: https://github.com/shinyorg/obd
- **Namespaces**: `Shiny.Obd`, `Shiny.Obd.Ble`, `Shiny.Obd.Commands`
- **NuGet**: `Shiny.Obd` (core), `Shiny.Obd.Ble` (BLE transport)
- **Targets**: `net10.0` (core), `net10.0` (BLE)

## Core Types

### IObdCommand<T> — Command interface

Every OBD command implements this. `T` is the parsed result type.

```csharp
public interface IObdCommand<T>
{
    string RawCommand { get; }
    T Parse(byte[] data);
}
```

### ObdCommand<T> — Base class for standard Mode/PID commands

Validates mode+PID response header, strips it, and delegates to `ParseData`.

```csharp
public abstract class ObdCommand<T> : IObdCommand<T>
{
    protected ObdCommand(byte mode, byte pid);
    public byte Mode { get; }
    public byte Pid { get; }
    public virtual string RawCommand { get; } // "{Mode:X2}{Pid:X2}"
    protected abstract T ParseData(byte[] data); // data after header
}
```

### IObdConnection — Connection interface

```csharp
public interface IObdConnection : IAsyncDisposable
{
    bool IsConnected { get; }
    Task Connect(CancellationToken ct = default);
    Task Disconnect();
    Task<T> Execute<T>(IObdCommand<T> command, CancellationToken ct = default);
    Task<string> SendRaw(string command, CancellationToken ct = default);
}
```

### IObdTransport — Transport abstraction

```csharp
public interface IObdTransport : IAsyncDisposable
{
    bool IsConnected { get; }
    Task Connect(CancellationToken ct = default);
    Task Disconnect();
    Task<string> Send(string command, CancellationToken ct = default);
}
```

### IObdDeviceScanner — Device discovery

```csharp
public interface IObdDeviceScanner
{
    Task Scan(Action<ObdDiscoveredDevice> onDeviceFound, CancellationToken ct = default);
}
```

Cancel the token to stop scanning. Each discovered device invokes the callback.

### ObdDiscoveredDevice — Discovered adapter

```csharp
public class ObdDiscoveredDevice
{
    public string Name { get; }        // e.g. "OBDLink MX+"
    public string Id { get; }          // unique identifier (BLE UUID, IP, etc.)
    public object NativeDevice { get; } // IPeripheral for BLE, IPEndPoint for WiFi, etc.
}
```

### ObdConnection — ELM327 protocol handler

Two constructors:
- `ObdConnection(IObdTransport transport)` — auto-detects adapter via ATI
- `ObdConnection(IObdTransport transport, IObdAdapterProfile profile)` — uses explicit profile, skips detection

Properties:
- `DetectedAdapter` — `ObdAdapterInfo?` with `RawIdentifier` (string) and `Type` (ObdAdapterType enum: Unknown, Elm327, ObdLink). Null when explicit profile used.

Handles:
- ELM327 hex response parsing (single-line and multi-frame CAN)
- Multi-frame framing: discards the leading byte-count line and the `N:` frame index, then concatenates frames in order
- Spaced and unspaced hex alike (`0: 49 02 01` and `0:490201`)
- Error detection: "NO DATA", "UNABLE TO CONNECT", "BUS INIT: ...ERROR", "?"
- Strips "SEARCHING..." and "BUS INIT" prefixes

### IObdAdapterProfile — Adapter initialization

```csharp
public interface IObdAdapterProfile
{
    string Name { get; }
    Task Initialize(IObdConnection connection, CancellationToken ct = default);
}
```

Built-in profiles:
- `Elm327AdapterProfile` — ATZ, ATE0, ATL0, ATS1, ATH0, ATSP0
- `ObdLinkAdapterProfile` — extends Elm327 with STFAC, ATCAF1

## Standard Commands (StandardCommands static class)

| Property | Type | Command | Return | Parse Formula |
|----------|------|---------|--------|---------------|
| `VehicleSpeed` | `VehicleSpeedCommand` | `010D` | `int` (km/h) | `A` |
| `EngineRpm` | `EngineRpmCommand` | `010C` | `int` (RPM) | `((A*256)+B)/4` |
| `CoolantTemperature` | `CoolantTemperatureCommand` | `0105` | `int` (°C) | `A-40` |
| `ThrottlePosition` | `ThrottlePositionCommand` | `0111` | `double` (%) | `(A*100)/255` |
| `FuelLevel` | `FuelLevelCommand` | `012F` | `double` (%) | `(A*100)/255` |
| `CalculatedEngineLoad` | `CalculatedEngineLoadCommand` | `0104` | `double` (%) | `(A*100)/255` |
| `IntakeAirTemperature` | `IntakeAirTemperatureCommand` | `010F` | `int` (°C) | `A-40` |
| `RuntimeSinceStart` | `RuntimeSinceStartCommand` | `011F` | `TimeSpan` | `(A*256)+B` seconds |
| `Vin` | `VinCommand` | `0902` | `string` | skip count byte, ASCII decode |
| `Odometer` | `OdometerCommand` | `01A6` | `double` (km) | `((A<<24)\|(B<<16)\|(C<<8)\|D)/10` |
| `DistanceSinceCodesCleared` | `DistanceSinceCodesClearedCommand` | `0131` | `int` (km) | `(A*256)+B` |
| `ControlModuleVoltage` | `ControlModuleVoltageCommand` | `0142` | `double` (V) | `((A*256)+B)/1000` |
| `MassAirFlow` | `MassAirFlowCommand` | `0110` | `double` (g/s) | `((A*256)+B)/100` |
| `EngineFuelRate` | `EngineFuelRateCommand` | `015E` | `double` (L/h) | `((A*256)+B)/20` |
| `EngineOilTemperature` | `EngineOilTemperatureCommand` | `015C` | `int` (°C) | `A-40` |
| `FuelType` | `FuelTypeCommand` | `0151` | `byte` | `A` (J1979 code) |
| `HybridBatteryLife` | `HybridBatteryLifeCommand` | `015B` | `double` (%) | `(A*100)/255` |
| `MonitorStatus` | `MonitorStatusCommand` | `0101` | `MonitorStatus` | see readiness section below |
| `MonitorStatusThisDriveCycle` | `MonitorStatusThisDriveCycleCommand` | `0141` | `MonitorStatus` | same layout; byte A reserved |
| `FuelSystemStatus` | `FuelSystemStatusCommand` | `0103` | `FuelSystemStatus` | enumerated loop state, `A` and optional `B` |
| `IntakeManifoldPressure` | `IntakeManifoldPressureCommand` | `010B` | `int` (kPa) | `A` |
| `BarometricPressure` | `BarometricPressureCommand` | `0133` | `int` (kPa) | `A` |
| `TimingAdvance` | `TimingAdvanceCommand` | `010E` | `double` (° BTDC) | `(A/2)-64` |
| `AmbientAirTemperature` | `AmbientAirTemperatureCommand` | `0146` | `int` (°C) | `A-40` |
| `RelativeAcceleratorPedalPosition` | `RelativeAcceleratorPedalPositionCommand` | `015A` | `double` (%) | `(A*100)/255` |
| `CommandedThrottleActuator` | `CommandedThrottleActuatorCommand` | `014C` | `double` (%) | `(A*100)/255` |
| `DistanceWithMilOn` | `DistanceWithMilOnCommand` | `0121` | `int` (km) | `(A*256)+B` |
| `TimeRunWithMilOn` | `TimeRunWithMilOnCommand` | `014D` | `TimeSpan` | `(A*256)+B` **minutes** |
| `TimeSinceCodesCleared` | `TimeSinceCodesClearedCommand` | `014E` | `TimeSpan` | `(A*256)+B` **minutes** |
| `CalibrationId` | `CalibrationIdCommand` | `0904` | `IReadOnlyList<string>` | 16-byte ASCII blocks, null-padded |

⚠️ PIDs `4D`/`4E` are **minutes**; `RuntimeSinceStart` (`011F`) is **seconds**. Do not copy one
formula onto the other.

⚠️ Do not confuse the four position PIDs. `ThrottlePosition` (`0111`) is *absolute throttle plate*
position, the drive-by-wire **output**, and it carries a closed-pedal floor of 12-18% that varies by
vehicle. `AcceleratorPedalPositionCommand` (`0149`/`014A`/`014B`) and
`RelativeAcceleratorPedalPosition` (`015A`) are the driver's **input**, and `015A` is already
normalised so a released pedal reads 0. When measuring how hard a vehicle is being driven, prefer
`015A`, then a pedal sensor, and only fall back to `0111` — never assume `0111` bottoms out at zero.

⚠️ `AmbientAirTemperature` (`0146`) is the air outside; `IntakeAirTemperature` (`010F`) is what the
engine is breathing, measured after the engine bay has warmed it. They are not interchangeable.

## Commands not on StandardCommands

These need construction data or carry their own shared instances, so never write
`StandardCommands.FuelTrim` or `StandardCommands.SupportedPids` — they do not exist.

| Command | Construct with | Return | Notes |
|---------|----------------|--------|-------|
| `FuelTrimCommand` | `FuelTrimCommand.ShortTermBank1()` / `.LongTermBank1()` / `.ShortTermBank2()` / `.LongTermBank2()` (PIDs 06/07/08/09), or `new FuelTrimCommand(pid)` | `double` (%) | `(A*100/128)-100`. 128 is zero correction; positive = ECU adding fuel |
| `SupportedPidsCommand` | `new SupportedPidsCommand(block)` for each of `SupportedPidsCommand.BlockPids` | `IReadOnlyList<byte>` | The 32 PIDs following the block PID that the vehicle answers |
| `AcceleratorPedalPositionCommand` | `AcceleratorPedalPositionCommand.D()` / `.E()` / `.F()` (PIDs 49/4A/4B), or `new AcceleratorPedalPositionCommand(pid)` | `double` (%) | `(A*100)/255`. Redundant sensors on one pedal; most vehicles report D and E |
| `DtcReadCommand` | `DtcReadCommand.Stored` (03) / `.Pending` (07) / `.Permanent` (0A) | `IReadOnlyList<string>` | SAE J2012 strings, e.g. `"P0301"` |
| `ClearDtcCommand` | `ClearDtcCommand.Instance` (04) | `bool` | True when the ECU answers `0x44` |
| mode 02 readings | `someModeOneCommand.AsFreezeFrame(frame)` | same as the mode 01 command | Never write a separate mode 02 command class |
| `FreezeFrameCommands.CausalDtc(frame)` | static factory | `string?` | The code that stored the frame; null = no snapshot |

### Reading a J1979 fuel type

`FuelTypeCommand` returns a raw code; `FuelTypes.Describe` names it. It answers **null** for `0x00`
("not available") and for anything outside the table — never the string "Unknown", because a caller
storing or displaying this has to be able to tell an absent answer from a claim about the vehicle.
Read it once per connection, not on a poll: a vehicle does not change what it burns.

```csharp
var fuelType = FuelTypes.Describe(await connection.Execute(StandardCommands.FuelType));
```

### Probing supported PIDs first

Querying an unsupported PID returns NO DATA, so read the bitmask blocks once per connection and
only poll what the vehicle answers. An unsupported reading must surface as **missing, not zero** —
the odometer PID is absent on most vehicles and hybrid battery life on every vehicle without a pack,
and a zero there is indistinguishable from a dead pack.

```csharp
var supported = new HashSet<byte>();
foreach (var block in SupportedPidsCommand.BlockPids)   // 00, 20, 40, 60, 80, A0, C0
{
    foreach (var pid in await connection.Execute(new SupportedPidsCommand(block)))
        supported.Add(pid);
}
```

### Reading and clearing trouble codes

```csharp
var stored = await connection.Execute(DtcReadCommand.Stored);        // mode 03 — turn the MIL on
var pending = await connection.Execute(DtcReadCommand.Pending);      // mode 07
var permanent = await connection.Execute(DtcReadCommand.Permanent);  // mode 0A — ECU clears these
var cleared = await connection.Execute(ClearDtcCommand.Instance);    // mode 04
```

Modes 03/07/0A/04 carry **no PID**, so these implement `IObdCommand<T>` directly rather than
extending `ObdCommand<T>` — follow that pattern for any other PID-less mode. `DtcDecoder` is the
public parser behind them: it strips the mode echo, then uses payload **parity** to tell a CAN
reply (`43 <count> <pairs>`) from a pre-CAN one (`43 <pairs>`) rather than assuming a transport.

⚠️ Mode 04 also resets the emissions readiness monitors, which then take several drive cycles to
re-run and will fail an emissions test in the meantime. Always put it behind an explicit user
confirmation that says so — never call it as part of a connect or refresh routine.

### Emissions monitor readiness

`MonitorStatus` decodes all four bytes of PID 0x01, not just the lamp. `MonitorStatusDecoder` is the
public parser and is testable without a transport.

```csharp
var status = await connection.Execute(StandardCommands.MonitorStatus);
if (status.IsReadyForInspection == false)
    Report(status.Incomplete.Select(x => x.Monitor));
```

Rules that matter when generating code against this:

- `Monitors` contains **only the monitors the vehicle supports**. Do not write code that treats a
  missing monitor as incomplete — a monitor that does not exist on a car has no readiness state.
- The completion bits are **inverted** in the raw bytes (a set bit means the test is still running).
  `MonitorReadiness.Complete` has already flipped it; never flip it again.
- Which monitors bytes C/D describe depends on `IgnitionType`, taken from bit 3 of byte B. The same
  bit means different monitors on a petrol and a diesel — the decoder handles it, so never index the
  bytes yourself.
- `EmissionMonitor.GasolineParticulateFilter` is that bit's meaning under ISO 15031-5:2015 and
  later; earlier revisions defined it as A/C refrigerant monitoring.
- A vehicle reads not-ready for several drive cycles after codes are cleared with nothing wrong with
  it. Pair it with `TimeSinceCodesCleared` (PID 0x4E) before telling a user their car has a problem.
- `MonitorStatusThisDriveCycle` (PID 0x41) shares the type, but byte A is reserved — its `MilOn` and
  `DtcCount` are always false/0 and must not be surfaced.

### Freeze frames (mode 02)

Mode 02 accepts the same PIDs as mode 01 and scales them identically. **Never write a separate mode
02 command class** — call `AsFreezeFrame()` on the mode 01 command.

```csharp
var causal = await connection.Execute(FreezeFrameCommands.CausalDtc());
if (causal != null)
{
    var rpm = await connection.Execute(StandardCommands.EngineRpm.AsFreezeFrame());
    var load = await connection.Execute(StandardCommands.CalculatedEngineLoad.AsFreezeFrame());
}
```

⚠️ **Always gate mode 02 reads on `CausalDtc` returning non-null.** When there is no stored snapshot
the frame is zero-filled, so an engine load of 0% and a coolant temperature of -40 °C come back
looking like measurements rather than like an absence.

The mode 02 header is **three bytes** (`42 <PID> <frame>`), not two, because the frame number is
echoed. `AsFreezeFrame` throws `ObdException` on a non-mode-01 command — mode 09 identifiers are not
sampled at a moment, so there is no frame to ask for.

## VIN Decoding (Shiny.Obd.Vin)

Registration, and the only two forms that exist:

```csharp
services.AddVinDecoder();                        // built-in NHTSA vPIC
services.AddVinDecoder<MyRegistryVinDecoder>();  // your own IVinDecoder
```

`AddVinDecoder` also calls `AddHttpClient()` and registers with `TryAddSingleton`, so the first
registration wins. Do not hand-register `VpicVinDecoder`; do not resolve it by concrete type.

```csharp
var vin = await connection.Execute(StandardCommands.Vin);   // mode 09 PID 02
var vehicle = await vinDecoder.Decode(vin);                 // null when it cannot be identified
```

`VinVehicle` (provider-neutral, every field nullable):

| Property | Type | Bounds |
|----------|------|--------|
| `Make` / `Model` / `Trim` | `string?` | |
| `ModelYear` | `int?` | 1900-2100 |
| `FuelType` / `Electrification` | `string?` | |
| `EngineCylinders` | `int?` | 1-16 |
| `EngineDisplacementLitres` | `double?` | 0-20 |
| `EngineHorsepower` | `int?` | 1-2000 |
| `DriveType` / `BodyClass` / `TransmissionStyle` | `string?` | |
| `IsUsable` | `bool` | make or model identified |

Rules that matter when generating code against this:

- **Never re-parse or re-clean the result.** The values arrive typed, bounded and with the
  registries' `"Not Applicable"` / `"Not Available"` / `"N/A"` placeholders already stripped to null.
  Writing a second cleaning pass in the consumer is the mistake this shape exists to prevent.
- **Null means the registry had nothing.** Never substitute `"Unknown"` or `0` — these values reach
  users and AI prompts, where a placeholder reads as a fact about the car.
- **`IVinDecoder` never throws**, by contract. Do not wrap calls in try/catch for control flow, and
  any implementation you write must return null rather than throwing or guessing.
- **Coverage falls off outside North America.** A decode with a make and model and nothing else is a
  success, not a failure — do not treat a missing displacement as an error.
- `VinNumber.IsPlausible` / `Normalize` are the pure pre-check (17 chars, no I/O/Q). `Decode` applies
  them itself, so only call them directly when you need to know *why* nothing came back. The check
  digit is deliberately not validated — it is only mandatory in North America.
- **Mode 01 PID 0x51 outranks the registry for fuel type.** `FuelTypes.Describe` off the bus needs no
  network and is the more trustworthy source on a rebadged or grey-import vehicle:
  `var fuel = fromBus ?? vehicle?.FuelType;`

## BLE Transport (Shiny.Obd.Ble)

### BleObdConfiguration

```csharp
public class BleObdConfiguration
{
    public string ServiceUuid { get; set; } = "FFF0";
    public string ReadCharacteristicUuid { get; set; } = "FFF1";
    public string WriteCharacteristicUuid { get; set; } = "FFF2";
    public string? DeviceNameFilter { get; set; }
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
```

### BleObdTransport

Three constructors:
- `BleObdTransport(IBleManager bleManager, BleObdConfiguration config)` — scans for adapter
- `BleObdTransport(IPeripheral peripheral, BleObdConfiguration config)` — uses pre-discovered peripheral
- `BleObdTransport(ObdDiscoveredDevice device, BleObdConfiguration config)` — uses device from scanner

Uses Shiny.BluetoothLE v4 APIs:
- `ConnectAsync` for task-based connection
- `NotifyCharacteristic` for RX notifications
- `WriteCharacteristicAsync` for TX writes
- Collects notification bytes until `>` prompt, returns complete response

An exchange that hits `CommandTimeout` throws `ObdTimeoutException` and is then closed off — a late
reply arriving afterwards is discarded rather than completing the next command's wait. `Disconnect()`
fails an in-flight command straight away instead of making the caller wait out the timeout.

### BLE scanning rules

Two rules the library already follows internally. Generated code that scans with `IBleManager`
directly must follow them too, or it will not find adapters on iOS.

**1. Never match on `IPeripheral.Name` alone.** On iOS `CBPeripheral.Name` is null while scanning a
peripheral that has never been connected to — the name is only in the advertisement. Always fall
back:

```csharp
var name = scanResult.Peripheral.Name ?? scanResult.AdvertisementData?.LocalName;
```

**2. Never pass the adapter's `ServiceUuid` as a scan filter.** iOS matches a scan filter against the
*advertisement*, and most ELM327 clones don't advertise their GATT service — it appears only after
connecting. `bleManager.Scan(new ScanConfig("FFF0"))` finds nothing at all on iPhone. Scan
unfiltered and use `ServiceUuid` after connecting.

`BleObdDeviceScanner` logs every advertisement at `Debug` level before filtering (name,
`Peripheral.Name`, id, RSSI, advertised service UUIDs). When a user reports that an adapter isn't
found, tell them to enable debug logging (`builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug)`)
and read that dump rather than guessing at UUIDs.

### ObdTimeoutException

```csharp
public class ObdTimeoutException : ObdException
{
    public string Command { get; }    // the command that went unanswered
    public TimeSpan Timeout { get; }  // the deadline that elapsed
}
```

Thrown when the adapter does not answer within the transport's `CommandTimeout`. **Not** an
`OperationCanceledException` — generated polling code must be able to tell a quiet adapter apart from
its own cancellation token firing:

```csharp
try
{
    reading = await connection.Execute(StandardCommands.VehicleSpeed, ct);
}
catch (OperationCanceledException) when (ct.IsCancellationRequested)
{
    throw;                      // our own shutdown
}
catch (ObdException)
{
    reading = null;             // this PID is a write-off, the loop carries on
}
```

## Code Generation Patterns

### Creating a custom OBD command (standard PID)

```csharp
public class BarometricPressureCommand : ObdCommand<int>
{
    public BarometricPressureCommand() : base(0x01, 0x33) { }
    protected override int ParseData(byte[] data) => data[0];
}
```

### Creating a custom OBD command (non-standard)

```csharp
public class ManufacturerCommand : IObdCommand<string>
{
    public string RawCommand => "2101";
    public string Parse(byte[] data) => BitConverter.ToString(data);
}
```

### Full connection setup with BLE

```csharp
var transport = new BleObdTransport(bleManager, new BleObdConfiguration
{
    DeviceNameFilter = "OBDLink"
});
var connection = new ObdConnection(transport);
await connection.Connect();

var speed = await connection.Execute(StandardCommands.VehicleSpeed);
var rpm = await connection.Execute(StandardCommands.EngineRpm);
var vin = await connection.Execute(StandardCommands.Vin);
```

### Scanning for devices then connecting

```csharp
var scanner = new BleObdDeviceScanner(bleManager);
var cts = new CancellationTokenSource();
ObdDiscoveredDevice? selected = null;

await scanner.Scan(device =>
{
    selected = device;
    cts.Cancel(); // stop after first device
}, cts.Token);

var transport = new BleObdTransport(selected!, new BleObdConfiguration());
var connection = new ObdConnection(transport);
await connection.Connect();
```

### MAUI DI registration

```csharp
// In MauiProgram.cs
builder.Services.AddBluetoothLE();  // Shiny BLE v4 — namespace: Shiny
builder.Services.AddShinyObdBluetoothLE(new BleObdConfiguration
{
    DeviceNameFilter = "OBD"
});
```

`AddShinyObdBluetoothLE` registers `BleObdConfiguration` and `IObdDeviceScanner` (`BleObdDeviceScanner`). Call `AddBluetoothLE()` separately for platform BLE support.

### Explicit adapter profile

```csharp
var connection = new ObdConnection(transport, new ObdLinkAdapterProfile());
await connection.Connect();
```

### Custom adapter profile

```csharp
public class MyAdapterProfile : IObdAdapterProfile
{
    public string Name => "MyAdapter";
    public async Task Initialize(IObdConnection connection, CancellationToken ct = default)
    {
        await connection.SendRaw("ATZ", ct);
        await Task.Delay(500, ct);
        await connection.SendRaw("ATE0", ct);
        await connection.SendRaw("ATSP6", ct); // force CAN 11-bit 500kbaud
    }
}
```

### Custom transport implementation

```csharp
public class WifiObdTransport : IObdTransport
{
    public bool IsConnected { get; private set; }
    public async Task Connect(CancellationToken ct = default) { /* TCP connect */ }
    public Task Disconnect() { /* close socket */ }
    public async Task<string> Send(string command, CancellationToken ct = default)
    {
        // Write command, read until '>' prompt, return response without '>'
    }
    public ValueTask DisposeAsync() { /* cleanup */ }
}
```

## Important Notes

- All async methods do NOT use the `Async` suffix (e.g. `Connect`, `Execute`, `SendRaw`).
- `ObdCommand<T>.ParseData` receives bytes AFTER the 2-byte mode+PID header is stripped.
- `IObdCommand<T>.Parse` receives ALL response bytes including mode+PID header.
- `ObdConnection` appends `\r` to all commands sent via `SendRaw` before passing to transport.
- BLE transport uses `SemaphoreSlim` to serialize commands (one at a time). It is deliberately never disposed — a pending `Send` would otherwise get an `ObjectDisposedException` when the transport is torn down.
- A timeout is an `ObdTimeoutException`, never an `OperationCanceledException`. Never write `catch (OperationCanceledException) { throw; }` around `Execute` without a `when (ct.IsCancellationRequested)` filter — historically that turned one slow adapter reply into a dead polling loop.
- A BLE adapter reports `IsConnected` true long after it has stopped answering the vehicle. Polling code should count consecutive unanswered polls and rebuild the connection, rather than trusting the connection flag.
- ELM327 response parser handles both single-line (`"41 0D 50"`) and multi-frame CAN formats. A real multi-frame reply is preceded by a bare byte-count line — `"014\r0: 49 02 01 57 42 41\r1: 31 32..."` — where `014` is the total data byte count, not data. It is discarded, along with each frame's `N:` index. When writing a custom `IObdConnection` or a test fixture, include that count line: it parses cleanly as `0x14`, so omitting it in a fixture hides a real bug, and keeping it in the payload shifts every byte along by one and fails the mode-echo check. Only multi-frame commands are exposed to this — the VIN (mode 09 PID 02) and mode 03 with three or more codes.
- `BleObdDeviceScanner` deduplicates by peripheral UUID — each device is reported once.
- BLE scans match on `Peripheral.Name ?? AdvertisementData.LocalName`, never `Peripheral.Name` alone (null on iOS while scanning), and are never filtered by `ServiceUuid` (iOS matches that against the advertisement, which ELM327 clones don't carry). See "BLE scanning rules" above.
- For MAUI, register BLE with `builder.Services.AddBluetoothLE()` (namespace `Shiny`, no `UseShiny` needed in v4), then call `builder.Services.AddShinyObdBluetoothLE()` to register OBD BLE services.
- `AddShinyObdBluetoothLE` registers `BleObdConfiguration` and `IObdDeviceScanner` (`BleObdDeviceScanner`).
- A full MAUI sample app exists in `samples/Sample.Maui/` with scan → select → dashboard flow.
