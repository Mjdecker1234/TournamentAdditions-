using TaleWorlds.CampaignSystem;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Provides startup notification when TournamentMastery first activates.
    /// Extends easily to support future notification types.
    /// </summary>
    public sealed class TournamentNotificationBehavior : CampaignBehaviorBase
    {
        private bool _startupDone;

        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("TM_StartupDone", ref _startupDone);
        }

        private void OnNewGameCreated(CampaignGameStarter _) => ShowStartupIfNeeded();
        private void OnGameLoaded(CampaignGameStarter _) => ShowStartupIfNeeded();

        private void ShowStartupIfNeeded()
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.ShowStartupNotification) return;
            if (_startupDone) return;

            _startupDone = true;
            TMLog.Info("Tournament Mastery is active. Open MCM to configure features.");
        }
    }
}
