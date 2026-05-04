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

### Claude Code

**Install the entire marketplace** (all plugins at once):

```bash
claude mcp add-marketplace shinyorg/skills
```

**Install a single plugin** (only the skills you need):

```bash
claude plugin add shinyorg/skills/shiny-client
claude plugin add shinyorg/skills/shiny-mediator
```

You can also add plugins manually in your project's `.claude/settings.json`:

```json
{
  "plugins": [
    "shinyorg/skills/shiny-client",
    "shinyorg/skills/shiny-mediator"
  ]
}
```

Once installed, Claude Code automatically activates skills when it detects matching trigger keywords in your conversation (e.g. mentioning "BluetoothLE" or "IMediator" will load the relevant skill context).

### GitHub Copilot

Each skill in this repository uses a `SKILL.md` file with YAML front matter (`name`, `description`) and a `## Triggers` section, which is compatible with GitHub Copilot Agent Skills. There are two ways to use these skills with Copilot:

**Option 1 — Copy skill directories into your project**

Copy the skill folders you need into a Copilot skills location such as `.github/skills/` or `.agents/skills/`. Keep each folder name the same as the `name` field in its `SKILL.md`.

```
your-project/
  .github/
    skills/
      shiny-bluetoothle/
        SKILL.md
        reference/
          ...
      shiny-mediator/
        SKILL.md
        reference/
          ...
```

Copilot will discover them automatically and use the trigger keywords and instructions when relevant.

**Option 2 — Point Copilot at this repository**

If you don't want to copy files, configure `chat.agentSkillsLocations` in your VS Code settings to reference one or more plugin skill directories from this repository:

```json
{
  "chat.agentSkillsLocations": [
    "/path/to/shinyorg/skills/plugins/shiny-client/skills",
    "/path/to/shinyorg/skills/plugins/shiny-mediator/skills"
  ]
}
```

This makes all skills within the referenced plugin directories available to Copilot without duplicating any files.

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
