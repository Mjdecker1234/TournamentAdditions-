using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the Tournament Tracker panel.
    /// Exposes bindable properties for use with GauntletUI data binding.
    /// Refreshes the underlying service and maps entries to bindable row items.
    /// </summary>
    public sealed class TournamentTrackerViewModel : ViewModel
    {
        private MBBindingList<TournamentEntryItemVM> _entries = new();
        private string _title = "Tournament Tracker";
        private string _filterFaction = string.Empty;
        private string _filterCulture = string.Empty;
        private string _filterName = string.Empty;
        private bool _sortAscending;
        private int _totalCount;

        public TournamentTrackerViewModel()
        {
            RefreshEntries();
        }

        [DataSourceProperty]
        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }

        [DataSourceProperty]
        public MBBindingList<TournamentEntryItemVM> Entries
        {
            get => _entries;
            set { if (_entries != value) { _entries = value; OnPropertyChanged(); } }
        }

        [DataSourceProperty]
        public string FilterFaction
        {
            get => _filterFaction;
            set { if (_filterFaction != value) { _filterFaction = value; OnPropertyChanged(); ApplyFilters(); } }
        }

        [DataSourceProperty]
        public string FilterCulture
        {
            get => _filterCulture;
            set { if (_filterCulture != value) { _filterCulture = value; OnPropertyChanged(); ApplyFilters(); } }
        }

        [DataSourceProperty]
        public string FilterName
        {
            get => _filterName;
            set { if (_filterName != value) { _filterName = value; OnPropertyChanged(); ApplyFilters(); } }
        }

        [DataSourceProperty]
        public bool SortAscending
        {
            get => _sortAscending;
            set { if (_sortAscending != value) { _sortAscending = value; OnPropertyChanged(); ApplyFilters(); } }
        }

        [DataSourceProperty]
        public int TotalCount
        {
            get => _totalCount;
            set { if (_totalCount != value) { _totalCount = value; OnPropertyChanged(); } }
        }

        public void Refresh()
        {
            TournamentTrackerService.Instance.Refresh();
            RefreshEntries();
        }

        private void ApplyFilters()
        {
            TournamentTrackerService.Instance.SetFilters(
                faction: _filterFaction,
                culture: _filterCulture,
                name: _filterName);
            RefreshEntries();
        }

        private void RefreshEntries()
        {
            var settings = TournamentMasterySettings.Instance;
            var source = TournamentTrackerService.Instance.Entries;

            _entries.Clear();
            foreach (var entry in source)
                _entries.Add(new TournamentEntryItemVM(entry, settings));

            TotalCount = _entries.Count;
            Title = $"Tournament Tracker ({TotalCount} active)";
        }
    }

    /// <summary>
    /// Row item ViewModel for a single tournament entry.
    /// </summary>
    public sealed class TournamentEntryItemVM : ViewModel
    {
        public string SettlementName { get; }
        public string Distance { get; }
        public string Faction { get; }
        public string Culture { get; }
        public string Prize { get; }
        public int PrizeValue { get; }
        public bool ShowFaction { get; }
        public bool ShowPrize { get; }

        public TournamentEntryItemVM(TournamentEntry entry, TournamentMasterySettings? settings)
        {
            SettlementName = entry.Settlement.Name?.ToString() ?? "?";
            Distance = entry.DistanceFormatted;
            Faction = entry.FactionName;
            Culture = entry.CultureName;
            Prize = entry.PrizeName;
            PrizeValue = entry.PrizeValue;
            ShowFaction = settings?.TrackerShowFactionCulture ?? true;
            ShowPrize = settings?.TrackerShowPrize ?? true;
        }
    }
}
