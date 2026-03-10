using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Calculates tournament betting odds and manages the player's current bet.
    /// Supports vanilla-style odds (based on participant tier ranking) and a
    /// skill-based mode that weights combat attributes.
    /// </summary>
    public sealed class BettingService
    {
        private static BettingService? _instance;
        public static BettingService Instance => _instance ??= new BettingService();

        private int _currentBet;
        private float _currentOdds;

        public int CurrentBet => _currentBet;
        public float CurrentOdds => _currentOdds;

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Calculates odds for the player in the current tournament.
        /// Returns a multiplier (e.g. 2.5 means 2.5:1 payout).
        /// </summary>
        public float CalculateOdds(IList<CharacterObject> participants)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableBettingCustomization)
                return 1f;

            float rawOdds = settings.BettingSkillBasedOdds
                ? CalculateSkillBasedOdds(participants)
                : CalculateVanillaStyleOdds(participants);

            if (settings.BettingHandicapEnabled)
                rawOdds *= settings.BettingHandicapOddsBonus;

            _currentOdds = Math.Clamp(rawOdds, 1f, settings.BettingMaxOddsRatio);
            TMLog.Debug($"Tournament odds for player: {_currentOdds:F2}x");
            return _currentOdds;
        }

        /// <summary>
        /// Places a bet. Returns false if amount exceeds limits or player funds.
        /// </summary>
        public bool TryPlaceBet(int amount, Hero player)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null) return false;

            if (amount > settings.BettingMaxBet)
            {
                TMLog.Info($"Bet exceeds max ({settings.BettingMaxBet} gold). Clamping.");
                amount = settings.BettingMaxBet;
            }

            if (amount > player.Gold)
            {
                TMLog.Info("Insufficient gold to place this bet.");
                return false;
            }

            player.ChangeHeroGold(-amount);
            _currentBet = amount;
            return true;
        }

        /// <summary>
        /// Pays out a winning bet. Call when the player wins the tournament.
        /// Returns gold paid out (0 if no bet was placed).
        /// </summary>
        public int CollectWinnings(Hero player)
        {
            if (_currentBet <= 0) return 0;
            int payout = (int)(_currentBet * _currentOdds);
            player.ChangeHeroGold(payout);
            TMLog.Info($"Betting payout: {payout} gold ({_currentOdds:F2}x on {_currentBet} bet).");
            int profit = payout - _currentBet;
            _currentBet = 0;
            return profit;
        }

        /// <summary>Returns the current bet, forfeit it (player lost/withdrew).</summary>
        public void ForfeitBet() => _currentBet = 0;

        // ── Odds strategies ──────────────────────────────────────────────

        private static float CalculateSkillBasedOdds(IList<CharacterObject> participants)
        {
            if (participants is null || participants.Count == 0) return 1f;

            CharacterObject? player = participants.FirstOrDefault(p => p?.IsPlayerCharacter == true);
            if (player is null) return 1f;

            float playerSkill = GetCombatScore(player);
            float avgOpponentSkill = participants
                .Where(p => !p.IsPlayerCharacter)
                .Select(GetCombatScore)
                .DefaultIfEmpty(1f)
                .Average();

            if (playerSkill <= 0f || avgOpponentSkill <= 0f) return 1f;

            float ratio = avgOpponentSkill / playerSkill;
            return Math.Max(1f, ratio);
        }

        private static float CalculateVanillaStyleOdds(IList<CharacterObject> participants)
        {
            if (participants is null || participants.Count == 0) return 1f;

            CharacterObject? player = participants.FirstOrDefault(p => p?.IsPlayerCharacter == true);
            if (player is null) return 1f;

            // Use participant tier rank: higher average tier → better odds.
            int totalTier = participants.Where(p => !p.IsPlayerCharacter).Sum(p => p.Tier);
            int count = participants.Count - 1;
            if (count <= 0) return 1f;

            float avgTier = (float)totalTier / count;
            float playerTier = player.Tier;
            return Math.Max(1f, avgTier / Math.Max(1f, playerTier));
        }

        private static float GetCombatScore(CharacterObject c)
        {
            // Sum relevant combat skills as a proxy for overall fighting ability.
            if (c?.IsHero != true) return c?.Level ?? 1f;

            Hero h = c.HeroObject;
            return h.GetSkillValue(DefaultSkills.OneHanded)
                 + h.GetSkillValue(DefaultSkills.TwoHanded)
                 + h.GetSkillValue(DefaultSkills.Polearm)
                 + h.GetSkillValue(DefaultSkills.Athletics)
                 + h.GetSkillValue(DefaultSkills.Riding)
                 + (h.CharacterObject.GetPerkValue(DefaultPerks.OneHanded.ShieldBearer) ? 20 : 0);
        }
    }
}
