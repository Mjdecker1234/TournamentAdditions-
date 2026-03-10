using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace TournamentMastery.Settings
{
    /// <summary>
    /// Root MCM settings for Tournament Mastery. All settings are grouped into categories.
    /// Serialized automatically by MCM. Safe to add fields between saves.
    /// </summary>
    public sealed partial class TournamentMasterySettings : AttributeGlobalSettings<TournamentMasterySettings>
    {
        public override string Id => "TournamentMastery_v1";
        public override string DisplayName => "Tournament Mastery";
        public override string FolderName => "TournamentMastery";
        public override string Format => "json";

        // Singleton shortcut (MCM provides Instance via AttributeGlobalSettings)
    }
}
