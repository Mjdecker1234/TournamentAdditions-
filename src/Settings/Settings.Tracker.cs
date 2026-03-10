using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  TOURNAMENT TRACKER
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupTracker = "Tournament Tracker";

        [SettingPropertyBool(
            "Enable Tracker",
            RequireRestart = false,
            HintText = "Show the Tournament Tracker panel listing all active tournaments with distance, faction, culture, and prize info.",
            Order = 0)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool EnableTracker { get; set; } = true;

        [SettingPropertyInteger(
            "Max Results Shown",
            1, 100,
            RequireRestart = false,
            HintText = "Maximum number of tournament entries displayed in the tracker list.",
            Order = 1)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public int TrackerMaxResults { get; set; } = 30;

        [SettingPropertyBool(
            "Sort Ascending (Nearest First)",
            RequireRestart = false,
            HintText = "When enabled, tournaments are sorted nearest-first. Disable to sort farthest-first.",
            Order = 2)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerSortAscending { get; set; } = true;

        [SettingPropertyBool(
            "Show Only Active Tournaments",
            RequireRestart = false,
            HintText = "Only list settlements that currently have an active, joinable tournament.",
            Order = 3)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerOnlyActive { get; set; } = true;

        [SettingPropertyBool(
            "Show Prize Info",
            RequireRestart = false,
            HintText = "Display prize item name and estimated value in the tracker list.",
            Order = 4)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerShowPrize { get; set; } = true;

        [SettingPropertyBool(
            "Show Faction & Culture",
            RequireRestart = false,
            HintText = "Display the owning faction and culture next to each tournament settlement.",
            Order = 5)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerShowFactionCulture { get; set; } = true;

        [SettingPropertyInteger(
            "Distance Decimal Places",
            0, 3,
            RequireRestart = false,
            HintText = "Number of decimal places to show for tournament distance values (0–3).",
            Order = 6)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public int TrackerDistanceDecimals { get; set; } = 1;

        [SettingPropertyBool(
            "Show Tracker in Arena Menu",
            RequireRestart = false,
            HintText = "Inject a 'View All Tournaments' option into the arena practise menu.",
            Order = 7)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerInjectArenaMenu { get; set; } = true;

        [SettingPropertyBool(
            "Show Tracker in Town Menu",
            RequireRestart = false,
            HintText = "Inject a 'View All Tournaments' option into the town visit menu.",
            Order = 8)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerInjectTownMenu { get; set; } = true;

        [SettingPropertyBool(
            "Auto-Notify Nearby Tournaments",
            RequireRestart = false,
            HintText = "Display an in-game notification when a new tournament opens within the notification radius.",
            Order = 9)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerAutoNotify { get; set; } = true;

        [SettingPropertyFloatingInteger(
            "Notification Radius",
            10f, 500f,
            "#0 units",
            RequireRestart = false,
            HintText = "Campaign-map units. Tournaments within this radius trigger a nearby-tournament notification.",
            Order = 10)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public float TrackerNotificationRadius { get; set; } = 150f;

        [SettingPropertyBool(
            "Track Only Attendable Tournaments",
            RequireRestart = false,
            HintText = "Only show tournaments the player can reasonably travel to given their current campaign situation.",
            Order = 11)]
        [SettingPropertyGroup(GroupTracker, GroupOrder = 1)]
        public bool TrackerOnlyAttendable { get; set; } = false;
    }
}
