# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for mediator, DI, stores, hosting, MAUI Shell, notifications, push, BLE, locations, jobs, HTTP transfers, spatial, and more across .NET MAUI, Blazor, and ASP.NET Core.

## Plugins & Skills

This marketplace is organized into **plugins**, each containing one or more **skills**. Install only the plugins relevant to your project.

| Plugin | Description | Skills |
|--------|-------------|--------|
| `shiny-client` | Cross-platform mobile framework — BLE, GPS, geofencing, jobs, notifications, push, HTTP transfers, OBD, music, and more | `shiny-core`, `shiny-bluetoothle`, `shiny-ble-hosting`, `shiny-firebase`, `shiny-http-transfers`, `shiny-jobs`, `shiny-locations`, `shiny-music`, `shiny-notifications`, `shiny-obd`, `shiny-push` |
| `shiny-maui` | Purpose-built .NET MAUI components — Shell navigation with source-generated routes and contact store access | `shiny-contactstore`, `shiny-maui-shell` |
| `controls` | Shiny Controls — a shared UI control surface for .NET MAUI and Blazor (TableView, BottomSheet, Pill, ImageViewer, SecurityPin, Fab/FabMenu, Scheduler, Markdown) | `shiny-controls` |
| `shiny-mediator` | Mediator/CQRS pattern for .NET — request/response, commands, events, streams, middleware, HTTP client generation | `shiny-mediator` |
| `shiny-data` | Lightweight AOT-compatible data libraries — DocumentDB JSON document store and Spatial geospatial R*Tree | `shiny-documentdb`, `shiny-spatial` |
| `shiny-aspire` | .NET Aspire integrations — Orleans ADO.NET hosting and Gluetun VPN container routing | `shiny-aspire` |
| `shiny-extensions` | Source generators and utilities — DI, key/value stores, reflection, localization, hosting modules | `localizegen`, `shiny-di`, `shiny-maui-hosting`, `shiny-reflector`, `shiny-stores`, `shiny-web-hosting` |

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
