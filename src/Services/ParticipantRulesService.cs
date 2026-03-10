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
    /// Controls which characters are eligible to participate in tournaments
    /// and adjusts tournament-related AI difficulty.
    /// </summary>
    public sealed class ParticipantRulesService
    {
        private static ParticipantRulesService? _instance;
        public static ParticipantRulesService Instance => _instance ??= new ParticipantRulesService();

        // Anti-exploit: track recent wins per settlement.
        private readonly Dictionary<string, (int wins, CampaignTime lastWin)> _settlementWins = new();

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Returns the list of CharacterObjects that should participate in the tournament.
        /// Extends the base native list with heroes if settings permit.
        /// </summary>
        public List<CharacterObject> BuildParticipantList(
            List<CharacterObject> nativeParticipants,
            Town hostTown)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableParticipantRules)
                return nativeParticipants;

            var result = new List<CharacterObject>(nativeParticipants);
            var settlement = hostTown.Settlement;

            TryAddHeroParticipants(result, settlement, settings);

            return result;
        }

        /// <summary>
        /// Returns the AI attribute override multiplier for tournament participants.
        /// 1.0 = vanilla difficulty.
        /// </summary>
        public float GetAIDifficultyMultiplier()
        {
            var settings = TournamentMasterySettings.Instance;
            return settings?.ParticipantsAIDifficulty ?? 1f;
        }

        /// <summary>
        /// Records a win at the given town. Returns the diminishing XP/reward factor
        /// if anti-exploit is enabled (1.0 = full, 0.1 = near-minimum).
        /// </summary>
        public float RecordWin(Town town)
        {
            var settings = TournamentMasterySettings.Instance;
            string key = town.StringId;

            if (settings is null || !settings.AntiExploitSafeguards)
            {
                if (_settlementWins.TryGetValue(key, out var existing))
                    _settlementWins[key] = (existing.wins + 1, CampaignTime.Now);
                else
                    _settlementWins[key] = (1, CampaignTime.Now);
                return 1f;
            }

            if (!_settlementWins.TryGetValue(key, out var record))
            {
                _settlementWins[key] = (1, CampaignTime.Now);
                return 1f;
            }

            // Decay: wins more than 30 days ago reset the counter.
            if ((CampaignTime.Now - record.lastWin).ToDays > 30)
            {
                _settlementWins[key] = (1, CampaignTime.Now);
                return 1f;
            }

            int newCount = record.wins + 1;
            _settlementWins[key] = (newCount, CampaignTime.Now);

            // Diminishing returns: factor = 1 / (1 + excessWins * 0.5)
            float factor = 1f / (1f + Math.Max(0, newCount - 1) * 0.5f);
            TMLog.Debug($"Anti-exploit factor at {town.Name}: {factor:F2} (wins: {newCount})");
            return Math.Max(0.1f, factor);
        }

        // ── Private helpers ──────────────────────────────────────────────

        private static void TryAddHeroParticipants(
            List<CharacterObject> list,
            Settlement settlement,
            TournamentMasterySettings settings)
        {
            int slotsAvailable = Math.Max(0, 16 - list.Count); // vanilla max is typically 16
            int added = 0;

            // Companions
            if (settings.ParticipantsAllowCompanions && added < slotsAvailable)
            {
                foreach (TroopRosterElement element in MobileParty.MainParty?.MemberRoster?.GetTroopRoster()
                             ?? Enumerable.Empty<TroopRosterElement>())
                {
                    if (added >= slotsAvailable) break;
                    CharacterObject troop = element.Character;
                    if (troop?.IsHero != true || troop.IsPlayerCharacter) continue;
                    Hero companion = troop.HeroObject;
                    if (companion.IsWanderer && !list.Contains(troop))
                    {
                        list.Add(troop);
                        added++;
                    }
                }
            }

            // Spouse
            if (settings.ParticipantsAllowSpouse && added < slotsAvailable
                && Hero.MainHero?.Spouse is { } spouse
                && spouse.IsAlive
                && spouse.CurrentSettlement == settlement
                && !list.Contains(spouse.CharacterObject))
            {
                list.Add(spouse.CharacterObject);
                added++;
            }

            // Extra nobles present in the settlement
            if (settings.ParticipantsMoreNobles && added < slotsAvailable)
            {
                foreach (Hero notable in settlement.HeroesWithoutParty)
                {
                    if (added >= slotsAvailable) break;
                    if (!notable.IsActive || notable.IsNotable) continue;
                    if (!list.Contains(notable.CharacterObject))
                    {
                        list.Add(notable.CharacterObject);
                        added++;
                    }
                }
            }
        }
    }
}
