using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Allows the player to host tournaments in towns they own.
    /// Tracks cooldowns and applies settlement effects.
    /// </summary>
    public sealed class TournamentHostingService
    {
        private static TournamentHostingService? _instance;
        public static TournamentHostingService Instance => _instance ??= new TournamentHostingService();

        // townStringId → last host time
        private readonly Dictionary<string, CampaignTime> _cooldowns = new();

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Returns true if the player can host a tournament in the given town.
        /// Checks ownership, cooldown, gold, and existing tournaments.
        /// </summary>
        public bool CanHost(Town town, Hero player, out string reason)
        {
            reason = string.Empty;
            var settings = TournamentMasterySettings.Instance;

            if (settings is null || !settings.EnablePlayerHosting)
            {
                reason = "Player hosting is disabled in MCM settings.";
                return false;
            }

            if (town.OwnerClan != player.Clan)
            {
                reason = "You do not own this town.";
                return false;
            }

            if (TournamentCache.HasActiveTournament(town.Settlement))
            {
                reason = "A tournament is already active in this town.";
                return false;
            }

            if (IsOnCooldown(town))
            {
                float daysLeft = GetCooldownDaysRemaining(town);
                reason = $"Hosting cooldown: {daysLeft:F0} days remaining.";
                return false;
            }

            if (player.Gold < settings.HostingCost)
            {
                reason = $"Insufficient gold. Need {settings.HostingCost}, have {player.Gold}.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Hosts a tournament: deducts gold, starts a tournament, applies settlement effects.
        /// Returns true on success.
        /// </summary>
        public bool HostTournament(Town town, Hero player)
        {
            if (!CanHost(town, player, out string reason))
            {
                TMLog.Warning($"Cannot host tournament at {town.Name}: {reason}");
                return false;
            }

            var settings = TournamentMasterySettings.Instance!;

            player.ChangeHeroGold(-settings.HostingCost);
            Campaign.Current.TournamentManager.AddTournament(town);
            _cooldowns[town.StringId] = CampaignTime.Now;

            ApplySettlementEffects(town, settings);

            if (settings.EnableNotifications && settings.HostingInvitationNotifications)
                TMLog.Info($"A tournament has been hosted at {town.Name}! The word spreads across the realm.");

            TournamentCache.Invalidate();
            TournamentTrackerService.Instance.Refresh();
            return true;
        }

        // ── Private helpers ──────────────────────────────────────────────

        private bool IsOnCooldown(Town town)
        {
            if (!_cooldowns.TryGetValue(town.StringId, out var lastHost)) return false;
            var settings = TournamentMasterySettings.Instance;
            float cooldownDays = settings?.HostingCooldownDays ?? 30f;
            return (CampaignTime.Now - lastHost).ToDays < cooldownDays;
        }

        private float GetCooldownDaysRemaining(Town town)
        {
            if (!_cooldowns.TryGetValue(town.StringId, out var lastHost)) return 0f;
            var settings = TournamentMasterySettings.Instance;
            float cooldownDays = settings?.HostingCooldownDays ?? 30f;
            return (float)(cooldownDays - (CampaignTime.Now - lastHost).ToDays);
        }

        private static void ApplySettlementEffects(Town town, TournamentMasterySettings settings)
        {
            if (settings.HostingProsperityEffect)
            {
                town.Prosperity += settings.HostingProsperityBonus;
                TMLog.Debug($"Prosperity +{settings.HostingProsperityBonus} at {town.Name}");
            }

            if (settings.HostingLoyaltyEffect)
            {
                town.Loyalty += settings.HostingLoyaltyBonus;
                TMLog.Debug($"Loyalty +{settings.HostingLoyaltyBonus} at {town.Name}");
            }
        }
    }
}
