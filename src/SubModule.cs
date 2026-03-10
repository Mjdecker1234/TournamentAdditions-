using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TournamentMastery.Behaviors;
using TournamentMastery.Patches;
using TournamentMastery.Services;
using TournamentMastery.Utils;

namespace TournamentMastery
{
    public sealed class SubModule : MBSubModuleBase
    {
        private static Harmony? _harmony;
        private bool _initialized;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            _harmony = new Harmony("com.tournamentmastery");
            TMLog.Info("TournamentMastery SubModule loaded.");
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_initialized) return;
            _initialized = true;

            CompatibilityService.Initialize();
            PatchManager.ApplyAll(_harmony!);
            TMLog.Info("TournamentMastery patches applied.");
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (gameStarter is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new TournamentTrackerBehavior());
                campaignStarter.AddBehavior(new TournamentEquipmentBehavior());
                campaignStarter.AddBehavior(new TournamentRewardBehavior());
                campaignStarter.AddBehavior(new TournamentBettingBehavior());
                campaignStarter.AddBehavior(new TournamentHostingBehavior());
                campaignStarter.AddBehavior(new TournamentNotificationBehavior());
                TMLog.Info("TournamentMastery campaign behaviors registered.");
            }
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            TournamentTrackerService.Instance.Reset();
        }
    }
}
