using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;

namespace TournamentMastery.Settings
{
    // ──────────────────────────────────────────────────────────────────────
    //  PARTICIPANTS & DIFFICULTY
    // ──────────────────────────────────────────────────────────────────────
    public sealed partial class TournamentMasterySettings
    {
        private const string GroupParticipants = "Participants & Difficulty";

        [SettingPropertyBool(
            "Enable Participant Rules",
            RequireRestart = false,
            HintText = "Enable custom participant composition rules (nobles, companions, heroes etc.).",
            Order = 0)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool EnableParticipantRules { get; set; } = true;

        [SettingPropertyBool(
            "Allow More Nobles / Lords",
            RequireRestart = false,
            HintText = "Increase the probability that lords and nobles participate in tournaments.",
            Order = 1)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool ParticipantsMoreNobles { get; set; } = true;

        [SettingPropertyBool(
            "Allow Companions to Participate",
            RequireRestart = false,
            HintText = "Allow player companions to join tournaments when eligible.",
            Order = 2)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool ParticipantsAllowCompanions { get; set; } = true;

        [SettingPropertyBool(
            "Allow Spouse to Participate",
            RequireRestart = false,
            HintText = "Allow the player's spouse to join tournaments when both are present at the same settlement.",
            Order = 3)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool ParticipantsAllowSpouse { get; set; } = true;

        [SettingPropertyFloatingInteger(
            "AI Difficulty Multiplier",
            0.5f, 3f,
            "#0.0x",
            RequireRestart = false,
            HintText = "Multiplier applied to tournament AI combat attributes. 1.0 = vanilla difficulty.",
            Order = 4)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public float ParticipantsAIDifficulty { get; set; } = 1f;

        [SettingPropertyInteger(
            "Tournament Frequency (Days)",
            1, 60,
            RequireRestart = false,
            HintText = "Minimum days between tournaments spawning in the same settlement.",
            Order = 5)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public int TournamentFrequencyDays { get; set; } = 14;

        [SettingPropertyFloatingInteger(
            "Skill XP Multiplier",
            0.1f, 5f,
            "#0.0x",
            RequireRestart = false,
            HintText = "Multiplier for skill XP gained through tournament participation.",
            Order = 6)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public float SkillXpMultiplier { get; set; } = 1f;

        [SettingPropertyBool(
            "Allow Trait Gains",
            RequireRestart = false,
            HintText = "Winning/losing tournaments can affect character traits (valor, generosity, etc.).",
            Order = 7)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool ParticipantsTraitGains { get; set; } = false;

        [SettingPropertyBool(
            "Anti-Exploit Safeguards",
            RequireRestart = false,
            HintText = "Prevent reward or XP farming by adding diminishing returns on repeated same-settlement tournament wins.",
            Order = 8)]
        [SettingPropertyGroup(GroupParticipants, GroupOrder = 5)]
        public bool AntiExploitSafeguards { get; set; } = true;
    }
}
