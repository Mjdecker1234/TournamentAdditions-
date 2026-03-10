using TaleWorlds.CampaignSystem;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Coordinates equipment standardization events.
    /// Listens for tournament start/end events and delegates to the standardizer service.
    /// </summary>
    public sealed class TournamentEquipmentBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TournamentStarted.AddNonSerializedListener(this, OnTournamentStarted);
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
        }

        public override void SyncData(IDataStore dataStore) { }

        private static void OnTournamentStarted(Town town)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnableEquipmentStandardizer) return;

            TMLog.Debug($"Equipment standardizer: tournament started at {town?.Name}.");
            // Actual participant standardization is applied via EquipmentNormalizePatch
            // because we need to intercept the equipment assignment point.
        }

        private static void OnTournamentFinished(
            CharacterObject winner,
            Town town,
            ItemObject? prize,
            bool isPlayerWinner)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod) return;

            if (settings.EnableEquipmentStandardizer)
            {
                TMLog.Debug($"Restoring equipment after tournament at {town?.Name}.");
                EquipmentStandardizerService.Instance.RestoreAll();
            }
        }
    }
}
