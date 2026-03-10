using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  REWARDS & PRIZES
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupRewards = "Rewards & Prizes";

        [SettingPropertyBool(
            "Enable Prize Customization",
            RequireRestart = false,
            HintText = "Master toggle for the prize/reward customization system.",
            Order = 0)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool EnablePrizeCustomization { get; set; } = true;

        [SettingPropertyBool(
            "Allow Prize Reroll",
            RequireRestart = false,
            HintText = "Players can reroll the prize before joining a tournament, at a configurable gold cost.",
            Order = 1)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool PrizeRerollEnabled { get; set; } = true;

        [SettingPropertyInteger(
            "Reroll Base Cost (Gold)",
            0, 10000,
            RequireRestart = false,
            HintText = "Base gold cost for the first prize reroll.",
            Order = 2)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int PrizeRerollBaseCost { get; set; } = 500;

        [SettingPropertyFloatingInteger(
            "Reroll Cost Multiplier",
            1f, 5f,
            "#0.0x",
            RequireRestart = false,
            HintText = "Each subsequent reroll costs this multiple of the previous reroll cost (scaling interest).",
            Order = 3)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public float PrizeRerollCostMultiplier { get; set; } = 1.5f;

        [SettingPropertyBool(
            "Choose Prize From Pool",
            RequireRestart = false,
            HintText = "Let the player pick from a generated prize pool instead of seeing only one prize.",
            Order = 4)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool PrizeChooseFromPool { get; set; } = true;

        [SettingPropertyInteger(
            "Prize Pool Size",
            2, 10,
            RequireRestart = false,
            HintText = "How many prizes to show in the prize selection pool.",
            Order = 5)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int PrizePoolSize { get; set; } = 4;

        [SettingPropertyBool(
            "Allow Prize From Town Inventory",
            RequireRestart = false,
            HintText = "Prize pool can include items from the host town's inventory, creating more variety.",
            Order = 6)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool PrizeFromTownInventory { get; set; } = false;

        [SettingPropertyInteger(
            "Prize Min Value (Gold)",
            0, 100000,
            RequireRestart = false,
            HintText = "Minimum allowed prize item value in gold. Items below this threshold are excluded from prize pools.",
            Order = 7)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int PrizeMinValue { get; set; } = 1000;

        [SettingPropertyInteger(
            "Prize Max Value (Gold)",
            1000, 500000,
            RequireRestart = false,
            HintText = "Maximum allowed prize item value. Set to 500000 to disable the cap.",
            Order = 8)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int PrizeMaxValue { get; set; } = 100000;

        [SettingPropertyBool(
            "Scale Prize With Player Level",
            RequireRestart = false,
            HintText = "Adjust prize tier based on the player character's level so prizes feel relevant.",
            Order = 9)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool PrizeScaleWithLevel { get; set; } = true;

        [SettingPropertyBool(
            "Scale Prize With Renown",
            RequireRestart = false,
            HintText = "Higher player renown increases the chance of better-quality prizes.",
            Order = 10)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public bool PrizeScaleWithRenown { get; set; } = true;

        [SettingPropertyInteger(
            "Bonus Gold Reward",
            0, 50000,
            RequireRestart = false,
            HintText = "Additional gold awarded to the tournament winner on top of the vanilla gold prize.",
            Order = 11)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int BonusGoldReward { get; set; } = 0;

        [SettingPropertyFloatingInteger(
            "Renown Multiplier",
            0.1f, 5f,
            "#0.0x",
            RequireRestart = false,
            HintText = "Multiplier applied to the renown gained from winning a tournament.",
            Order = 12)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public float RenownMultiplier { get; set; } = 1f;

        [SettingPropertyInteger(
            "Influence Reward",
            0, 500,
            RequireRestart = false,
            HintText = "Influence awarded to the player for winning a tournament (0 = vanilla behaviour).",
            Order = 13)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int InfluenceReward { get; set; } = 0;

        [SettingPropertyInteger(
            "Morale Reward",
            0, 50,
            RequireRestart = false,
            HintText = "Party morale bonus for winning a tournament.",
            Order = 14)]
        [SettingPropertyGroup(GroupRewards, GroupOrder = 3)]
        public int MoraleReward { get; set; } = 5;
    }
}
