using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Adds a "Host Tournament" option to the town castle/keep menu for towns the player owns.
    /// </summary>
    public sealed class TournamentHostingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        }

        public override void SyncData(IDataStore dataStore) { }

        private static void OnNewGameCreated(CampaignGameStarter starter) => InjectMenuEntries(starter);
        private static void OnGameLoaded(CampaignGameStarter starter) => InjectMenuEntries(starter);

        private static void InjectMenuEntries(CampaignGameStarter starter)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnablePlayerHosting) return;

            starter.AddGameMenuOption(
                "town_keep",
                "tm_host_tournament",
                "{=TM_host_tournament}Host a Tournament",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Default;
                    var s = TournamentMasterySettings.Instance;
                    if (s is null || !s.EnableMod || !s.EnablePlayerHosting) return false;
                    Town? town = Settlement.CurrentSettlement?.Town;
                    if (town is null) return false;
                    bool canHost = TournamentHostingService.Instance.CanHost(town, Hero.MainHero!, out _);
                    return canHost;
                },
                _ => OnHostTournamentSelected(),
                false,
                5);
        }

        private static void OnHostTournamentSelected()
        {
            Town? town = Settlement.CurrentSettlement?.Town;
            if (town is null || Hero.MainHero is null) return;

            var settings = TournamentMasterySettings.Instance;
            if (settings is null) return;

            InformationManager.ShowInquiry(new InquiryData(
                "Host Tournament",
                $"Host a tournament at {town.Name} for {settings.HostingCost} gold?\n" +
                $"Cooldown: {settings.HostingCooldownDays} days.\n" +
                (settings.HostingProsperityEffect ? $"Prosperity +{settings.HostingProsperityBonus:F0}\n" : string.Empty) +
                (settings.HostingLoyaltyEffect ? $"Loyalty +{settings.HostingLoyaltyBonus:F0}\n" : string.Empty),
                true,
                true,
                "Confirm",
                "Cancel",
                () => TournamentHostingService.Instance.HostTournament(town, Hero.MainHero!),
                null));
        }
    }
}
