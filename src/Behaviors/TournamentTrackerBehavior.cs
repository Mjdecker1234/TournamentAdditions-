using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TournamentMastery.Services;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Behaviors
{
    /// <summary>
    /// Campaign behavior for the Tournament Tracker.
    /// - Refreshes the tracker cache on each daily tick.
    /// - Injects menu entries into arena and town menus.
    /// - Notifies the player of new nearby tournaments.
    /// </summary>
    public sealed class TournamentTrackerBehavior : CampaignBehaviorBase
    {
        private readonly HashSet<string> _knownTournaments = new();

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // No persistent data required; tracker rebuilds from campaign state.
        }

        // ── Event handlers ───────────────────────────────────────────────

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            InjectMenuEntries(starter);
            RefreshTracker();
        }

        private void OnGameLoaded(CampaignGameStarter starter)
        {
            InjectMenuEntries(starter);
            RefreshTracker();
        }

        private void OnDailyTick()
        {
            RefreshTracker();
            CheckForNewNearbyTournaments();
        }

        // ── Tracker refresh ──────────────────────────────────────────────

        private void RefreshTracker()
        {
            try
            {
                TournamentTrackerService.Instance.Refresh();
            }
            catch (Exception ex)
            {
                TMLog.Exception(ex, "TournamentTrackerBehavior.RefreshTracker");
            }
        }

        private void CheckForNewNearbyTournaments()
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableMod || !settings.EnableTracker
                || !settings.TrackerAutoNotify || !settings.EnableNotifications)
                return;

            foreach (var entry in TournamentTrackerService.Instance.Entries)
            {
                string id = entry.Settlement.StringId;
                if (_knownTournaments.Contains(id)) continue;

                _knownTournaments.Add(id);

                if (entry.Distance <= settings.TrackerNotificationRadius)
                {
                    if (settings.NotifyNearbyTournament)
                        TMLog.Info($"A tournament is happening nearby at {entry.Settlement.Name} ({entry.DistanceFormatted} away)!");
                }
                else if (settings.NotifyNewTournament)
                {
                    TMLog.Info($"A new tournament has started at {entry.Settlement.Name}.");
                }
            }

            // Prune settlements whose tournaments have ended.
            _knownTournaments.RemoveWhere(id =>
            {
                Settlement? settlement = Settlement.All?.FirstOrDefault(s => s.StringId == id);
                return settlement is null || !TournamentCache.HasActiveTournament(settlement);
            });
        }

        // ── Menu injection ───────────────────────────────────────────────

        private static void InjectMenuEntries(CampaignGameStarter starter)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null) return;

            if (settings.TrackerInjectTownMenu)
            {
                starter.AddGameMenuOption(
                    "town",
                    "tm_tracker_town",
                    "{=TM_tracker_town}View All Tournaments",
                    args =>
                    {
                        var s = TournamentMasterySettings.Instance;
                        args.optionLeaveType = GameMenuOption.LeaveType.Default;
                        return s is not null && s.EnableMod && s.EnableTracker;
                    },
                    _ => OpenTrackerUI(),
                    false,
                    4);
            }

            if (settings.TrackerInjectArenaMenu)
            {
                starter.AddGameMenuOption(
                    "arena",
                    "tm_tracker_arena",
                    "{=TM_tracker_arena}View All Tournaments",
                    args =>
                    {
                        var s = TournamentMasterySettings.Instance;
                        args.optionLeaveType = GameMenuOption.LeaveType.Default;
                        return s is not null && s.EnableMod && s.EnableTracker;
                    },
                    _ => OpenTrackerUI(),
                    false,
                    3);
            }
        }

        private static void OpenTrackerUI()
        {
            try
            {
                TournamentTrackerService.Instance.Refresh();
                var vm = new UI.ViewModels.TournamentTrackerViewModel();
                // Show as a generic inquiry panel using GauntletUI.
                // Full UIExtenderEx screen injection would require XML UI files;
                // we use the inquiry approach for maximum compatibility.
                var entries = TournamentTrackerService.Instance.Entries;
                var lines = new StringBuilder();
                lines.AppendLine($"Active Tournaments ({entries.Count}):\n");

                var s = TournamentMasterySettings.Instance;
                int decimals = s?.TrackerDistanceDecimals ?? 1;

                foreach (var entry in entries)
                {
                    lines.Append($"• {entry.Settlement.Name}");
                    if (s?.TrackerShowFactionCulture == true)
                        lines.Append($" [{entry.FactionName} / {entry.CultureName}]");
                    lines.Append($" — {entry.DistanceFormatted} away");
                    if (s?.TrackerShowPrize == true && entry.PrizeItem is not null)
                        lines.Append($" — Prize: {entry.PrizeName} ({entry.PrizeValue}g)");
                    lines.AppendLine();
                }

                InformationManager.ShowInquiry(new InquiryData(
                    "Tournament Tracker",
                    lines.ToString(),
                    true,
                    false,
                    "Close",
                    string.Empty,
                    null,
                    null));
            }
            catch (Exception ex)
            {
                TMLog.Exception(ex, "OpenTrackerUI");
            }
        }
    }
}
