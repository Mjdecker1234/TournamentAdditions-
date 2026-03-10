using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  HOSTING & SPAWNING
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupHosting = "Hosting & Spawning";

        [SettingPropertyBool(
            "Enable Player Hosting",
            RequireRestart = false,
            HintText = "Allow the player to pay gold to host a tournament in any town they own.",
            Order = 0)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public bool EnablePlayerHosting { get; set; } = true;

        [SettingPropertyInteger(
            "Hosting Cost (Gold)",
            0, 100000,
            RequireRestart = false,
            HintText = "Gold cost to host a tournament in a player-owned town.",
            Order = 1)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public int HostingCost { get; set; } = 5000;

        [SettingPropertyInteger(
            "Hosting Cooldown (Days)",
            1, 180,
            RequireRestart = false,
            HintText = "Minimum days before the same town can host another player-sponsored tournament.",
            Order = 2)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public int HostingCooldownDays { get; set; } = 30;

        [SettingPropertyBool(
            "Enable Invitation Notifications",
            RequireRestart = false,
            HintText = "Send an in-game invitation-style notification from nearby lords when a hosted tournament begins.",
            Order = 3)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public bool HostingInvitationNotifications { get; set; } = true;

        [SettingPropertyBool(
            "Settlement Prosperity Effect",
            RequireRestart = false,
            HintText = "Hosting a tournament gives a small prosperity boost to the host settlement.",
            Order = 4)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public bool HostingProsperityEffect { get; set; } = true;

        [SettingPropertyFloatingInteger(
            "Prosperity Bonus",
            0f, 100f,
            "#0.0",
            RequireRestart = false,
            HintText = "Amount of prosperity added to the host settlement when a player-hosted tournament is started.",
            Order = 5)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public float HostingProsperityBonus { get; set; } = 10f;

        [SettingPropertyBool(
            "Settlement Loyalty Effect",
            RequireRestart = false,
            HintText = "Hosting a tournament gives a small loyalty boost to the host settlement.",
            Order = 6)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public bool HostingLoyaltyEffect { get; set; } = false;

        [SettingPropertyFloatingInteger(
            "Loyalty Bonus",
            0f, 50f,
            "#0.0",
            RequireRestart = false,
            HintText = "Loyalty points added to the host settlement when a tournament starts.",
            Order = 7)]
        [SettingPropertyGroup(GroupHosting, GroupOrder = 7)]
        public float HostingLoyaltyBonus { get; set; } = 5f;
    }
}
