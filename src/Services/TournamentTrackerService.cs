using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Entry object returned by the tracker for each active tournament.
    /// Immutable data-class—safe to enumerate from UI without locking.
    /// </summary>
    public sealed class TournamentEntry
    {
        public Settlement Settlement { get; }
        public TournamentGame Tournament { get; }
        public float Distance { get; }
        public string DistanceFormatted { get; }
        public ItemObject? PrizeItem { get; }
        public string PrizeName { get; }
        public int PrizeValue { get; }
        public string FactionName { get; }
        public string CultureName { get; }

        public TournamentEntry(Settlement s, TournamentGame t, float dist, int decimals)
        {
            Settlement = s;
            Tournament = t;
            Distance = dist;
            DistanceFormatted = dist >= float.MaxValue ? "N/A" : dist.ToString($"F{Math.Clamp(decimals, 0, 3)}");
            PrizeItem = TournamentCache.GetPrizeItem(t);
            PrizeName = PrizeItem?.Name?.ToString() ?? "Unknown";
            PrizeValue = PrizeItem?.Value ?? 0;
            FactionName = s.OwnerClan?.Kingdom?.Name?.ToString()
                       ?? s.OwnerClan?.Name?.ToString()
                       ?? "Independent";
            CultureName = s.Culture?.Name?.ToString() ?? "Unknown";
        }
    }

    /// <summary>
    /// Singleton service that builds and caches the sorted list of active tournament entries.
    /// Call <see cref="Refresh"/> on a campaign tick; read <see cref="Entries"/> from UI.
    /// </summary>
    public sealed class TournamentTrackerService
    {
        private static TournamentTrackerService? _instance;
        public static TournamentTrackerService Instance => _instance ??= new TournamentTrackerService();

        private List<TournamentEntry> _entries = new();
        private IReadOnlyList<TournamentEntry> _filtered = Array.Empty<TournamentEntry>();

        private string _filterFaction = string.Empty;
        private string _filterCulture = string.Empty;
        private string _filterName = string.Empty;
        private float _filterMaxDistance = float.MaxValue;
        private int _filterMinPrize;
        private int _filterMaxPrize = int.MaxValue;

        public IReadOnlyList<TournamentEntry> Entries => _filtered;

        public void Reset()
        {
            _entries.Clear();
            _filtered = Array.Empty<TournamentEntry>();
            _instance = null;
            TournamentCache.Invalidate();
        }

        /// <summary>Rebuilds the tournament list from the campaign state.</summary>
        public void Refresh()
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnableTracker)
            {
                _entries.Clear();
                _filtered = Array.Empty<TournamentEntry>();
                return;
            }

            TournamentCache.Invalidate();
            int decimals = settings.TrackerDistanceDecimals;

            var rawList = TournamentCache.GetActiveTournaments();
            _entries = rawList
                .Select(pair => new TournamentEntry(
                    pair.settlement,
                    pair.tournament,
                    DistanceCalculator.GetDistance(pair.settlement),
                    decimals))
                .ToList();

            ApplyFilters(settings);
        }

        public void SetFilters(
            string faction = "",
            string culture = "",
            string name = "",
            float maxDistance = float.MaxValue,
            int minPrize = 0,
            int maxPrize = int.MaxValue)
        {
            _filterFaction = faction ?? string.Empty;
            _filterCulture = culture ?? string.Empty;
            _filterName = name ?? string.Empty;
            _filterMaxDistance = maxDistance;
            _filterMinPrize = minPrize;
            _filterMaxPrize = maxPrize;

            var settings = TournamentMasterySettings.Instance;
            if (settings is not null) ApplyFilters(settings);
        }

        private void ApplyFilters(TournamentMasterySettings settings)
        {
            IEnumerable<TournamentEntry> q = _entries;

            if (!string.IsNullOrWhiteSpace(_filterFaction))
                q = q.Where(e => e.FactionName.IndexOf(_filterFaction, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrWhiteSpace(_filterCulture))
                q = q.Where(e => e.CultureName.IndexOf(_filterCulture, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrWhiteSpace(_filterName))
                q = q.Where(e => e.Settlement.Name.ToString().IndexOf(_filterName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (_filterMaxDistance < float.MaxValue)
                q = q.Where(e => e.Distance <= _filterMaxDistance);

            if (_filterMinPrize > 0)
                q = q.Where(e => e.PrizeValue >= _filterMinPrize);

            if (_filterMaxPrize < int.MaxValue)
                q = q.Where(e => e.PrizeValue <= _filterMaxPrize);

            q = settings.TrackerSortAscending
                ? q.OrderBy(e => e.Distance)
                : q.OrderByDescending(e => e.Distance);

            _filtered = q.Take(settings.TrackerMaxResults).ToList();
        }
    }
}
