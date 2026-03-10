# Changelog

All notable changes to Tournament Mastery will be documented here.

## [1.0.0] - 2026-03-10

### Added
- **Tournament Tracker** — global list of active tournaments with real distance, faction, culture, and prize info
  - Nearest-first sorting (configurable ascending/descending)
  - Filters: faction, culture, name, distance range, prize value range
  - Auto-notify for new/nearby tournaments
  - Injected into Arena menu and Town menu
- **Equipment Standardizer** — set a uniform armor tier for all tournament participants
  - Tier range: Naked, Peasant, Low, Mid, High, Elite, Max
  - Per-slot control (head, body, arms, legs)
  - Culture-aware item pools with generic fallback
  - Strict fairness mode
  - Safe equipment backup/restore (no save corruption)
- **Prize & Reward Customization**
  - Prize reroll with base cost and scaling interest
  - Prize pool selection (choose from configurable pool size)
  - Optional prize from town inventory
  - Prize value scaling with player level and renown
  - Bonus gold, renown multiplier, influence, morale rewards
- **Betting & Odds System**
  - Configurable max bet and max odds ratio
  - Skill-based odds mode
  - Handicap option
- **Participant Rules**
  - Allow companions, spouse, and extra nobles in tournaments
  - AI difficulty multiplier
  - Configurable tournament frequency (days)
  - Skill XP multiplier
  - Anti-exploit diminishing returns
- **Player Tournament Hosting**
  - Host tournaments in owned towns
  - Configurable cost and cooldown
  - Optional prosperity/loyalty effects
- **Notification System**
  - Configurable triggers: new tournament, nearby tournament, victory, reroll
- **Full MCM Integration**
  - 10 organized categories with tooltips and sensible defaults
  - Master enable toggle and safe-mode emergency patch disabler
- **Compatibility Layer**
  - Assembly scanning for known conflicting mods
  - Auto-disable conflicting features
  - Per-feature independent disable

### Architecture
- Modular service/behavior/patch structure
- DistanceCalculator with swappable strategy
- TournamentCache for deduplication
- TMLog / TMLocalization utilities
- PatchManager for feature-gated Harmony application
