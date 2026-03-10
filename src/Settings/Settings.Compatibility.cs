using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  COMPATIBILITY
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupCompatibility = "Compatibility";

        [SettingPropertyBool(
            "Auto-Disable Conflicting Features",
            RequireRestart = false,
            HintText = "When a known conflicting mod is detected, automatically disable the feature that would overlap rather than crashing.",
            Order = 0)]
        [SettingPropertyGroup(GroupCompatibility, GroupOrder = 8)]
        public bool CompatAutoDisableConflicts { get; set; } = true;

        [SettingPropertyBool(
            "Log Compatibility Warnings",
            RequireRestart = false,
            HintText = "Write compatibility diagnostic messages to the log when conflicting mods are detected.",
            Order = 1)]
        [SettingPropertyGroup(GroupCompatibility, GroupOrder = 8)]
        public bool CompatLogWarnings { get; set; } = true;

        [SettingPropertyBool(
            "Safe Mode (Disable All Patches)",
            RequireRestart = false,
            HintText = "Emergency toggle: disables every Harmony patch from TournamentMastery. Use if a crash occurs and you need to diagnose the cause. Requires game restart.",
            Order = 2)]
        [SettingPropertyGroup(GroupCompatibility, GroupOrder = 8)]
        public bool CompatSafeMode { get; set; } = false;
    }
}
