using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Patches
{
    /// <summary>
    /// Patches the TournamentCampaignBehavior to adjust tournament spawn frequency.
    /// The patch is applied only when participant rules are enabled.
    ///
    /// This targets the method responsible for determining how often tournaments
    /// can be created per settlement. We intercept the interval check and replace
    /// it with our configurable value.
    /// </summary>
    [HarmonyPatch]
    public static class TournamentFrequencyPatch
    {
        [HarmonyTargetMethod]
        public static System.Reflection.MethodBase? TargetMethod()
        {
            try
            {
                return AccessTools.Method(typeof(TournamentCampaignBehavior), "GetTournamentCreationFrequencyInDays")
                    ?? AccessTools.Method(typeof(TournamentCampaignBehavior), "OnDailyTickTown");
            }
            catch (Exception ex)
            {
                TMLog.Debug($"TournamentFrequencyPatch.TargetMethod failed: {ex.Message}");
                return null;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref int __result)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnableParticipantRules) return;
            if (settings.TournamentFrequencyDays == __result) return;

            __result = settings.TournamentFrequencyDays;
            TMLog.Debug($"Tournament frequency overridden to {__result} days.");
        }
    }
}
