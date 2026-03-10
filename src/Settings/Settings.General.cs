using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  GENERAL
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupGeneral = "General";

        [SettingPropertyBool(
            "Enable Tournament Mastery",
            RequireRestart = false,
            HintText = "Master toggle. Disabling this turns off all TournamentMastery features without uninstalling the mod.",
            Order = 0)]
        [SettingPropertyGroup(GroupGeneral, GroupOrder = 0)]
        public bool EnableMod { get; set; } = true;

        [SettingPropertyBool(
            "Debug Logging",
            RequireRestart = false,
            HintText = "Write verbose debug output to the Bannerlord log file. Disable in production runs for performance.",
            Order = 1)]
        [SettingPropertyGroup(GroupGeneral, GroupOrder = 0)]
        public bool DebugLogging { get; set; } = false;

        [SettingPropertyBool(
            "Show Startup Notification",
            RequireRestart = false,
            HintText = "Show a brief in-game notification when Tournament Mastery is active on campaign start.",
            Order = 2)]
        [SettingPropertyGroup(GroupGeneral, GroupOrder = 0)]
        public bool ShowStartupNotification { get; set; } = true;
    }
}
