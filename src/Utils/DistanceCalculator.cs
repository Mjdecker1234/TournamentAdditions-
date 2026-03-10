using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Settlements;

namespace TournamentMastery.Utils
{
    /// <summary>
    /// Provides campaign-map distance calculations between the player and settlements.
    /// Strategy pattern: tries the most accurate available method first, falls back
    /// gracefully so the strategy can be swapped without touching call-sites.
    /// </summary>
    public static class DistanceCalculator
    {
        private enum DistanceStrategy { PathDistance, StraightLine }

        // Start with straight-line which is always available; upgrade if needed.
        private static DistanceStrategy _strategy = DistanceStrategy.StraightLine;

        /// <summary>
        /// Returns the numeric campaign-map distance from the player's current position
        /// to the given settlement.  Always returns a finite non-negative float.
        /// </summary>
        public static float GetDistance(Settlement settlement)
        {
            if (settlement is null) return float.MaxValue;

            try
            {
                return _strategy switch
                {
                    DistanceStrategy.PathDistance => GetPathDistance(settlement),
                    _ => GetStraightLineDistance(settlement)
                };
            }
            catch (Exception ex)
            {
                TMLog.Debug($"DistanceCalculator fallback triggered: {ex.Message}");
                _strategy = DistanceStrategy.StraightLine;
                return GetStraightLineDistance(settlement);
            }
        }

        /// <summary>
        /// Returns the distance formatted to the configured decimal precision.
        /// </summary>
        public static string GetFormattedDistance(Settlement settlement, int decimalPlaces)
        {
            float dist = GetDistance(settlement);
            return dist >= float.MaxValue
                ? "N/A"
                : dist.ToString($"F{Math.Clamp(decimalPlaces, 0, 3)}");
        }

        // ── Strategy implementations ─────────────────────────────────────

        private static float GetStraightLineDistance(Settlement settlement)
        {
            IMapPoint playerPos = GetPlayerPosition();
            if (playerPos is null) return float.MaxValue;
            return settlement.Position2D.Distance(playerPos.Position2D);
        }

        private static float GetPathDistance(Settlement settlement)
        {
            // Campaign.GetPathDistanceBetweenSettlements may not be available on all
            // versions; we keep this isolated so it can be upgraded when the API is
            // confirmed stable for the target version.
            return GetStraightLineDistance(settlement);
        }

        private static IMapPoint? GetPlayerPosition()
        {
            if (MobileParty.MainParty is { } party)
                return party;

            if (Hero.MainHero?.CurrentSettlement is { } s)
                return s;

            return null;
        }
    }
}
