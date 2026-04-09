# Shiny AI Skills

AI skills for [Shiny Libraries](https://shinylib.net) — providing rich context and code generation guidance for mediator, DI, stores, hosting, MAUI Shell, notifications, push, BLE, locations, jobs, HTTP transfers, spatial, and more across .NET MAUI, Blazor, and ASP.NET Core.

## Available Skills

| Skill | Description |
|-------|-------------|
| `shiny-core` | Core infrastructure, hosting, DI, key-value stores, lifecycle hooks |
| `shiny-mediator` | Mediator / CQRS pattern with middleware and source generators |
| `shiny-di` | Dependency injection patterns and registration |
| `shiny-stores` | Persistent key-value and object stores |
| `shiny-maui-shell` | MAUI Shell navigation, tabs, flyout, and routing |
| `shiny-maui-hosting` | MAUI hosting model and module infrastructure |
| `shiny-maui-scheduler` | MAUI background scheduling |
| `shiny-notifications` | Local notifications |
| `shiny-push` | Push notification providers (Firebase, Azure, etc.) |
| `shiny-bluetoothle` | Bluetooth Low Energy client |
| `shiny-ble-hosting` | Bluetooth Low Energy GATT server hosting |
| `shiny-locations` | GPS, geofencing, and motion activity |
| `shiny-jobs` | Background jobs and periodic tasks |
| `shiny-http-transfers` | Background HTTP uploads and downloads |
| `shiny-spatial` | Geospatial utilities, geofencing, R-tree |
| `shiny-aspire` | .NET Aspire integration with Orleans |
| `shiny-web-hosting` | ASP.NET Core web hosting modules |
| `shiny-reflector` | Reflection utilities, property access, serialization |
| `shiny-contactstore` | Device contact store access |
| `shiny-obd` | OBD-II vehicle diagnostics over Bluetooth |
| `shiny-tableview` | MAUI settings/table view pages |
| `shiny-documentdb` | SQLite-backed JSON document store |
| `shiny-firebase` | Firebase integration |
| `shiny-music` | Music library and media playback |
| `localizegen` | Localization source generator from .resx files |

## Installation

### Claude Code

Install the Shiny skills as a Claude Code plugin from the marketplace:

```bash
claude plugin add shinyorg/skills
```

Or add it manually by including the following in your project's `.claude/settings.json`:

```json
{
  "plugins": ["shinyorg/skills"]
}
```

### GitHub Copilot

To use the Shiny skills with GitHub Copilot, you can reference the skill files as custom instructions.

**Option 1: Repository-level instructions**

Create a `.github/copilot-instructions.md` file in your repository and reference the skill content you need. You can copy the relevant skill content from the `skills/` directory in this repo into your instructions file:

```markdown
<!-- .github/copilot-instructions.md -->
<!-- Paste the content from the relevant SKILL.md files below -->
```

**Option 2: Use as a Copilot Extension (VS Code)**

1. Clone or download this repository
2. In VS Code, open Settings and search for `github.copilot.chat.codeGeneration.instructions`
3. Add file references to the skills you need:

```json
{
  "github.copilot.chat.codeGeneration.instructions": [
    {
      "file": "/path/to/skills/shiny-core/SKILL.md"
    },
    {
      "file": "/path/to/skills/shiny-mediator/SKILL.md"
    }
  ]
}
```

**Option 3: Workspace-level instructions (`.github/copilot-instructions.md`)**

For team-wide consistency, add a `.github/copilot-instructions.md` to your project repository with the Shiny skill content relevant to your project. This file is automatically picked up by Copilot for all contributors.

## Skill Structure

Each skill lives in `skills/<skill-name>/` and contains:

- `SKILL.md` — The main skill file with triggers, usage guidance, code generation instructions, and best practices
- `reference/` — API reference docs and additional context files

## License

[MIT](LICENSE)
