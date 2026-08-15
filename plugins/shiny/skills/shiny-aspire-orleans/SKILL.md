---
name: shiny-aspire-orleans
description: Generate code using Shiny Aspire integrations — Orleans ADO.NET hosting, Gluetun VPN container routing, and tunnelling (public addresses for Aspire endpoints, the Shiny relay hosted in the app model, and SSH forwards to services behind a bastion)
auto_invoke: true
triggers:
  - aspire orleans
  - orleans aspire
  - WithDatabaseSetup
  - UseAdoNet
  - UseAdoNetClient
  - orleans database setup
  - orleans schema
  - orleans clustering
  - orleans grain storage
  - orleans reminders
  - OrleansFeature
  - Shiny.Aspire.Orleans
  - aspire orleans hosting
  - aspire orleans server
  - aspire orleans client
  - orleans adonet
  - orleans ado.net
  - gluetun
  - gluetun vpn
  - AddGluetun
  - WithRoutedContainer
  - WithVpnProvider
  - WithWireGuard
  - WithOpenVpn
  - vpn container
  - aspire vpn
  - Shiny.Aspire.Hosting.Gluetun
  - GluetunResource
  - network_mode
  - vpn routing
  - aspire tunnel
  - tunnel aspire
  - public url aspire
  - webhook local development
  - AddTunnel
  - WithTunnel
  - WithTunnelUrl
  - WithQuickTunnel
  - AddQuickTunnel
  - WithSshTunnel
  - AddSshTunnel
  - AddSshPortForward
  - WithShinyRelayTunnel
  - AddShinyRelay
  - WithAzureRelayTunnel
  - AddAzureRelayTunnel
  - WithCloudflareTunnel
  - AddCloudflareTunnel
  - WithNgrokTunnel
  - AddNgrokTunnel
  - TunnelResource
  - ITunnelResource
  - InProcessTunnelResource
  - ContainerTunnelResource
  - SshTunnelResource
  - SshPortForwardResource
  - ShinyRelayResource
  - CloudflaredResource
  - NgrokResource
  - ITunnelProvider
  - Shiny.Aspire.Hosting.Tunnel
  - ngrok aspire
  - cloudflared aspire
  - quick tunnel
  - pinggy
  - ssh port forward
  - bastion
  - reverse tunnel
---

# Shiny Aspire Skill

You are an expert in Shiny's .NET Aspire integrations:

1. **Shiny.Aspire.Orleans** — Zero-friction integration between .NET Aspire and Microsoft Orleans for ADO.NET storage backends. Automatically provisions Orleans database schemas and wires up clustering, grain persistence, and reminders from Aspire configuration.
2. **Shiny.Aspire.Hosting.Gluetun** — Aspire hosting integration for Gluetun VPN containers. Models Gluetun as a first-class Aspire resource and lets other containers route their traffic through the VPN tunnel.
3. **Shiny.Aspire.Hosting.Tunnel** (and `.Ssh`, `.AzureRelay`, `.Cloudflare`, `.Ngrok`) — tunnelling. Gives a project or container endpoint a public address, hosts the Shiny relay in the app model, and reaches services on the far side of an SSH bastion. Built on the same `ITunnelProvider` abstraction as Shiny.Net.HttpServer.

## When to Use This Skill

Invoke this skill when the user wants to:
- Set up Orleans with .NET Aspire using ADO.NET storage (PostgreSQL, SQL Server, or MySQL)
- Automatically create Orleans database schemas from Aspire
- Configure an Orleans silo with ADO.NET providers from Aspire-injected config
- Configure an Orleans client with ADO.NET clustering from Aspire-injected config
- Use `WithDatabaseSetup` to auto-provision Orleans tables
- Use `silo.UseAdoNet()` to configure a silo inside `UseOrleans`
- Use `client.UseAdoNetClient()` to configure a client inside `UseOrleansClient`
- Select which Orleans features to provision (clustering, persistence, reminders)
- Set up multiple named grain storage providers
- Add a Gluetun VPN container to an Aspire app
- Route container traffic through a VPN tunnel
- Configure VPN providers, WireGuard, or OpenVPN in Aspire
- Use `AddGluetun`, `WithRoutedContainer`, `WithVpnProvider`, `WithWireGuard`, or `WithOpenVpn`
- Set up Docker Compose publishing with VPN network mode and port transfer
- Give an Aspire project or container a public URL for webhooks, OAuth redirects, or a demo
- Use `WithQuickTunnel`, `WithSshTunnel`, `WithShinyRelayTunnel`, `WithAzureRelayTunnel`, `WithCloudflareTunnel` or `WithNgrokTunnel`
- Hand a resource its own public URL with `WithTunnelUrl`
- Host the Shiny relay in the app model with `AddShinyRelay` so devices or MAUI apps can register into it
- Reach a database or API behind an SSH bastion with `AddSshPortForward`
- Plug a custom `ITunnelProvider` in with `AddTunnel`

## Library Overview

- **Repository**: https://github.com/shinyorg/aspire
- **Target**: `net10.0`
- **Aspire**: 13.1+
- **Orleans**: 10.0+ (Orleans packages only)

### Packages

| Package | NuGet | Usage |
|---|---|---|
| `Shiny.Aspire.Orleans.Hosting` | Install in Aspire AppHost | Auto-runs Orleans schema scripts when the database becomes ready |
| `Shiny.Aspire.Orleans.Server` | Install in Orleans silo | Registers ADO.NET provider builders for clustering, grain storage, and reminders |
| `Shiny.Aspire.Orleans.Client` | Install in Orleans client | Registers ADO.NET provider builder for clustering |
| `Shiny.Aspire.Hosting.Gluetun` | Install in Aspire AppHost | Adds a Gluetun VPN container and routes other containers through it |
| `Shiny.Aspire.Hosting.Tunnel` | Install in Aspire AppHost | The tunnel resource, the pluggable provider, and the Shiny relay hosted in the app model |
| `Shiny.Aspire.Hosting.Tunnel.Ssh` | Install in Aspire AppHost | Quick tunnels, SSH remote forwarding, and local forwards through a bastion |
| `Shiny.Aspire.Hosting.Tunnel.AzureRelay` | Install in Aspire AppHost | Azure Relay Hybrid Connections |
| `Shiny.Aspire.Hosting.Tunnel.Cloudflare` | Install in Aspire AppHost | `cloudflared` as a container resource |
| `Shiny.Aspire.Hosting.Tunnel.Ngrok` | Install in Aspire AppHost | The ngrok agent as a container resource |

### Supported Databases

| Database | ADO.NET Invariant | ProviderType Value |
|---|---|---|
| PostgreSQL | `Npgsql` | `PostgresDatabase` |
| SQL Server | `Microsoft.Data.SqlClient` | `SqlServerDatabase` |
| MySQL | `MySql.Data.MySqlClient` | `MySqlDatabase` |

## Architecture: Provider Registration

The Server and Client packages register Orleans provider builders via `[assembly: RegisterProvider]` attributes. When Orleans calls `ApplyConfiguration` during `UseOrleans()`, it reads the Aspire-injected configuration (e.g. `Orleans:Clustering:ProviderType = "PostgresDatabase"`) and resolves the matching provider builder automatically. The provider builder maps the database type to the correct ADO.NET invariant and resolves the connection string from the `ServiceKey`.

This means:
- **No manual configuration** — providers are resolved automatically from Aspire config.
- **Extension methods** — `UseAdoNet()` on `ISiloBuilder` and `UseAdoNetClient()` on `IClientBuilder` are identity methods for discoverability. The actual wiring happens through the registered provider builders.
- **Composable** — because configuration happens inside `UseOrleans(silo => { ... })` or `UseOrleansClient(client => { ... })`, users can add other features alongside.

## Setup

### 1. Aspire AppHost

Install `Shiny.Aspire.Orleans.Hosting` in the AppHost project.

```csharp
using Shiny.Aspire.Orleans.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pg")
    .WithPgAdmin()
    .AddDatabase("orleans-db");

var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithReminders(db)
    .WithDatabaseSetup(db); // creates all Orleans tables automatically

builder.AddProject<Projects.MySilo>("silo")
    .WithReference(orleans)
    .WaitFor(db);

builder.AddProject<Projects.MyApi>("api")
    .WithReference(orleans.AsClient())
    .WaitFor(db);

builder.Build().Run();
```

### 2. Orleans Silo

Install `Shiny.Aspire.Orleans.Server` in the silo project. Call `silo.UseAdoNet()` inside `UseOrleans`.

```csharp
using Shiny.Aspire.Orleans.Server;

var builder = WebApplication.CreateBuilder(args);

builder.UseOrleans(silo =>
{
    silo.UseAdoNet();
});

var app = builder.Build();
app.Run();
```

### 3. Orleans Client

Install `Shiny.Aspire.Orleans.Client` in the client project (e.g. an API gateway). Call `client.UseAdoNetClient()` inside `UseOrleansClient`.

```csharp
using Shiny.Aspire.Orleans.Client;

var builder = WebApplication.CreateBuilder(args);

builder.UseOrleansClient(client =>
{
    client.UseAdoNetClient();
});

var app = builder.Build();

app.MapGet("/counter/{name}", async (string name, IClusterClient client) =>
{
    var grain = client.GetGrain<ICounterGrain>(name);
    var count = await grain.GetCount();
    return Results.Ok(new { name, count });
});

app.Run();
```

## API Reference

### Hosting Package — `Shiny.Aspire.Orleans.Hosting`

#### WithDatabaseSetup

```csharp
public static OrleansService WithDatabaseSetup(
    this OrleansService orleans,
    IResourceBuilder<IResourceWithConnectionString> database,
    OrleansFeature features = OrleansFeature.All
)
```

Subscribes to Aspire's `ResourceReadyEvent` for the database resource. When the database is up and accepting connections, it executes embedded Orleans SQL schema scripts. The database type (PostgreSQL, SQL Server, MySQL) is auto-detected from the Aspire resource.

**Parameters:**
- `orleans` — The Orleans service from `builder.AddOrleans()`
- `database` — An Aspire database resource (e.g. from `AddPostgres`, `AddSqlServer`, `AddMySql`)
- `features` — Which Orleans features to provision (default: `OrleansFeature.All`)

#### OrleansFeature (Flags Enum)

```csharp
[Flags]
public enum OrleansFeature
{
    Clustering = 1,   // Membership tables for silo discovery
    Persistence = 2,  // Grain storage tables
    Reminders = 4,    // Reminder tables
    All = Clustering | Persistence | Reminders
}
```

Use to limit which schemas are provisioned:

```csharp
// Only clustering and persistence (no reminders)
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering | OrleansFeature.Persistence);

// Only clustering
orleans.WithDatabaseSetup(db, OrleansFeature.Clustering);
```

#### DatabaseType (Enum)

```csharp
public enum DatabaseType
{
    SqlServer,
    PostgreSQL,
    MySql
}
```

Auto-detected from the Aspire resource — you do not need to specify this directly.

### Server Package — `Shiny.Aspire.Orleans.Server`

#### UseAdoNet (ISiloBuilder extension)

```csharp
public static ISiloBuilder UseAdoNet(this ISiloBuilder siloBuilder)
```

Marker extension for discoverability. The actual provider registration happens automatically via `[assembly: RegisterProvider]` attributes when the package is referenced. Providers are registered for all three database types across Clustering, GrainStorage, and Reminders.

Call inside `UseOrleans`:

```csharp
builder.UseOrleans(silo =>
{
    silo.UseAdoNet();
    // compose with other silo features here
});
```

### Client Package — `Shiny.Aspire.Orleans.Client`

#### UseAdoNetClient (IClientBuilder extension)

```csharp
public static IClientBuilder UseAdoNetClient(this IClientBuilder clientBuilder)
```

Marker extension for discoverability. Registers ADO.NET clustering provider builders for both Silo and Client targets. Clients do not need grain storage or reminders.

Call inside `UseOrleansClient`:

```csharp
builder.UseOrleansClient(client =>
{
    client.UseAdoNetClient();
});
```

## Configuration Flow

Aspire injects the following configuration when you use `.WithReference(orleans)`:

```
Orleans:Clustering:ProviderType = "PostgresDatabase"
Orleans:Clustering:ServiceKey   = "orleans-db"
Orleans:GrainStorage:Default:ProviderType = "PostgresDatabase"
Orleans:GrainStorage:Default:ServiceKey   = "orleans-db"
Orleans:Reminders:ProviderType  = "PostgresDatabase"
Orleans:Reminders:ServiceKey    = "orleans-db"
ConnectionStrings:orleans-db    = "Host=...;Database=..."
```

Orleans' `ApplyConfiguration` reads these sections and delegates to the registered provider builders, which configure the ADO.NET providers with the correct connection strings and invariants.

## Schema Provisioning Order

`WithDatabaseSetup` runs embedded SQL scripts in order:

1. **Main** — creates the `OrleansQuery` table (query registry)
2. **Clustering** — creates `OrleansMembershipVersionTable`, `OrleansMembershipTable`, and stored procedures
3. **Persistence** — creates `OrleansStorage` table and stored procedures
4. **Reminders** — creates `OrleansRemindersTable` and stored procedures

## Switching Databases

Swap the Aspire resource builder — everything else stays the same:

```csharp
// PostgreSQL
var db = builder.AddPostgres("pg").AddDatabase("orleans-db");

// SQL Server
var db = builder.AddSqlServer("sql").AddDatabase("orleans-db");

// MySQL
var db = builder.AddMySql("mysql").AddDatabase("orleans-db");
```

## Multiple Grain Storage Providers

```csharp
// AppHost
var orleans = builder.AddOrleans("cluster")
    .WithClustering(db)
    .WithGrainStorage("Default", db)
    .WithGrainStorage("Archive", archiveDb)
    .WithDatabaseSetup(db);

// Grain
public class MyGrain(
    [PersistentState("state", "Default")] IPersistentState<MyState> state,
    [PersistentState("archive", "Archive")] IPersistentState<ArchiveState> archive
) : Grain, IMyGrain { }
```

Each named provider reads from `Orleans:GrainStorage:{Name}:ProviderType` and `Orleans:GrainStorage:{Name}:ServiceKey`.

## Code Generation Best Practices

1. **Always use `WithDatabaseSetup`** in the AppHost to auto-provision schemas — never require manual SQL scripts.
2. **Always call `WaitFor(db)`** on projects that reference Orleans, so the database is ready before the silo starts.
3. **Use `.AsClient()`** when wiring a client project — this provides only clustering config, not full silo config.
4. **Use `UseOrleans` and `UseOrleansClient`** — call `silo.UseAdoNet()` in silo projects inside `UseOrleans` and `client.UseAdoNetClient()` in client projects inside `UseOrleansClient`.
5. **Feature flags are optional** — only use `OrleansFeature` flags if the user explicitly wants to skip certain schemas.
6. **Don't hardcode connection strings** — Aspire injects them automatically via configuration.
7. **Don't manually configure ADO.NET invariants** — the packages auto-detect the correct invariant from the provider type.
8. **Use named grain storage** for multiple persistence stores — each name maps to a separate configuration section.

---

# Gluetun VPN — `Shiny.Aspire.Hosting.Gluetun`

## Setup

Install `Shiny.Aspire.Hosting.Gluetun` in the Aspire AppHost project.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var vpn = builder.AddGluetun("vpn")
    .WithVpnProvider("mullvad")
    .WithWireGuard(builder.AddParameter("wireguard-key", secret: true))
    .WithServerCountries("US", "Canada");

var scraper = builder.AddContainer("scraper", "my-scraper")
    .WithHttpEndpoint(targetPort: 8080);

vpn.WithRoutedContainer(scraper);

builder.Build().Run();
```

## API Reference

### AddGluetun

```csharp
public static IResourceBuilder<GluetunResource> AddGluetun(
    this IDistributedApplicationBuilder builder,
    string name,
    int? httpProxyPort = null,
    int? shadowsocksPort = null)
```

Creates a Gluetun container resource with:
- Image: `qmcgaw/gluetun:latest` from `docker.io`
- `--cap-add NET_ADMIN` runtime arg
- `--device /dev/net/tun` runtime arg
- Docker Compose publish callback that sets `cap_add`, `devices`, and transfers ports from routed containers

Optional port parameters expose Gluetun's built-in HTTP proxy (target port 8888) and Shadowsocks proxy (target port 8388).

### WithVpnProvider

```csharp
vpn.WithVpnProvider("mullvad");
```

Sets the `VPN_SERVICE_PROVIDER` environment variable. Required for all Gluetun setups.

### WithOpenVpn

```csharp
// String credentials
vpn.WithOpenVpn("username", "password");

// Aspire parameter resources (recommended for secrets)
vpn.WithOpenVpn(
    builder.AddParameter("openvpn-user"),
    builder.AddParameter("openvpn-pass", secret: true));
```

Sets `VPN_TYPE=openvpn`, `OPENVPN_USER`, and `OPENVPN_PASSWORD`.

### WithWireGuard

```csharp
// String key
vpn.WithWireGuard("my-private-key");

// Aspire parameter resource (recommended for secrets)
vpn.WithWireGuard(builder.AddParameter("wireguard-key", secret: true));
```

Sets `VPN_TYPE=wireguard` and `WIREGUARD_PRIVATE_KEY`.

### WithServerCountries / WithServerCities

```csharp
vpn.WithServerCountries("US", "Canada", "Germany");
vpn.WithServerCities("New York", "Toronto");
```

Values are comma-joined and set as `SERVER_COUNTRIES` / `SERVER_CITIES` environment variables.

### WithHttpProxy / WithShadowsocks

```csharp
vpn.WithHttpProxy();           // HTTPPROXY=on
vpn.WithHttpProxy(false);      // HTTPPROXY=off
vpn.WithShadowsocks();         // SHADOWSOCKS=on
vpn.WithShadowsocks(false);    // SHADOWSOCKS=off
```

### WithFirewallOutboundSubnets

```csharp
vpn.WithFirewallOutboundSubnets("10.0.0.0/8", "192.168.0.0/16");
```

Sets `FIREWALL_OUTBOUND_SUBNETS` (comma-joined). Useful for allowing traffic to local network resources outside the VPN tunnel.

### WithTimezone

```csharp
vpn.WithTimezone("America/New_York");
```

Sets the `TZ` environment variable.

### WithGluetunEnvironment

```csharp
// String value
vpn.WithGluetunEnvironment("DNS_ADDRESS", "1.1.1.1");

// Aspire parameter resource
vpn.WithGluetunEnvironment("UPDATER_PERIOD", builder.AddParameter("updater-period"));
```

Generic passthrough for any Gluetun environment variable not covered by the typed methods.

### WithRoutedContainer

```csharp
vpn.WithRoutedContainer(scraper);
vpn.WithRoutedContainer(downloader);
```

Routes a container's traffic through the Gluetun VPN tunnel. Each call:
1. Adds a `GluetunRoutedResourceAnnotation` to the Gluetun resource
2. Sets `--network container:<vpn-name>` runtime args on the routed container
3. On Docker Compose publish, sets `network_mode: "service:<vpn-name>"` and transfers port mappings to the Gluetun service

You can route multiple containers through the same VPN.

## Docker Compose Output

When published as Docker Compose, routed containers automatically get:

```yaml
services:
  vpn:
    image: qmcgaw/gluetun:latest
    cap_add:
      - NET_ADMIN
    devices:
      - /dev/net/tun
    environment:
      - VPN_SERVICE_PROVIDER=mullvad
      - VPN_TYPE=wireguard
      - WIREGUARD_PRIVATE_KEY=${wireguard-key}
      - SERVER_COUNTRIES=US,Canada
    ports:
      - "8080:8080"    # forwarded from scraper
  scraper:
    image: my-scraper
    network_mode: "service:vpn"
    # ports moved to vpn service
```

## Types

### GluetunResource

```csharp
namespace Aspire.Hosting.ApplicationModel;
public class GluetunResource(string name) : ContainerResource(name);
```

### GluetunRoutedResourceAnnotation

```csharp
namespace Aspire.Hosting.ApplicationModel;
public sealed record GluetunRoutedResourceAnnotation(
    GluetunResource GluetunResource,
    ContainerResource RoutedResource) : IResourceAnnotation;
```

Stored on the Gluetun resource. References each container that routes through it.

## Gluetun Code Generation Best Practices

1. **Always use `WithVpnProvider`** — it is required for Gluetun to connect to any VPN service.
2. **Use `ParameterResource` for secrets** — never hardcode private keys or passwords in the AppHost. Use `builder.AddParameter("key", secret: true)`.
3. **Call `WithRoutedContainer` on the VPN builder** — not on the container. The method is on `IResourceBuilder<GluetunResource>`.
4. **Ports transfer automatically** — when a container is routed through Gluetun, its endpoints are served on the Gluetun container in Docker Compose. Do not manually duplicate port mappings.
5. **Use `WithGluetunEnvironment` for provider-specific settings** — the typed methods cover common settings, but many providers have additional options documented in the Gluetun wiki.
6. **Use `WithFirewallOutboundSubnets` for local network access** — if routed containers need to reach local services (databases, APIs) outside the VPN, allow those subnets explicitly.
7. **Multiple containers can share one VPN** — call `WithRoutedContainer` multiple times on the same Gluetun resource.

---

# Shiny.Aspire.Hosting.Tunnel

Gives something that is only listening on localhost a public address. Install
`Shiny.Aspire.Hosting.Tunnel` plus the provider package you need, in the **AppHost** project only —
nothing is installed in the projects being published.

## The core idea

A tunnel is a resource whose **public URL is its connection string**. That single decision is what
makes the rest work without new concepts:

- `WithReference(tunnel)` injects it as `ConnectionStrings__<name>`
- `WithTunnelUrl("PUBLIC_URL", tunnel)` names the variable yourself
- both wait for the tunnel to actually open before the referencing resource starts

A tunnel opens when its target's **endpoints are allocated**, which is before the target process is
launched. A service may therefore reference its own tunnel — the usual case, since a webhook
receiver has to hand out its own address.

## Choosing a provider

| Method | Package | Address | Needs |
|---|---|---|---|
| `WithQuickTunnel()` | `.Ssh` | assigned, changes on reconnect | nothing |
| `WithSshTunnel(host, configure)` | `.Ssh` | yours | an SSH server you can log in to |
| `WithShinyRelayTunnel(relay)` | core | yours | the relay, hosted here or elsewhere |
| `WithAzureRelayTunnel(cs)` | `.AzureRelay` | stable, yours | an Azure Relay namespace |
| `WithCloudflareTunnel()` | `.Cloudflare` | assigned, or your domain | Docker |
| `WithNgrokTunnel(token)` | `.Ngrok` | assigned, or reserved domain | Docker + an ngrok account |

The first four run inside the AppHost as managed code. The last two run the vendor's agent in a
container.

## Two spellings, always

Every provider offers `AddXTunnel(...)` on the builder (returns the tunnel, for referencing it) and
`WithXTunnel(...)` on a target resource (creates and attaches it in one line, returns the target).

```csharp
// Attach in one line — the tunnel resource is named "{target}-tunnel"
api.WithQuickTunnel();

// Keep a handle, because something needs the URL
var tunnel = builder.AddQuickTunnel("public");
api.WithTunnel(tunnel);
worker.WithTunnelUrl("API_PUBLIC_URL", tunnel);
```

Give `name:` explicitly when a resource has **more than one** tunnel — both default to
`{target}-tunnel` and Aspire rejects duplicate resource names.

## Quick tunnels (`.Ssh`)

```csharp
api.WithQuickTunnel();                                   // pinggy.io, the default
api.WithQuickTunnel(QuickTunnelHost.Sish);               // tuns.sh
api.WithQuickTunnel(subdomain: "<access-token>");        // pinggy: lifts the 60-minute cap
api.WithQuickTunnel(configure: o => o.AutoReconnect = false);
```

`QuickTunnelHost` values are `Pinggy` (default), `Sish`, `Serveo`, `LocalhostRun`. Prefer `Pinggy`:
it reports the address it assigns in a form that can actually be read back. `LocalhostRun` never
confirms the session request carrying the URL, so it needs `o.PublicUrl` set explicitly.

## SSH (`.Ssh`)

```csharp
api.WithSshTunnel("tunnel.example.com", ssh =>
{
    ssh.Username = "deploy";
    ssh.PrivateKeyPath = "/home/me/.ssh/id_ed25519";
    ssh.RemoteBindAddress = "0.0.0.0";          // needs GatewayPorts on the server
    ssh.RemotePort = 8080;                      // 0 asks the server to allocate one
    ssh.PublicUrl = "https://api.example.com";  // where it really answers, if a proxy fronts it
    ssh.HostKeyFingerprints.Add("SHA256:47DEQpj8HBSa+…");
});
```

Options are `SshTunnelOptions` from `Shiny.Net.HttpServer.Ssh` — the same type a MAUI app uses, so a
tunnel is described identically on both sides. **Connecting fails unless a host key is pinned or
`AcceptAnyHostKey` is set.** That is deliberate, not a bug to work around silently.

## The Shiny relay (core)

The public end of a Shiny tunnel, hosted in the app model: a control port where clients register and
a public port where traffic arrives, routed by Host header. Use it when devices — a phone running
Shiny.Net.HttpServer, an embedded server — need somewhere to register into a dev environment.

```csharp
var relay = builder.AddShinyRelay("relay", controlPort: 5050, publicPort: 8080)
    .WithToken(builder.AddParameter("relay-token", secret: true))
    .WithDomain("localtest.me", scheme: "http", includePort: true)
    .WithBindAddress("0.0.0.0", clientHost: "192.168.1.20")   // so a phone can reach it
    .ConfigureRelay(o => o.MaxTunnels = 20);

api.WithShinyRelayTunnel(relay, subdomain: "api");
```

The relay's ports are its own, not Aspire-allocated — it listens inside the AppHost process, so pick
free ports. Its connection string is `Host=…;Port=…;UseTls=…;Token=…`, matching the properties of
`RelayTunnelOptions` on the client side.

Against a relay elsewhere (your VPS), no relay resource is needed:

```csharp
api.WithShinyRelayTunnel("relay.example.com", port: 5050, token: token, subdomain: "api");
```

## Azure Relay (`.AzureRelay`)

```csharp
var cs = builder.AddParameter("relay-cs", secret: true);

api.WithAzureRelayTunnel(cs, hybridConnectionName: "api");
api.WithAzureRelayTunnel(cs, "api", configure: o => o.Mode = AzureRelayMode.Http);
```

The hybrid connection name may come from the connection string's `EntityPath` instead.

## Container agents (`.Cloudflare`, `.Ngrok`)

```csharp
api.WithCloudflareTunnel();
api.WithNgrokTunnel(builder.AddParameter("ngrok-token", secret: true));

// A named Cloudflare tunnel on a host name you control — ingress is configured in Cloudflare,
// so the URL is stated rather than discovered:
builder.AddCloudflareTunnel("public")
    .WithNamedTunnel(builder.AddParameter("cf-token", secret: true), "https://api.example.com");

// A reserved ngrok domain:
builder.AddNgrokTunnel("public", token)
    .WithOrigin(api.GetEndpoint("http"))
    .WithDomain("api.ngrok.app");
```

Both agents announce their assigned address in their own output; it is read back out of the
container's log stream. ngrok **requires** an auth token — there is no anonymous mode.

## Reaching a service through a bastion (`.Ssh`)

The opposite direction: something the app model needs but cannot reach directly.

```csharp
var db = builder
    .AddSshPortForward("staging-db", "bastion.example.com", "10.0.0.5", 5432, ssh =>
    {
        ssh.Username = "ops";
        ssh.PrivateKeyPath = "/home/me/.ssh/id_ed25519";
        ssh.AcceptAnyHostKey = true;
    })
    .WithLocalPort(15432)          // optional; ephemeral and stable across reconnects otherwise
    .WithContainerAccess()         // binds 0.0.0.0 so container resources can reach it
    .WithConnectionString(f => ReferenceExpression.Create(
        $"Host={f.Host};Port={f.Port};Database=app;Username=app;Password={password}"
    ));

builder.AddProject<Projects.Api>("api").WithReference(db);
```

Without `WithConnectionString` the value is `host:port`. `f.Host` resolves per caller — `localhost`
for a project, the container host bridge for a container.

## A custom provider (core)

```csharp
var tunnel = builder.AddTunnel("public", "my-provider", (context, ct) =>
    ValueTask.FromResult<ITunnelProvider>(
        new MyProvider(context.TargetHost, context.TargetPort, context.LoggerFactory)
    )
);

api.WithTunnel(tunnel);
```

`ITunnelProvider` is `Shiny.Net.HttpServer.Tunneling.ITunnelProvider` — an `IConnectionListener` that
yields connections arriving from somewhere other than a local socket. Everything it yields is pumped
into the target endpoint byte for byte, so WebSockets, SSE and gRPC streaming pass through unchanged.

## Types

```csharp
namespace Aspire.Hosting.ApplicationModel;

public interface ITunnelResource : IResource
{
    EndpointReference? TargetEndpoint { get; }
    string? PublicUrl { get; }
    ReferenceExpression PublicUrlExpression { get; }
    Task<string> GetPublicUrlAsync(CancellationToken cancellationToken = default);
}

// In-process tunnels
public abstract class TunnelResource : Resource, ITunnelResource, IResourceWithConnectionString, IResourceWithWaitSupport;
public sealed class InProcessTunnelResource : TunnelResource;   // AddTunnel, relay, Azure Relay
public sealed class SshTunnelResource : TunnelResource;         // AddSshTunnel, AddQuickTunnel

// Container agents
public abstract class ContainerTunnelResource : ContainerResource, ITunnelResource, IResourceWithConnectionString;
public sealed class CloudflaredResource : ContainerTunnelResource;
public sealed class NgrokResource : ContainerTunnelResource;

// Not tunnels
public sealed class ShinyRelayResource : Resource, IResourceWithConnectionString, IResourceWithWaitSupport;
public sealed class SshPortForwardResource : Resource, IResourceWithConnectionString, IResourceWithWaitSupport;
```

## Tunnel Code Generation Best Practices

1. **Publish a cleartext endpoint.** TLS is terminated at the public end of the tunnel, so what
   arrives is plain HTTP. `http` is chosen by default for that reason; pass `endpointName` only to
   name another cleartext endpoint. Pointing a tunnel at an `https` endpoint fails every request.
2. **Use `ParameterResource` for every secret** — ngrok and Cloudflare tokens, relay tokens, Azure
   Relay connection strings. Never hardcode them in the AppHost.
3. **Never write an assigned URL into config.** Quick tunnels, cloudflared quick tunnels and free
   ngrok tunnels get a new address on every reconnect. Pass it with `WithTunnelUrl` or
   `WithReference`, and read the current one from the dashboard.
4. **Name the tunnel when there is more than one on a target** — the default name collides.
5. **Prefer the in-process providers** for anything that has to work on every machine and in CI:
   they need no Docker, no binary and no account.
6. **Pin host keys for an SSH server you own.** `AcceptAnyHostKey` is right for a throwaway hosted
   endpoint and wrong for infrastructure you control.
7. **Put authentication in front of anything that matters.** A tunnel hostname is unguessable, not
   private, and anyone who has it can reach the service.
8. **Do not expect tunnels in the manifest** — they are excluded on purpose, being a development-time
   affordance for a machine that is not on the internet.
