---
name: shiny-aspnet
description: Generate code for the Shiny.AspNet libraries on ASP.NET Core - WebDAV, CalDAV/CardDAV, tus resumable uploads, file sync, private NuGet feed and npm registry, Shiny.DocumentDb REST/SSE endpoints, JSON Patch, webhook verification/signing, Idempotency-Key, RFC 9530 Content-Digest, ACME automatic TLS for Kestrel, mDNS discovery, SSH quick tunnels and cloudflared/ngrok/Tailscale tunnel agents
auto_invoke: true
triggers:
  - Shiny.AspNet
  - MapWebDav
  - MapCalDav
  - MapTus
  - MapFileSync
  - MapNuGetFeed
  - MapNpmRegistry
  - MapDocuments
  - MapDocumentCollection
  - JsonPatchDocument
  - AddJsonPatch
  - AddWebhooks
  - MapWebhook
  - RequireWebhookSignature
  - UseWebhookVerification
  - AddIdempotency
  - RequireIdempotencyKey
  - AddContentDigest
  - RequireContentDigest
  - AddAcme
  - AddAcmeRegistry
  - AddServerAdvertisement
  - AddServerLocator
  - AddSshTunnel
  - AddQuickTunnel
  - AddCloudflareTunnel
  - AddNgrokTunnel
  - AddTailscaleFunnel
---

# Shiny.AspNet Skill

You are an expert in the Shiny.AspNet libraries: independent ASP.NET Core packages, most of them ports of the
Shiny.Net.HttpServer add-ons re-hosted on Kestrel.

**Documentation**: https://shinylib.net/aspnet/
**Repository**: https://github.com/shinyorg/aspnet
**Target**: `net10.0`, ASP.NET Core. All packages are trim and Native AOT clean.

For `IWebModule` / `AddInfrastructureModules` use the `shiny-web-hosting` skill instead.

## When to Use This Skill

- Serving files over WebDAV, calendars/contacts over CalDAV/CardDAV, resumable uploads (tus), or folder sync
- Hosting a private NuGet feed or npm registry inside an ASP.NET app
- Exposing a Shiny.DocumentDb document type as REST + Server-Sent Events
- JSON Patch, webhook verification, idempotency keys or content digests on ASP.NET endpoints
- Automatic Let's Encrypt/ACME certificates for Kestrel (a LettuceEncrypt replacement)
- Advertising the app on mDNS, or publishing it through an SSH tunnel, cloudflared, ngrok or Tailscale

**Do not** generate these against `Shiny.Net.HttpServer` types (`HttpServer`, `ShinyHttpServerBuilder`) — that is the
non-ASP.NET server with its own skill (`shiny-httpserver`). Namespaces here are `Shiny.AspNet.<Name>`.

## Conventions (apply to every library)

- Services: `builder.Services.Add<Name>(...)`. Endpoints: `app.Map<Name>(prefix, o => ...)` on any
  `IEndpointRouteBuilder`, including `MapGroup(...)` groups. Middleware: `app.Use<Name>()`.
- Every `Map<Name>` returns an `IEndpointConventionBuilder` — apply ASP.NET's own `.RequireAuthorization()`,
  `.RequireCors(...)`, `.RequireRateLimiting(...)`, `.WithMetadata(...)` to it. There is no `RequireIpFilter`.
- Upload-heavy routes (WebDav PUT, tus, FileSync packs, NuGet push, npm publish) raise Kestrel's 30 MB
  `MaxRequestBodySize` for their own routes only. An explicit `[RequestSizeLimit]` / `RequestSizeLimitAttribute`
  added via `.WithMetadata(...)` wins. Reverse proxies in front (IIS, nginx) need their own limits raised.
- Store and hook interfaces mirror Shiny.Net.HttpServer; request-level hooks return `Task` (not `ValueTask`).
- Things that need the bound port (tunnels, discovery, ACME renewal) start after Kestrel is listening — they
  read `IServerAddressesFeature`, so `http://127.0.0.1:0`, `--urls` and `ASPNETCORE_URLS` all work.

## Shiny.AspNet.WebDav
A WebDAV (RFC 4918) class 1 & 2 server for ASP.NET Core — mount a directory in Finder, Windows Explorer or any WebDAV client.

- PROPFIND/PROPPATCH/MKCOL/COPY/MOVE/LOCK/UNLOCK plus GET/HEAD with ranges & conditional requests
- `If` header (lock tokens and ETags), shared/exclusive locks, RFC 4331 quota, dead properties
- Built-in browser file manager (upload, new folder, rename, delete) driven by the mount's own verbs
- Read-only by default; atomic, size-bounded uploads; hardened XML parsing; containment checks incl. symlinks
- Pluggable `IWebDavFileSystem` and `IWebDavPropertyStore`
- Returns an `IEndpointConventionBuilder` — `RequireAuthorization`, `RequireCors`, etc. cover every verb; `RequireAuthorizationForChanges` leaves reads open

```csharp
app.MapWebDav("/dav", o =>
{
    o.RootPath = Path.Combine(app.Environment.ContentRootPath, "share");
    o.AllowWrite = true;
    o.AllowDelete = true;
})
.RequireAuthorization();
```

## Shiny.AspNet.CalDav
CalDAV (RFC 4791) and CardDAV (RFC 6352) for ASP.NET Core — serve calendars and contacts to iOS/macOS Calendar & Contacts, Thunderbird and DAVx⁵.

- Discovery: `/.well-known/caldav|carddav`, current-user-principal, home sets
- calendar-query (recurrence-aware time ranges), multiget, free-busy, addressbook-query, sync-collection (RFC 6578)
- PUT validation with RFC preconditions, If-Match / If-None-Match, per-principal isolation
- File-backed stores included; implement `ICalendarStore` / `IAddressBookStore` for your own data
- Built on Shiny.AspNet.WebDav

```csharp
app.MapCalDav("/dav", o =>
{
    o.CalendarStore = new FileCalendarStore(Path.Combine(dataPath, "calendars"));
    o.AddressBookStore = new FileAddressBookStore(Path.Combine(dataPath, "contacts"));
})
.RequireAuthorization();
```

## Shiny.AspNet.Tus
Resumable uploads over the tus 1.0.0 protocol — an upload that drops half way resumes from the last byte the server kept.

- Core protocol plus creation, creation-with-upload, defer-length, termination, expiration, checksum (sha1/sha256/md5) and concatenation
- Keeps every byte of an interrupted request; a stalled request is taken over instead of locking the client out
- Streams to the store, never buffers; `DiskTusStore` survives restarts (offset = file length)
- Lifts Kestrel's 30 MB body limit for tus requests (the protocol bounds them); an explicit `RequestSizeLimit` wins
- Expiry sweep tied to `IHostApplicationLifetime`; `WithTusHeaders()` for CORS

```csharp
app.MapTus("/files", o =>
{
    o.Store = new DiskTusStore(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "uploads"));
    o.Expiration = TimeSpan.FromDays(1);
    o.OnUploadCompleteAsync = async ctx => { await using var file = await ctx.OpenReadAsync(); /* ... */ };
})
.RequireAuthorization();
```

## Shiny.AspNet.FileSync
Dropbox-style file sync server: content-defined chunks stored once by hash, so an edit moves only what changed.

- Change feed with long-poll, optimistic revisions (409 conflicts, 422 missing chunks), tombstones
- Pack uploads/downloads (Range-resumable) and resumable pack uploads over tus at `{prefix}/uploads`
- Per-user stores via `StoreSelector`; garbage collection of unreferenced chunks
- Works with the existing `Shiny.Net.HttpServer.FileSync.Client` (background transfers on iOS/Android)
- One `IEndpointConventionBuilder` covers every route, tus included

```csharp
app.MapFileSync("/sync", o => o.Store = new DiskFileSyncStore(Path.Combine(dataDir, "sync")))
   .RequireAuthorization();
```

## Shiny.AspNet.NuGet
A private NuGet feed for ASP.NET Core: the NuGet V3 protocol, so `dotnet restore`, `dotnet nuget push`, Visual Studio and Rider work unchanged.

- Service index, package content, registrations, search and autocomplete; push, unlist/relist and delete
- Uses NuGet's own folder layout on disk, so the directory is also a plain local package source
- Version rules from `NuGet.Versioning`; the nuspec is read by hand, so it is trim/AOT-clean
- API keys (`X-NuGet-ApiKey`) or ASP.NET authorization; `OnBeforePushAsync` / `OnPackagePushedAsync` hooks
- Write routes raise Kestrel's 30 MB body limit to `MaxPackageSize` (250 MB by default)

```csharp
app.MapNuGetFeed("/nuget", o =>
{
    o.Store = new DiskNuGetPackageStore(Path.Combine(dataDir, "packages"));
    o.ApiKeys.Add(builder.Configuration["NuGet:ApiKey"]!);
})
.ForWrites(w => w.RequireRateLimiting("push"));
// dotnet nuget add source http://host:5000/nuget/v3/index.json -n mine
```

## Shiny.AspNet.Npm
A private npm registry for ASP.NET Core: the registry API npm, pnpm and Yarn speak. No database, no Node.js.

- Install (full and abbreviated metadata, ETag revalidation), publish, dist-tags, deprecate, unpublish, owners, search, whoami, login
- Bearer tokens mapped to users; maintainer checks like the public registry
- Optional fall-through to registry.npmjs.org (`Upstream`); local packages always win
- Scoped packages in both URL forms; trim/AOT-clean
- Write routes raise Kestrel's 30 MB body limit to fit `MaxTarballSize` (100 MB by default) once base64-encoded

```csharp
app.MapNpmRegistry("/npm", o =>
{
    o.Store = new DiskNpmPackageStore(Path.Combine(dataDir, "npm"));
    o.Tokens[builder.Configuration["Npm:Token"]!] = "ci";
});
// .npmrc: @acme:registry=http://host:5000/npm/
```

## Shiny.AspNet.JsonPatch
RFC 6902 JSON Patch over RFC 6901 JSON Pointer for ASP.NET Core, on System.Text.Json.Nodes — trim and Native AOT clean.

- add / remove / replace / move / copy / test, applied all-or-nothing to a copy
- Apply to any `JsonNode`, or to a typed value via its source-generated `JsonTypeInfo<T>`
- `JsonPatchDocument` binds as a minimal-API parameter (`application/json-patch+json` only; RDG-compatible)
- Failures carry the RFC 5789 status (400/409/415/422) and an RFC 9457 problem naming the failed operation
- Public `JsonPointer`, RFC 6902 §4.6 `test` equality, fluent builder

```csharp
builder.Services.AddProblemDetails();
builder.Services.AddJsonPatch();
app.UseExceptionHandler();

app.MapPatch("/notes/{id:int}", (int id, JsonPatchDocument patch, NoteStore notes) =>
    Results.Json(patch.ApplyTo(notes.Get(id), AppJson.Default.Note), AppJson.Default.Note));
```

## Shiny.AspNet.DocumentDb
A Shiny.DocumentDb document type as a complete REST + Server-Sent Events resource in one line.

- List (filter / orderby / fields / skip-take / cursor), by-id, count, create, replace, delete
- PATCH as RFC 7396 merge patch or RFC 6902 JSON Patch, with ETag / If-Match (412/428)
- Server-side scopes enforced on reads and on both sides of writes (out-of-scope = 404)
- Field allow-list, live SSE tail with heartbeats, schema-free `MapDocumentCollection`
- Returns a `RouteGroupBuilder` with OpenAPI metadata, so `RequireAuthorization`, `RequireRateLimiting`, etc. cover every route

```csharp
builder.Services.AddDocumentStore(o => o.DatabaseProvider = new SqliteDatabaseProvider(path));

app.MapDocuments<Order>("/orders", o =>
{
    o.Operations = DocumentEndpoints.All;
    o.TypeInfo = AppJson.Default.Order;
    o.AllowFilterOn(x => x.Status, x => x.Total);
    o.Scope<ITenantContext>((tenant, _) => x => x.TenantId == tenant.TenantId);
})
.RequireAuthorization("orders");
```

## Shiny.AspNet.Webhooks
Verify inbound webhooks over the exact bytes that arrived, and sign the ones you send.

- Built-in verifiers: GitHub, Stripe, Slack, Standard Webhooks (Svix, Clerk, Resend, OpenAI…), generic HMAC-in-a-header
- Constant-time comparison, secret rotation, replay windows on signed timestamps
- Duplicate-delivery suppression (bounded in-memory store, or your own `IWebhookDeliveryStore`)
- `MapWebhook` verifies inline; or `[RequireWebhookSignature]` / `.RequireWebhookSignature(...)` + `app.UseWebhookVerification()` on minimal APIs, groups and MVC
- 401 problem details via `IProblemDetailsService`; the reason is logged, never returned
- `WebhookSigner` for outbound Standard Webhooks signatures

```csharp
builder.Services.AddWebhooks(o =>
{
    o.AddVerifier("stripe", WebhookSignature.Stripe(builder.Configuration["Stripe:WebhookSecret"]!));
    o.SuppressDuplicates();
});

app.MapWebhook("/hooks/github", WebhookSignature.GitHub(secret), async (WebhookContext ctx) => { /* ... */ });

app.UseWebhookVerification();
app.MapPost("/hooks/stripe", (WebhookContext hook) => Results.Ok()).RequireWebhookSignature("stripe");
```

## Shiny.AspNet.Idempotency
`Idempotency-Key` (IETF draft) middleware: a retried POST gets the stored original response instead of running the handler twice.

- Opt in per endpoint: `.RequireIdempotencyKey()`, `.WithIdempotency()`, `.DisableIdempotency()` on endpoints, groups or controllers; `[Idempotent]` / `[DisableIdempotency]`
- Replays status, headers and body with `Idempotent-Replayed: true`
- 409 + `Retry-After` while the first request is in flight; 422 when a key is reused with a different body
- Keys scoped by method, path and authenticated caller; 5xx/thrown requests are not stored
- Response copied as it is written: streaming, `BodyWriter`, `SendFileAsync` and `Content-Length` behave normally
- In-memory store by default; `AddDistributedCacheIdempotencyStore()` for Redis/SQL via `IDistributedCache`

```csharp
builder.Services.AddIdempotency(o => o.Expiration = TimeSpan.FromHours(6));

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdempotency();

app.MapPost("/payments", Charge).RequireIdempotencyKey();
```

## Shiny.AspNet.ContentDigest
RFC 9530 `Content-Digest` / `Repr-Digest` for ASP.NET Core — end-to-end integrity across proxies, gateways and tunnels.

- Verifies request bodies as they stream (never buffered); mismatch, malformed or unsupported → 400 problem + `Want-Content-Digest`
- `.RequireContentDigest()` refuses bodies without one; handlers that swallow a failed check are overruled
- Answers `Want-Content-Digest` / `Want-Repr-Digest`: small bodies get a header, streamed bodies a trailer (HTTP/2, HTTP/3 — Kestrel sends no trailers on HTTP/1.1)
- `[ContentDigest]` / `[DisableContentDigest]` attributes and `.WithContentDigest()` / `.DisableContentDigest()` conventions
- sha-256 and sha-512; `DigestFields` helpers to compute and parse

```csharp
builder.Services.AddContentDigest();

var app = builder.Build();
app.UseContentDigest();   // after routing, before UseRequestDecompression/UseResponseCompression

app.MapPost("/firmware", Upload).RequireContentDigest();
app.MapGet("/snapshot", Snapshot).WithContentDigest();
```

## Shiny.AspNet.Acme
Automatic HTTPS for Kestrel from Let's Encrypt, ZeroSSL or any ACME (RFC 8555) CA — a maintained replacement for LettuceEncrypt.

- Issues, stores and renews certificates, hot-swapping them into Kestrel with no restart (intermediate chain included)
- HTTP-01 answered ahead of your whole pipeline; TLS-ALPN-01 answered from the TLS handshake
- ACME Renewal Information (RFC 9773), External Account Binding (ZeroSSL), IP-address certificates, ECDSA/RSA
- `AcmeCertificateRegistry`: many certificates chosen by SNI, added/removed at runtime, PEM/PFX import
- In-box crypto only (no BouncyCastle/Certes); `Shiny.AspNet.Acme` meter for expiry alerts

```csharp
builder.WebHost.UseUrls("http://*:80", "https://*:443");
builder.Services.AddAcme(o =>
{
    o.Domains = ["example.com"];
    o.Email = "ops@example.com";
    o.AcceptTermsOfService = true;
    o.DirectoryUrl = AcmeDirectories.LetsEncryptStaging; // remove once it works
});
```

## Shiny.AspNet.Discovery
Advertise an ASP.NET Core app on the local network with mDNS/DNS-SD (Bonjour), and find the servers other machines advertise.

- Publishes on the port Kestrel actually bound (port 0 works), withdraws with a goodbye on shutdown
- Re-announces on network change; retries a refused registration and logs at Error when it gives up
- `IServerLocator`: `FindFirstAsync` / `FindAllAsync` / `WatchAsync`, yielding ready-to-use base URLs, never finding itself
- TXT records for path, scheme and your own metadata

```csharp
builder.WebHost.UseUrls("http://0.0.0.0:0");
builder.Services.AddServerAdvertisement(o => o.ServiceType = "_myapp._tcp");
builder.Services.AddServerLocator();

var peer = await locator.FindFirstAsync("_myapp._tcp", TimeSpan.FromSeconds(5));
```

## Shiny.AspNet.Ssh
SSH remote port forwarding (`ssh -R`) for a Kestrel app — publish through a VPS you own, or get a free public HTTPS address from a hosted tunnel (Pinggy, sish). Pure managed code; nothing to install.

- Forwards to the address Kestrel actually bound (works with port 0, `--urls`, `ASPNETCORE_URLS`)
- Starts once Kestrel is listening, closes before it stops
- Host keys are pinned — connecting unverified fails unless you opt out explicitly
- Auto-reconnect with backoff; `PublicUrl` / `State` change notifications (`QuickTunnel` is `INotifyPropertyChanged`)

```csharp
// a free public address while developing
builder.Services.AddQuickTunnel(autoStart: builder.Environment.IsDevelopment());

// or a host you own
builder.Services.AddSshTunnel(o =>
{
    o.Host = "tunnel.example.com";
    o.Username = "tunnel";
    o.PrivateKeyPath = keyPath;
    o.RemotePort = 8080;
    o.HostKeyFingerprints.Add("SHA256:…");
});
```

## Shiny.AspNet.Tunnels
Publish a Kestrel app through cloudflared, ngrok or Tailscale Funnel by supervising the vendor's own agent process.

- Reads the port off the running server and starts the agent once Kestrel is listening
- Reports the public URL the agent announces (`ITunnelAgent.PublicUrl`)
- Fails fast with a useful message when the binary is missing, exits early, or never announces a URL
- Kills the agent's whole process tree on shutdown

```csharp
builder.Services.AddCloudflareTunnel();     // or AddNgrokTunnel() / AddTailscaleFunnel()
```

## Gotchas to generate around

- **ContentDigest**: `app.UseContentDigest()` goes after routing and *before* `UseRequestDecompression` /
  `UseResponseCompression`. Kestrel sends no trailers on HTTP/1.1, so streamed responses larger than
  `MaxBufferedResponseBytes` (64 KB) carry no digest there; SSE/long-poll handlers must call `DisableBuffering()`.
- **Idempotency**: place `app.UseIdempotency()` after authentication/authorization (keys are scoped per caller).
  `AddDistributedCacheIdempotencyStore()` cannot guarantee exactly-once across instances (no atomic set-if-absent).
- **Acme**: Kestrel keeps one `ConfigureHttpsDefaults` callback — if the app sets its own, call `https.UseAcme(...)`
  inside it. HTTP-01 needs port 80 reachable; on HTTP/3 endpoints set `https.OnAuthenticate = null`.
- **JsonPatch**: a `JsonPatchDocument` handler parameter only binds `application/json-patch+json`; other content
  types are a 415 from routing. Add `services.AddJsonPatch()` + `app.UseExceptionHandler()` to turn an unhandled
  `JsonPatchException` into a problem response.
- **WebDav / CalDav**: locks and sync tokens are in-process — use sticky sessions behind a load balancer.
  `MapCalDav` maps `/.well-known/caldav|carddav` only when called on the app; inside a group use `MapCalDavWellKnown`.
- **Tus CORS**: a CORS policy that sets its own exposed headers must include `.WithTusHeaders()`.
- **Tunnels**: need an `http://` Kestrel binding to forward to. Tunnelled requests arrive from loopback with
  `Request.Scheme == "http"` — `UseHttpsRedirection` will redirect them; use `UseForwardedHeaders` where the tunnel
  sends `X-Forwarded-*`. `AddQuickTunnel` defaults to `autoStart: false`; SSH host keys must be pinned
  (`HostKeyFingerprints`) unless `AcceptAnyHostKey` is set explicitly.
- Put authentication in front of anything a tunnel publishes.
