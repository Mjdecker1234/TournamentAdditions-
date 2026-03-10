using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Handles betting events: pays out winnings and forfeits bets on loss.
    /// </summary>
    public sealed class TournamentBettingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
        }

        public override void SyncData(IDataStore dataStore) { }

        private static void OnTournamentFinished(
            CharacterObject winner,
            Town town,
            ItemObject? prize,
            bool isPlayerWinner)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnableBettingCustomization) return;
            if (Hero.MainHero is null) return;

            if (isPlayerWinner)
                BettingService.Instance.CollectWinnings(Hero.MainHero);
            else
                BettingService.Instance.ForfeitBet();
        }
    }
}
