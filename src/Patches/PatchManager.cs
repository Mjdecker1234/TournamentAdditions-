using System;
using HarmonyLib;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Patches
{
    /// <summary>
    /// Central patch registry. Applies Harmony patches selectively based on MCM settings.
    /// Each patch class is registered individually so a disabled feature never patches.
    /// </summary>
    public static class PatchManager
    {
        public static void ApplyAll(Harmony harmony)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || settings.CompatSafeMode)
            {
                TMLog.Info("Harmony patches skipped (mod disabled or safe mode active).");
                return;
            }

            ApplyPatch(harmony, typeof(TournamentRenownPatch),    settings.EnablePrizeCustomization);
            ApplyPatch(harmony, typeof(TournamentFrequencyPatch), settings.EnableParticipantRules);
        }

        private static void ApplyPatch(Harmony harmony, Type patchType, bool condition)
        {
            if (!condition)
            {
                TMLog.Debug($"Skipping patch {patchType.Name} (feature disabled).");
                return;
            }

            try
            {
                harmony.CreateClassProcessor(patchType).Patch();
                TMLog.Debug($"Applied patch: {patchType.Name}");
            }
            catch (Exception ex)
            {
                TMLog.Exception(ex, $"Applying patch {patchType.Name}");
            }
        }
    }
}
