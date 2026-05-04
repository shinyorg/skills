# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for Bluetooth LE, GPS, geofencing, background jobs, push notifications, HTTP transfers, OBD-II diagnostics, music, speech recognition and synthesis, health data, MAUI Shell navigation, contact store, shared UI controls, mediator/CQRS, document store, spatial data, DI, localization, hosting modules, and .NET Aspire integrations across .NET MAUI, Blazor, and ASP.NET Core.

## Plugins & Skills

This repository is organized into **plugins**, each containing one or more **skills**. A plugin groups related skills together (e.g. all mobile client libraries, all UI controls). Each skill is a self-contained `SKILL.md` file with trigger conditions, code generation rules, best practices, and optional API reference docs. Install only the plugins relevant to your project.

| Plugin | Description | Skills |
|--------|-------------|--------|
| `shiny-client` | Cross-platform mobile libraries for .NET MAUI, iOS, and Android — Bluetooth LE (client & peripheral hosting), GPS tracking, geofencing (native + GPS-direct), background jobs, local & push notifications (APNs/FCM/Firebase Messaging/Azure), HTTP background transfers (with Azure Blob Storage uploads), OBD-II vehicle diagnostics (ELM327/OBDLink auto-detection, custom commands, pluggable transports), music library access (ShazamKit identification, custom playlists, synced lyrics, Apple Music streaming), health data (HealthKit/Health Connect) with read/write and real-time observation, speech-to-text, text-to-speech, audio capture and playback, and core infrastructure with DI, lifecycle hooks, and native hosting | `shiny-core`, `shiny-bluetoothle`, `shiny-ble-hosting`, `shiny-firebase`, `shiny-health`, `shiny-http-transfers`, `shiny-jobs`, `shiny-locations`, `shiny-music`, `shiny-notifications`, `shiny-obd`, `shiny-push`, `shiny-speech` |
| `shiny-maui` | .NET MAUI application components — Shell navigation with source-generated routes, deep linking, page/ViewModel registration, AI-driven navigation via Microsoft.Extensions.AI, multi-segment navigation builder, pluggable dialogs, and cross-platform device contact store with CRUD, LINQ queries, paging, and permissions | `shiny-contactstore`, `shiny-maui-shell` |
| `controls` | Shiny Controls — shared UI components for .NET MAUI and Blazor with full parity: TableView (14 cell types), FloatingPanel/OverlayHost/ShinyContentPage (bottom/top sheets with detents), ShinyDurationPicker, FrostedGlassView (native blur/glass effect), Toast service (code-invoked toast notifications with queue/stack, auto-dismiss, spinner, progress bar), ChatView (per-participant avatars, image messages, typing indicator), ImageViewer with zoom, ImageEditor (crop/rotate/draw/text, PNG/JPEG/WEBP export), AutoCompleteEntry, CountryPicker, AddressEntry with geocoding, PillView badges, SecurityPin, SignaturePad (canvas drawing with PNG export), Fab/FabMenu, Scheduler/calendar views (pinch-to-zoom, agenda, infinite scroll), Markdown renderer and editor, MermaidDiagrams, TextToSpeechButton, and haptic feedback | `shiny-controls` |
| `shiny-mediator` | Mediator/CQRS pattern for .NET — request/response handlers, commands, events, streams (EventStream/WaitForSingleEvent/Subscribe), middleware pipelines (cache, resilient, sample, throttle, validate, main thread, timer refresh), source-generated HTTP client proxies, OpenAPI client generation, SSE endpoint auto-generation, AI tools integration via Microsoft.Extensions.AI, and project scaffolding with code generation | `shiny-mediator` |
| `shiny-data` | Lightweight AOT-compatible data libraries — DocumentDB schema-free JSON document store (SQLite, LiteDB, CosmosDB, MySQL, SQL Server, PostgreSQL) with LINQ queries, batch operations, SQLCipher encryption, hot backup, diff tracking (JsonPatchDocument), and AI tool exposure; Spatial geospatial database with SQLite R*Tree indexing, custom geometry algorithms, geofencing package, and pre-built US/Canada geographic databases | `shiny-documentdb`, `shiny-spatial` |
| `shiny-aspire` | Shiny .NET Aspire integrations — Orleans ADO.NET hosting with selective schema provisioning (clustering, persistence, reminders) and Gluetun VPN container routing with WireGuard/OpenVPN, HTTP proxy, Shadowsocks, and firewall subnet controls | `shiny-aspire-orleans` |
| `shiny-extensions` | Source generators and cross-platform utilities — attribute-driven DI registration with keyed services, persistent key/value stores (mobile/desktop/Blazor WASM with session storage), AOT-compliant reflection via source generation (with AssemblyInfo generation and ReflectorJsonConverter), strongly-typed .resx localization generator with format-method generation, modular MAUI hosting with IMauiModule and full lifecycle hooks, and modular ASP.NET Core web hosting with IWebModule | `localizegen`, `shiny-di`, `shiny-maui-hosting`, `shiny-reflector`, `shiny-stores`, `shiny-web-hosting` |

## Installation

All skills in this repository are hosted on [GitHub](https://github.com/shinyorg/skills). For full installation instructions for both **Claude Code** and **GitHub Copilot**, see the [AI Skills documentation](https://shinylib.net/foundation/ai-skills/).

## Repository Layout

```
.claude-plugin/
  marketplace.json              # Marketplace manifest — lists all plugins
plugins/
  <plugin-name>/
    .claude-plugin/
      plugin.json               # Plugin manifest — name, version, description, keywords
    skills/
      <skill-name>/
        SKILL.md                # Skill definition — triggers, guidance, code gen rules
        reference/              # (optional) API reference docs and additional context
```

### Key Files

- **`marketplace.json`** — Top-level manifest that registers the marketplace and points to each plugin via relative paths.
- **`plugin.json`** — Per-plugin manifest containing name, version, description, keywords, and the relative path to its skills directory.
- **`SKILL.md`** — The skill itself: YAML front matter with name and description, followed by trigger keywords, usage guidance, code generation instructions, and best practices. Each skill may also include a `reference/` directory with API docs and examples.

## License

[MIT](LICENSE)
