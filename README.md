# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for Bluetooth LE, GPS, geofencing, background jobs, push notifications, HTTP transfers, OBD-II diagnostics, music, speech recognition and synthesis, AI conversation, health data, MAUI Shell navigation, contact & calendar stores, shared UI controls, mediator/CQRS, document store, spatial data, DI, localization, hosting modules, and .NET Aspire integrations across .NET MAUI, Blazor, and ASP.NET Core.

## The `shiny` Plugin

Everything ships as **one plugin** — `shiny`. Install it once and every Shiny skill becomes available; the agent loads only the skill relevant to what you're building, so there is no cost to having them all installed.

```bash
# Claude Code
claude plugin marketplace add shinyorg/skills
claude plugin install shiny@shiny

# GitHub Copilot CLI
copilot plugin marketplace add https://github.com/shinyorg/skills
copilot plugin install shiny@shiny
```

## Skills

Each skill is a self-contained `SKILL.md` with trigger conditions, code generation rules, best practices, and optional API reference docs.

### Client — cross-platform libraries for iOS, Android, Windows, macOS, Linux, and Web

| Skill | What It Helps With |
|---|---|
| `shiny-core` | Hosting, DI, key-value stores, lifecycle hooks, platform abstractions |
| `shiny-bluetoothle` | BLE scanning, connecting, GATT operations, managed scanner |
| `shiny-ble-hosting` | BLE peripheral GATT server, advertising, L2CAP CoC channels |
| `shiny-jobs` | Background job scheduling — native iOS/Android schedulers plus in-process jobs |
| `shiny-locations` | GPS tracking, geofencing, motion activity recognition |
| `shiny-notifications` | Local notification scheduling, channels, badges, interactive actions |
| `shiny-push` | Push notifications — native FCM/APNs and Azure Notification Hubs |
| `shiny-firebase` | Firebase Cloud Messaging for iOS and Android |
| `shiny-http-transfers` | Background uploads and downloads with progress tracking |
| `shiny-data-sync` | Bidirectional JSON sync over HTTP with background outbox/inbox |
| `shiny-httpserver` | Embedded HTTP/1.1, HTTP/2 & HTTP/3 server where ASP.NET Core cannot run |
| `shiny-discovery` | Local network discovery — mDNS/DNS-SD, SSDP/UPnP, WS-Discovery |
| `shiny-obd` | OBD-II commands, adapter auto-detection, BLE/WiFi/serial transports |
| `shiny-music` | Music library permissions, querying, playback, lyrics, album art |
| `shiny-health` | HealthKit / Health Connect queries, writes, real-time observers |
| `shiny-contactstore` | Contact CRUD, fluent async query builder, permissions |
| `shiny-calendarstore` | Calendar & event CRUD, fluent async query builder, permissions |
| `shiny-speech` | Speech-to-text, text-to-speech, audio capture and playback |
| `shiny-aiconversation` | Chat client, wake word, STT/TTS, persistent message store |

### MAUI

| Skill | What It Helps With |
|---|---|
| `shiny-maui-shell` | Pages, ViewModels, navigation, source-generated routes |

### Controls — shared .NET MAUI and Blazor UI

| Skill | What It Helps With |
|---|---|
| `shiny-controls` | TableView, TreeView, FloatingPanel, ChatView, ImageViewer/ImageEditor, CameraView, MediaElement, scheduler, Markdown, barcodes, motion icons, and more |

### Mediator

| Skill | What It Helps With |
|---|---|
| `shiny-mediator` | Handlers, contracts, middleware, HTTP extension, OpenAPI generation |

### Data

| Skill | What It Helps With |
|---|---|
| `shiny-documentdb` | Schema-free JSON document store — queries, CRUD, indexes, AOT configuration |
| `shiny-firestore-mobile` | On-device native Firestore provider with offline persistence and snapshot listeners |
| `shiny-spatial` | Spatial queries, geometry types, R\*Tree indexing |

### Aspire

| Skill | What It Helps With |
|---|---|
| `shiny-aspire-orleans` | Orleans ADO.NET hosting, Gluetun VPN container routing, tunnelling |

### Extensions — source generators and utilities

| Skill | What It Helps With |
|---|---|
| `shiny-di` | Attribute-driven service registration, keyed services, categories |
| `shiny-stores` | Cross-platform key/value stores, persistent service binding |
| `shiny-reflector` | AOT-compliant source-generated property access, JSON, assembly info |
| `shiny-serialization` | Centralized AOT-safe JSON serializer with source-generated contexts |
| `shiny-extensions-push` | Server-side push dispatch — APNs, FCM, Web Push, WNS |
| `localizegen` | Strongly-typed localization from .resx files |
| `shiny-maui-hosting` | Modular `IMauiModule` configuration, lifecycle hooks |
| `shiny-web-hosting` | Modular `IWebModule` configuration for ASP.NET Core |
| `shiny-blazor-hosting` | `IAppSupport` for Blazor WASM — version, viewport, culture/time-zone events |

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
  shiny/
    .claude-plugin/
      plugin.json               # Claude plugin manifest
    plugin.json                 # Copilot plugin manifest (plugin root)
    skills/
      <skill-name>/
        SKILL.md                # Skill definition — YAML metadata + guidance
        reference/              # (optional) API reference docs and additional context
```

### Key Files

- **`.claude-plugin/marketplace.json`** and **`.github/plugin/marketplace.json`** — Marketplace manifests for Claude Code and GitHub Copilot CLI, each declaring the single `shiny` plugin. Copilot CLI accepts either location; both are included for explicit discoverability.
- **`plugins/shiny/.claude-plugin/plugin.json`** (Claude Code) and **`plugins/shiny/plugin.json`** (GitHub Copilot CLI) — The plugin manifests containing name, version, description, keywords, and the relative path to the skills directory. Claude Code requires the manifest under `.claude-plugin/`; Copilot CLI requires it at the plugin root. Keep the two files identical.
- **`SKILL.md`** — The skill itself: YAML front matter with portable Agent Skills metadata (`name`, `description`, `auto_invoke`, `triggers`, and optional `when_to_use`), followed by usage guidance, code generation instructions, and best practices. Keep `triggers` and `auto_invoke: true` in front matter for GitHub Copilot CLI discovery, and keep the prose sections for Claude-friendly guidance. Supporting docs should live in the skill's `reference/` directory so both Claude and Copilot load them consistently.

### Adding a Skill

Skills are **synced in from their owning library repo** by a `sync-skills.yml` GitHub Action — do not hand-edit `SKILL.md` here. Each source repo keeps its skills under `skills/<skill-name>/` and its workflow copies them to `plugins/shiny/skills/<skill-name>/` in this repo via a pull request. To add a new skill, add it to the source repo and let the workflow open the PR, then add a row to the tables above.

## License

[MIT](LICENSE)
