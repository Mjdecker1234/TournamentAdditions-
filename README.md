# Tournament Mastery

A comprehensive, fully configurable tournament overhaul mod for **Mount & Blade II: Bannerlord**.

---

## Features

### Tournament Tracker
- Global list of all active tournaments with settlement, faction, culture, distance, and prize info
- Nearest-first sorting with configurable decimal precision
- Filters by faction, culture, distance, prize value, and settlement name
- Injected into Arena and Town menus for easy access
- Auto-notification for nearby tournaments within a configurable radius

### Equipment Standardizer
- Force all tournament participants to the same armor tier (Naked → Max)
- Per-slot overrides (head, body, arms, legs)
- Culture-aware item pools with generic fallback
- Never permanently mutates hero equipment — always restored after the tournament
- Strict fairness mode for fully deterministic loadouts

### Prize & Reward Customization
- Prize reroll with configurable base cost and scaling interest
- Prize pool selection (choose from N items)
- Optional prize from town inventory
- Prize scaling with player level and renown
- Bonus gold, renown multiplier, influence, and morale rewards

### Betting & Odds
- Configurable max bet and odds ratio
- Skill-based odds mode (uses combat attributes instead of tier ranking)
- Handicap system for better odds at the cost of tougher opponents

### Participants & Difficulty
- Allow companions, spouse, and more nobles to participate
- Configurable AI difficulty multiplier
- Configurable tournament spawn frequency
- Skill XP multiplier
- Anti-exploit diminishing returns for repeated same-settlement wins

### Player Hosting
- Host tournaments in owned towns for a configurable gold cost
- Cooldown system per town
- Optional prosperity and loyalty effects
- Invitation-style notifications

### Notifications
- Configurable notification triggers: new tournament, nearby tournament, victory, reroll

### MCM Integration
- Every feature is independently toggleable via MCM
- Sensible defaults, min/max validation, tooltips, and grouped categories
- Safe-mode emergency toggle that disables all Harmony patches

---

## Dependencies

| Dependency | Version | Required |
|---|---|---|
| Bannerlord.Harmony | ≥ 2.3.3 | Required |
| Bannerlord.ButterLib | ≥ 2.9.0 | Required |
| Bannerlord.MBOptionScreen (MCM) | ≥ 5.9.1 | Required |
| Bannerlord.UIExtenderEx | ≥ 2.9.0 | Optional |

---

## Installation

1. Install all required dependencies via NexusMods or Vortex.
2. Copy the `TournamentMastery` folder into your `Modules` directory.
3. Enable the mod in the launcher (after all dependencies).
4. Configure options in-game via the **Mod Options** (MCM) menu.

---

## Load Order

```
Native
SandBoxCore
Sandbox
CustomBattle
StoryMode
Bannerlord.Harmony
Bannerlord.ButterLib
Bannerlord.MBOptionScreen
Bannerlord.UIExtenderEx   (optional)
TournamentMastery
<other mods>
```

---

## Known Limitations

- **Path distance**: The distance calculator uses straight-line map distance. True path distance via campaign routing may differ; the strategy is isolated for future upgrade.
- **Equipment standardization**: Works at the service level; NPC character objects that are not Hero instances are handled via a Harmony patch intercept point. Some modded character types may bypass it if they override equipment assignment.
- **Team tournaments**: Not implemented in this version. The underlying Bannerlord APIs do not expose a clean team-tournament entry point without significant mission code changes.
- **Prize reroll UI**: Currently shown via inquiry dialog (no custom GauntletUI panel). A full UI panel requires UIExtenderEx XML integration and will be added in a future version.
- **Tournament frequency patch**: The target method name varies between Bannerlord e1.x versions. The patch uses reflection fallback and will no-op gracefully if it can't find the method.

---

## Compatibility

TournamentMastery scans loaded assemblies on startup. If a known conflicting tournament mod is detected, it will:
1. Log a warning.
2. Optionally auto-disable the feature group that would conflict.

Conflicts can be managed in **MCM → Compatibility**.

Known mods that may conflict with specific features:
- `TournamentXPFix` (XP gains)
- `TournamentRebalance` (frequency/odds)
- `BetterTournaments` (prize/equipment)
- `ArenaOverhaul` (equipment/participants)

---

## Building from Source

### Quick Start

1. Set the `BANNERLORD_GAME_DIR` environment variable to your Bannerlord installation path:
   ```
   # Windows (PowerShell)
   $env:BANNERLORD_GAME_DIR = "C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord"

   # Windows (Command Prompt)
   set BANNERLORD_GAME_DIR=C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord
   ```
2. Open `TournamentMastery.csproj` in Visual Studio 2022 or Rider (or run `dotnet build`).
3. Build in `Release` configuration.

### What Happens After Building

- The compiled `TournamentMastery.dll` is placed in **`Modules/TournamentMastery/bin/Win64_Shipping_Client/`** inside this repository — a complete, ready-to-transfer module folder.
- If `BANNERLORD_GAME_DIR` is set, the mod is **automatically deployed** to your Bannerlord installation. Just enable it in the launcher.
- If `BANNERLORD_GAME_DIR` is not set, simply copy the **`Modules/TournamentMastery/`** folder from this repository into your Bannerlord `Modules/` directory.

---

## Contributing

Pull requests welcome. Please keep feature additions modular and MCM-gated.
