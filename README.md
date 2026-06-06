# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for Bluetooth LE, GPS, geofencing, background jobs, push notifications, HTTP transfers, OBD-II diagnostics, music, speech recognition and synthesis, AI conversation, health data, MAUI Shell navigation, contact store, shared UI controls, mediator/CQRS, document store, spatial data, DI, localization, hosting modules, and .NET Aspire integrations across .NET MAUI, Blazor, and ASP.NET Core.

## Plugins & Skills

This repository is organized into **plugins**, each containing one or more **skills**. A plugin groups related skills together (e.g. all mobile client libraries, all UI controls). Each skill is a self-contained `SKILL.md` file with trigger conditions, code generation rules, best practices, and optional API reference docs. Install only the plugins relevant to your project.

| Plugin | Description | Skills |
|--------|-------------|--------|
| `shiny-client` | Cross-platform mobile libraries for .NET MAUI, iOS, and Android — Bluetooth LE (client & peripheral hosting, including L2CAP channels), GPS tracking with motion activity recognition, geofencing (native + GPS-direct), background jobs (native iOS/Android schedulers plus in-process jobs for plain .NET, Linux, macOS, and Blazor WASM), local & push notifications (APNs/FCM/Firebase Messaging/Azure), HTTP background transfers across iOS, Android, Windows, Linux, macOS, and Blazor WASM (Service Worker Background Sync, with Azure Blob Storage uploads), OBD-II vehicle diagnostics (ELM327/OBDLink auto-detection, custom commands, pluggable transports), music library access (ShazamKit identification, custom playlists, play counts, synced lyrics, Apple Music streaming), health data (HealthKit/Health Connect) with read/write and real-time observation, speech-to-text, text-to-speech, audio capture and playback, AI conversation service (wake word detection, acknowledgement modes, persistent message store, AI chat history), and core infrastructure with DI, lifecycle hooks, and native hosting | `shiny-core`, `shiny-bluetoothle`, `shiny-ble-hosting`, `shiny-firebase`, `shiny-health`, `shiny-http-transfers`, `shiny-jobs`, `shiny-locations`, `shiny-music`, `shiny-notifications`, `shiny-obd`, `shiny-push`, `shiny-speech`, `shiny-aiconversation` |
| `shiny-maui` | .NET MAUI application components — Shell navigation with source-generated routes, deep linking, page/ViewModel registration, AI-driven navigation via Microsoft.Extensions.AI, multi-segment navigation builder, pluggable dialogs, and cross-platform device contact store with CRUD, LINQ queries, paging, and permissions | `shiny-contactstore`, `shiny-maui-shell` |
| `shiny-controls` | Shiny Controls — shared .NET MAUI and Blazor UI components including TableView, FloatingPanel/OverlayHost, ShinyDurationPicker, FrostedGlassView, Toast service, ChatView, ImageViewer/ImageEditor, AutoCompleteEntry, CountryPicker, AddressEntry, PillView, SecurityPin, SignaturePad, Fab/FabMenu, scheduler/calendar views, TextEntry, Slider, ProgressBar, overlays, Markdown/Mermaid rendering, TextToSpeechButton, and feedback/haptics | `shiny-controls` |
| `shiny-mediator` | Mediator/CQRS pattern for .NET — request/response handlers, commands, events, streams (EventStream/WaitForSingleEvent/Subscribe), middleware pipelines (cache, resilient, sample, throttle, validate, main thread, timer refresh), source-generated HTTP client proxies, OpenAPI client generation, SSE endpoint auto-generation, AI tools integration via Microsoft.Extensions.AI, and project scaffolding with code generation | `shiny-mediator` |
| `shiny-data` | Lightweight AOT-compatible data libraries — DocumentDB schema-free JSON document store (SQLite, LiteDB, CosmosDB, IndexedDB for Blazor WASM, MySQL, SQL Server, PostgreSQL) with LINQ queries, spatial/geo queries, batch operations, SQLCipher encryption, hot backup, diff tracking (JsonPatchDocument), and AI tool exposure; Spatial geospatial database with SQLite R*Tree indexing, custom geometry algorithms, geofencing package, and pre-built US/Canada geographic databases | `shiny-documentdb`, `shiny-spatial` |
| `shiny-aspire` | Shiny .NET Aspire integrations — Orleans ADO.NET hosting with selective schema provisioning (clustering, persistence, reminders) and Gluetun VPN container routing with WireGuard/OpenVPN, HTTP proxy, Shadowsocks, and firewall subnet controls | `shiny-aspire-orleans` |
| `shiny-extensions` | Source generators and cross-platform utilities — attribute-driven DI registration with keyed services, persistent key/value stores (mobile/desktop/Blazor WASM with session storage), strongly-typed .resx localization generator with format-method generation, modular MAUI hosting with IMauiModule and full lifecycle hooks, and modular ASP.NET Core web hosting with IWebModule | `localizegen`, `shiny-di`, `shiny-maui-hosting`, `shiny-stores`, `shiny-web-hosting` |

## Installation

All skills in this repository are hosted on [GitHub](https://github.com/shinyorg/skills). For full installation instructions for both **Claude Code** and **GitHub Copilot**, see the [AI Skills documentation](https://shinylib.net/foundation/ai-skills/).

## Repository Layout

```
.claude-plugin/
  marketplace.json              # Claude marketplace manifest
.github/
  plugin/
    marketplace.json            # Copilot marketplace manifest
plugins/
  <plugin-name>/
    .claude-plugin/
      plugin.json               # Claude plugin manifest
    .github/
      plugin/
        plugin.json             # Copilot plugin manifest
    skills/
      <skill-name>/
        SKILL.md                # Skill definition — YAML metadata + guidance
        reference/              # (optional) API reference docs and additional context
```

### Key Files

- **`.claude-plugin/marketplace.json`** and **`.github/plugin/marketplace.json`** — Marketplace manifests for Claude Code and GitHub Copilot CLI, each pointing to the same plugin directories.
- **`.claude-plugin/plugin.json`** and **`.github/plugin/plugin.json`** — Per-plugin manifests containing name, version, description, keywords, and the relative path to the plugin's skills directory.
- **`SKILL.md`** — The skill itself: YAML front matter with portable Agent Skills metadata (`name`, `description`, `auto_invoke`, `triggers`, and optional `when_to_use`), followed by usage guidance, code generation instructions, and best practices. Keep `triggers` and `auto_invoke: true` in front matter for GitHub Copilot CLI discovery, and keep the prose sections for Claude-friendly guidance. Supporting docs should live in the skill's `reference/` directory so both Claude and Copilot load them consistently.

## License

[MIT](LICENSE)
