using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  BETTING & ODDS
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupBetting = "Betting & Odds";

        [SettingPropertyBool(
            "Enable Betting Customization",
            RequireRestart = false,
            HintText = "Enable Tournament Mastery's betting and odds customization system.",
            Order = 0)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public bool EnableBettingCustomization { get; set; } = true;

        [SettingPropertyInteger(
            "Max Bet Amount (Gold)",
            100, 1000000,
            RequireRestart = false,
            HintText = "Maximum gold the player can bet on any single tournament.",
            Order = 1)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public int BettingMaxBet { get; set; } = 10000;

        [SettingPropertyFloatingInteger(
            "Max Odds Ratio",
            1.1f, 20f,
            "#0.0x",
            RequireRestart = false,
            HintText = "The highest multiplier the betting system can offer. Higher values allow bigger payouts.",
            Order = 2)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public float BettingMaxOddsRatio { get; set; } = 4f;

        [SettingPropertyBool(
            "Skill-Based Odds Mode",
            RequireRestart = false,
            HintText = "Calculate odds using participant combat skills and attributes instead of only vanilla-style tier ranking.",
            Order = 3)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public bool BettingSkillBasedOdds { get; set; } = true;

        [SettingPropertyBool(
            "Enable Handicap System",
            RequireRestart = false,
            HintText = "When active, the player receives better odds but the AI participants are slightly strengthened.",
            Order = 4)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public bool BettingHandicapEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger(
            "Handicap Odds Bonus",
            1f, 3f,
            "#0.0x",
            RequireRestart = false,
            HintText = "Multiplier applied to the player's odds when the handicap system is active.",
            Order = 5)]
        [SettingPropertyGroup(GroupBetting, GroupOrder = 4)]
        public float BettingHandicapOddsBonus { get; set; } = 1.5f;
    }
}
