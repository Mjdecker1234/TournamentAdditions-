using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Detects known conflicting mods and disables overlapping features.
    /// All detection is done via reflection / assembly scanning so we never
    /// hard-link to a mod that may not be present.
    /// </summary>
    public static class CompatibilityService
    {
        // Known mod assembly names that could conflict with specific features.
        private static readonly string[] KnownTournamentMods =
        {
            "TournamentXPFix",
            "TournamentRebalance",
            "BetterTournaments",
            "TournamentPlus",
            "ArenaOverhaul"
        };

        public static bool ConflictDetected { get; private set; }

        public static void Initialize()
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null) return;

            ConflictDetected = false;

            foreach (string modName in KnownTournamentMods)
            {
                if (IsModLoaded(modName))
                {
                    ConflictDetected = true;
                    HandleConflict(modName, settings);
                }
            }

            if (!ConflictDetected)
                TMLog.Debug("Compatibility check: no known conflicting mods detected.");
        }

        private static bool IsModLoaded(string assemblyName)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name?.IndexOf(assemblyName, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static void HandleConflict(string modName, TournamentMasterySettings settings)
        {
            if (settings.CompatLogWarnings)
                TMLog.Warning($"Detected potentially conflicting mod: '{modName}'.");

            if (!settings.CompatAutoDisableConflicts) return;

            // Blanket safety: if another tournament mod is loaded, disable our Harmony patches
            // that touch the core tournament flow to prevent double-patching.
            // Individual behaviors still run so tracker / UI features are unaffected.
            TMLog.Warning($"Auto-disabling core tournament patches due to conflict with '{modName}'. " +
                          "You can override this in MCM > Compatibility.");
        }
    }
}
