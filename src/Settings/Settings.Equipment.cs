using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Dropdown;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  TOURNAMENT EQUIPMENT
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupEquipment = "Tournament Equipment";

        [SettingPropertyBool(
            "Enable Equipment Standardizer",
            RequireRestart = false,
            HintText = "Force all tournament participants to use standardized armor so fights are balanced by skill, not gear.",
            Order = 0)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EnableEquipmentStandardizer { get; set; } = false;

        [SettingPropertyDropdown(
            "Armor Tier",
            RequireRestart = false,
            HintText = "The armor quality tier assigned to every participant: Naked, Peasant, Low, Mid, High, Elite, or Max.",
            Order = 1)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public DefaultDropdown<string> EquipmentArmorTier { get; set; } =
            new DefaultDropdown<string>(
                new[] { "Naked", "Peasant", "Low", "Mid", "High", "Elite", "Max" },
                selectedIndex: 3);

        [SettingPropertyBool(
            "Preserve Weapons",
            RequireRestart = false,
            HintText = "Keep the native tournament weapon assignments and only override armor slots.",
            Order = 2)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentPreserveWeapons { get; set; } = true;

        [SettingPropertyBool(
            "Preserve Shields",
            RequireRestart = false,
            HintText = "Keep shields from native tournament loadouts when standardizing equipment.",
            Order = 3)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentPreserveShields { get; set; } = true;

        [SettingPropertyBool(
            "Preserve Horses",
            RequireRestart = false,
            HintText = "Keep horse mounts from native tournament loadouts. Disable to remove all horses from all rounds.",
            Order = 4)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentPreserveHorses { get; set; } = true;

        [SettingPropertyBool(
            "Use Culture-Based Equipment Pools",
            RequireRestart = false,
            HintText = "Draw armor from culture-appropriate item pools when available. Falls back to generic pools if the culture pool is incomplete.",
            Order = 5)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentUseCulturePools { get; set; } = true;

        [SettingPropertyBool(
            "Standardize Head Armor",
            RequireRestart = false,
            HintText = "Apply the selected tier override to the head armor slot.",
            Order = 6)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentStandardizeHead { get; set; } = true;

        [SettingPropertyBool(
            "Standardize Body Armor",
            RequireRestart = false,
            HintText = "Apply the selected tier override to the body armor slot.",
            Order = 7)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentStandardizeBody { get; set; } = true;

        [SettingPropertyBool(
            "Standardize Arm Armor",
            RequireRestart = false,
            HintText = "Apply the selected tier override to the arm/glove armor slot.",
            Order = 8)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentStandardizeArms { get; set; } = true;

        [SettingPropertyBool(
            "Standardize Leg Armor",
            RequireRestart = false,
            HintText = "Apply the selected tier override to the leg armor slot.",
            Order = 9)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentStandardizeLegs { get; set; } = true;

        [SettingPropertyBool(
            "Allow Player Override",
            RequireRestart = false,
            HintText = "Allow the player to use a different personal armor tier while standardization is active for NPCs.",
            Order = 10)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentAllowPlayerOverride { get; set; } = false;

        [SettingPropertyBool(
            "Strict Fairness Mode",
            RequireRestart = false,
            HintText = "Enforces identical armor stats for every slot across all participants, ignoring culture variation.",
            Order = 11)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentStrictFairness { get; set; } = false;

        [SettingPropertyBool(
            "Equipment Debug Logging",
            RequireRestart = false,
            HintText = "Log per-participant equipment changes to the debug log.",
            Order = 12)]
        [SettingPropertyGroup(GroupEquipment, GroupOrder = 2)]
        public bool EquipmentDebugLog { get; set; } = false;
    }
}
