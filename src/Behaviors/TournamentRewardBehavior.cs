using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Hooks into the tournament reward flow to apply bonus prizes, rerolls, and scaled rewards.
    /// </summary>
    public sealed class TournamentRewardBehavior : CampaignBehaviorBase
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
            if (settings is null || !settings.EnableMod || !settings.EnablePrizeCustomization) return;
            if (!isPlayerWinner || Hero.MainHero is null) return;

            PrizeService.Instance.ApplyWinRewards(Hero.MainHero, town);
            float antiExploit = ParticipantRulesService.Instance.RecordWin(town);

            if (antiExploit < 1f)
                TMLog.Debug($"Anti-exploit applied: reward factor {antiExploit:F2}.");

            // Betting payout is handled separately via BettingBehavior.
        }
    }
}
