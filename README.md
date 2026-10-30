# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for Bluetooth LE (client, peripheral & hubs), beacons, GPS, geofencing, background jobs, push notifications, Live Activities, HTTP transfers, embedded HTTP server, Wi-Fi & local network discovery, screen recording, printing, game controllers, Game Center/Play Games, in-app purchases, app store checks, Siri/App Intents & Android AppFunctions, wearables, OBD-II diagnostics, music, speech recognition and synthesis, AI conversation, health data, MAUI Shell navigation, web apps hosted inside MAUI, contact & calendar stores, shared UI controls, mediator/CQRS, virtual actors, document store, DI, localization, ASP.NET Core extensions, and .NET Aspire integrations across .NET MAUI, Blazor, and ASP.NET Core.

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
| `shiny-ble-hubs` | SignalR-style hubs over Bluetooth LE — generated client proxies, groups, streaming, L2CAP file transfers and media channels (live audio/video) |
| `shiny-universal-hubs` | BLE hubs also served over Wi-Fi (HttpServer + mDNS) with Wi-Fi-first connect, BLE fallback, and media channels over WebSockets |
| `shiny-beacons` | iBeacon & Eddystone ranging, background region monitoring, broadcasting |
| `shiny-jobs` | Background job scheduling — native iOS/Android schedulers plus in-process jobs |
| `shiny-locations` | GPS tracking, geofencing, motion activity recognition |
| `shiny-notifications` | Local notification scheduling, channels, badges, interactive actions |
| `shiny-push` | Push notifications — native FCM/APNs and Azure Notification Hubs |
| `shiny-liveactivities` | iOS Live Activities (Lock Screen, Dynamic Island) and Android 16 Live Updates |
| `shiny-firebase` | Firebase Cloud Messaging for iOS and Android |
| `shiny-http-transfers` | Background uploads and downloads with progress tracking, Live Activity / foreground-service progress |
| `shiny-data-sync` | Bidirectional JSON sync over HTTP with background outbox/inbox |
| `shiny-httpserver` | Embedded HTTP/1.1, HTTP/2 & HTTP/3 server where ASP.NET Core cannot run — reverse proxy, gRPC, WebDAV, MCP, tunnelling |
| `shiny-wifi` | Wi-Fi scanning, connect/disconnect, saved networks, current network, hotspot |
| `shiny-discovery` | Local network discovery — mDNS/DNS-SD, SSDP/UPnP, WS-Discovery |
| `shiny-obd` | OBD-II commands, adapter auto-detection, BLE/WiFi/serial transports |
| `shiny-music` | Music library permissions, querying, playback, lyrics, album art |
| `shiny-health` | HealthKit / Health Connect queries, writes, real-time observers |
| `shiny-contactstore` | Contact CRUD, fluent async query builder, permissions |
| `shiny-calendarstore` | Calendar & event CRUD, fluent async query builder, permissions |
| `shiny-gamepad` | Game controller input — sticks, triggers, buttons, rumble, battery, motion, light bars |
| `shiny-gamecenter` | Achievements, leaderboards and friends over Apple Game Center and Google Play Games |
| `shiny-inapppurchases` | In-app purchases (StoreKit 2 / Play Billing) and server-side purchase verification |
| `shiny-appstore` | Store version checks, store/review deep links, native in-app review prompt |
| `shiny-appfunctions` | Expose app functions to Siri, Spotlight, Shortcuts, Apple Intelligence and Android AppFunctions |
| `shiny-wearables` | Companion Apple Watch / Wear OS messaging, shared context, data and file transfers |
| `shiny-printing` | Thermal/receipt printing (ESC/POS, TSPL) over BLE/WiFi/web, plus OS-native printing |
| `shiny-screenrecorder` | Screen recording to MP4 with mic/system audio, display/window selection, pause/resume |
| `shiny-speech` | Speech-to-text, text-to-speech, audio capture and playback, echo-cancelled full-duplex call audio |
| `shiny-aiconversation` | Chat client, wake word, STT/TTS, persistent message store |
| `shiny-actors` | Orleans-style virtual actors anywhere .NET runs — state, event sourcing, streams, reminders, remoting |

### MAUI

| Skill | What It Helps With |
|---|---|
| `shiny-maui-shell` | Pages, ViewModels, navigation, source-generated routes, guards, deep links, app shortcuts, typed dialogs |
| `shiny-appdevicebridge` | Host OTA-updated web apps (Blazor WASM, React, Vue) inside MAUI with typed C#/TypeScript device bridges |

### Controls — shared .NET MAUI and Blazor UI

| Skill | What It Helps With |
|---|---|
| `shiny-controls` | TableView, TreeView, FloatingPanel, ChatView, ImageViewer/ImageEditor, CameraView (with live H.264/JPEG video), MediaElement, DataGrid, Wizard, Walkthrough, Kanban, Gantt, Diagram, Spreadsheet, Document Editor, Ribbon, Flyout, TabbedPage/NavigationPage, scheduler, Markdown, barcodes, motion icons, and more |

### Mediator

| Skill | What It Helps With |
|---|---|
| `shiny-mediator` | Handlers, contracts, middleware, HTTP extension, OpenAPI generation |

### Data

| Skill | What It Helps With |
|---|---|
| `shiny-documentdb` | Schema-free JSON document store — queries, joins, CRUD, indexes & unique indexes, spatial/geo queries, AOT configuration |
| `shiny-firestore-mobile` | On-device native Firestore provider with offline persistence and snapshot listeners |

### ASP.NET Core

| Skill | What It Helps With |
|---|---|
| `shiny-web-hosting` | Modular ASP.NET Core configuration with `IWebModule` |
| `shiny-aspnet` | WebDAV, CalDAV/CardDAV, tus uploads, NuGet/npm feeds, DocumentDb endpoints, webhooks, idempotency, ACME TLS, tunnels |

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
- **`SKILL.md`** — The skill itself: YAML front matter with the shared Agent Skills fields `name` and `description`, followed by usage guidance, code generation instructions, and best practices. Copilot selects skills using their names and descriptions; custom fields such as `auto_invoke`, `triggers`, `when_to_use`, and `references` are not portable discovery requirements. Retain existing custom metadata for consumers that use it, but include essential invocation cues in `description` and reference supporting docs from the Markdown body using paths relative to the skill directory.

### Claude and Copilot Compatibility

The same skill files serve both clients; Claude Code remains the primary use case. Follow the [Agent Skills format](https://agentskills.io/specification) and [Copilot skill guidance](https://docs.github.com/en/copilot/how-tos/copilot-cli/customize-copilot/add-skills):

- Match each skill's `name` to its directory name (lowercase letters, digits, and hyphens; at most 64 characters).
- Keep `description` non-empty and at most **1,024 characters**, describing both the capability and when to invoke it. Use a YAML folded scalar (`description: >-`) for descriptions containing punctuation such as `: `.
- Keep detailed API scope, examples, constraints, and Claude-friendly instructions in the Markdown body or linked supporting docs, not in an oversized description. Existing topic files may stay beside `SKILL.md` or in `reference/`; neither client requires moving them.
- Avoid client-specific tool names or absolute installation paths in skill instructions.

Check local Copilot discovery without installing or updating the plugin:

```bash
copilot --plugin-dir ./plugins/shiny skill list --json
claude plugin validate ./plugins/shiny
```

Copilot should list every skill from the local plugin with no local skill-loading errors. Other installed plugins may contribute additional skills or unrelated diagnostics.

### Adding a Skill

Skills are **synced in from their owning library repo** by a `sync-skills.yml` GitHub Action — do not hand-edit `SKILL.md` here. Each source repo keeps its skills under `skills/<skill-name>/` and its workflow copies them to `plugins/shiny/skills/<skill-name>/` in this repo via a pull request. To add a new skill, add it to the source repo and let the workflow open the PR, then add a row to the tables above.

Apply compatibility fixes in the owning source repo as well; otherwise a later sync can overwrite them.

## License

[MIT](LICENSE)
