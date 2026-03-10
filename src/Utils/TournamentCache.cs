using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TournamentMastery.Utils
{
    /// <summary>
    /// Cached read access to campaign tournaments. Invalidated each campaign tick
    /// by the TournamentTrackerBehavior. Centralises null-safe querying of the
    /// TournamentManager so the rest of the codebase never repeats these checks.
    /// </summary>
    public static class TournamentCache
    {
        private static List<(Settlement settlement, TournamentGame tournament)>? _cache;

        public static void Invalidate() => _cache = null;

        /// <summary>Returns all settlements that currently have an active tournament.</summary>
        public static IReadOnlyList<(Settlement settlement, TournamentGame tournament)> GetActiveTournaments()
        {
            if (_cache is not null) return _cache;

            _cache = new List<(Settlement, TournamentGame)>();

            foreach (Town town in Town.AllTowns)
            {
                if (town?.Settlement is not { } settlement) continue;

                TournamentGame? tournament = Campaign.Current?.TournamentManager
                    ?.GetTournamentGame(town);

                if (tournament is not null)
                    _cache.Add((settlement, tournament));
            }

            return _cache;
        }

        /// <summary>Returns true if the given settlement currently has an active tournament.</summary>
        public static bool HasActiveTournament(Settlement settlement)
        {
            if (settlement?.Town is null) return false;
            return Campaign.Current?.TournamentManager?.GetTournamentGame(settlement.Town) is not null;
        }

        /// <summary>Safe accessor for a tournament's prize item; may return null.</summary>
        public static ItemObject? GetPrizeItem(TournamentGame tournament)
        {
            try { return tournament?.Prize; }
            catch { return null; }
        }
    }
}
