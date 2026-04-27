# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for Bluetooth LE, GPS, geofencing, background jobs, push notifications, HTTP transfers, OBD-II diagnostics, music, health data, MAUI Shell navigation, contact store, shared UI controls, mediator/CQRS, document store, spatial data, DI, localization, hosting modules, and .NET Aspire integrations across .NET MAUI, Blazor, and ASP.NET Core.

## Plugins & Skills

This marketplace is organized into **plugins**, each containing one or more **skills**. Install only the plugins relevant to your project.

| Plugin | Description | Skills |
|--------|-------------|--------|
| `shiny-client` | Cross-platform mobile libraries for .NET MAUI, iOS, and Android — Bluetooth LE (client & peripheral hosting), GPS tracking, geofencing, background jobs, local & push notifications (APNs/FCM/Azure), HTTP background transfers, OBD-II vehicle diagnostics, music library access, health data (HealthKit/Health Connect), and core infrastructure with DI and lifecycle hooks | `shiny-core`, `shiny-bluetoothle`, `shiny-ble-hosting`, `shiny-firebase`, `shiny-health`, `shiny-http-transfers`, `shiny-jobs`, `shiny-locations`, `shiny-music`, `shiny-notifications`, `shiny-obd`, `shiny-push` |
| `shiny-maui` | .NET MAUI application components — Shell navigation with source-generated routes, deep linking, page/ViewModel registration, and cross-platform device contact store with CRUD, LINQ queries, and permissions | `shiny-contactstore`, `shiny-maui-shell` |
| `controls` | Shiny Controls — shared UI components for .NET MAUI and Blazor: TableView (14 cell types), FloatingPanel/OverlayHost (bottom/top sheets with detents), ChatView, ImageViewer with zoom, ImageEditor (crop/rotate/draw/text), AutoCompleteEntry, CountryPicker, AddressEntry with geocoding, PillView badges, SecurityPin, Fab/FabMenu, Scheduler/calendar views, Markdown renderer and editor, and haptic feedback | `shiny-controls` |
| `shiny-mediator` | Mediator/CQRS pattern for .NET — request/response handlers, commands, events, streams, middleware pipelines, source-generated HTTP client proxies, and project scaffolding with code generation | `shiny-mediator` |
| `shiny-data` | Lightweight AOT-compatible data libraries — DocumentDB schema-free JSON document store (SQLite, MySQL, SQL Server, PostgreSQL) with LINQ queries, and Spatial geospatial database with SQLite R*Tree indexing and custom geometry algorithms | `shiny-documentdb`, `shiny-spatial` |
| `shiny-aspire` | Shiny .NET Aspire integrations — Orleans ADO.NET hosting and Gluetun VPN container routing | `shiny-aspire` |
| `shiny-extensions` | Source generators and cross-platform utilities — attribute-driven DI registration with keyed services, persistent key/value stores (mobile/desktop/Blazor WASM), AOT-compliant reflection via source generation, strongly-typed .resx localization generator, modular MAUI hosting with IMauiModule, and modular ASP.NET Core web hosting with IWebModule | `localizegen`, `shiny-di`, `shiny-maui-hosting`, `shiny-reflector`, `shiny-stores`, `shiny-web-hosting` |

## Installation

### Claude Code CLI

**Install the entire marketplace** (all plugins):

```bash
claude mcp add-marketplace shinyorg/skills
```

**Install a single plugin:**

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

### GitHub Copilot CLI

GitHub Copilot does not have a native plugin/marketplace system, so you reference skill files directly as custom instructions.

**Option 1: VS Code settings**

Add file references to the skills you need in your VS Code `settings.json`:

```json
{
  "github.copilot.chat.codeGeneration.instructions": [
    { "file": "/path/to/skills/plugins/shiny-client/skills/shiny-bluetoothle/SKILL.md" },
    { "file": "/path/to/skills/plugins/shiny-mediator/skills/shiny-mediator/SKILL.md" }
  ]
}
```

**Option 2: Workspace instructions**

Create a `.github/copilot-instructions.md` file in your repository and paste the relevant skill content from the `plugins/<plugin>/skills/<skill>/SKILL.md` files.

## Repository Layout

```
.claude-plugin/
  marketplace.json          # Marketplace manifest — lists all plugins
plugins/
  <plugin-name>/
    .claude-plugin/
      plugin.json           # Plugin manifest — metadata, keywords, skill path
    skills/
      <skill-name>/
        SKILL.md             # Skill definition — triggers, guidance, code gen rules
        reference/           # API reference docs and additional context
```

### Key Files

- **`marketplace.json`** — Top-level manifest that registers the marketplace and points to each plugin via relative paths.
- **`plugin.json`** — Per-plugin manifest containing name, version, description, keywords, and the path to its skills directory.
- **`SKILL.md`** — The skill itself: trigger conditions, usage guidance, code generation instructions, and best practices.

## License

[MIT](LICENSE)
