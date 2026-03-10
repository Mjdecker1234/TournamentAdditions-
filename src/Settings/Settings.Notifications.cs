using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  NOTIFICATIONS
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupNotifications = "Notifications";

        [SettingPropertyBool(
            "Enable Notifications",
            RequireRestart = false,
            HintText = "Master toggle for all Tournament Mastery in-game notifications.",
            Order = 0)]
        [SettingPropertyGroup(GroupNotifications, GroupOrder = 6)]
        public bool EnableNotifications { get; set; } = true;

        [SettingPropertyBool(
            "Notify on New Tournament",
            RequireRestart = false,
            HintText = "Show a message-log notification whenever a new tournament is created anywhere.",
            Order = 1)]
        [SettingPropertyGroup(GroupNotifications, GroupOrder = 6)]
        public bool NotifyNewTournament { get; set; } = true;

        [SettingPropertyBool(
            "Notify on Nearby Tournament",
            RequireRestart = false,
            HintText = "Show a notification when a new tournament opens within the configured notification radius.",
            Order = 2)]
        [SettingPropertyGroup(GroupNotifications, GroupOrder = 6)]
        public bool NotifyNearbyTournament { get; set; } = true;

        [SettingPropertyBool(
            "Notify on Tournament Victory",
            RequireRestart = false,
            HintText = "Show a detailed notification (renown, gold, morale) when the player wins a tournament.",
            Order = 3)]
        [SettingPropertyGroup(GroupNotifications, GroupOrder = 6)]
        public bool NotifyTournamentVictory { get; set; } = true;

        [SettingPropertyBool(
            "Notify on Prize Reroll",
            RequireRestart = false,
            HintText = "Show a brief notification with the new prize when the player rerolls.",
            Order = 4)]
        [SettingPropertyGroup(GroupNotifications, GroupOrder = 6)]
        public bool NotifyPrizeReroll { get; set; } = true;
    }
}
