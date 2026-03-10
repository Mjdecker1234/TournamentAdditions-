using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.TournamentGames;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Patches
{
    /// <summary>
    /// Postfix patch that multiplies the renown reward from tournament victories
    /// by the MCM-configurable <see cref="TournamentMasterySettings.RenownMultiplier"/>.
    ///
    /// Target: TournamentGame.OnPlayerWin (internal method called when the player
    /// wins the final round). The exact signature may vary; the patch is wrapped in
    /// try-catch so a signature mismatch fails gracefully without crashing.
    /// </summary>
    [HarmonyPatch]
    public static class TournamentRenownPatch
    {
        [HarmonyTargetMethod]
        public static System.Reflection.MethodBase? TargetMethod()
        {
            try
            {
                // Locate the method that awards renown. Name can vary between patches.
                return AccessTools.Method(typeof(TournamentGame), "OnPlayerWin")
                    ?? AccessTools.Method(typeof(TournamentGame), "GetRenownReward");
            }
            catch (Exception ex)
            {
                TMLog.Debug($"TournamentRenownPatch.TargetMethod failed: {ex.Message}");
                return null;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref float __result)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnablePrizeCustomization) return;
            if (Math.Abs(settings.RenownMultiplier - 1f) < 0.01f) return;

            __result *= settings.RenownMultiplier;
            TMLog.Debug($"Renown scaled by {settings.RenownMultiplier:F2}x → {__result:F1}");
        }
    }
}
