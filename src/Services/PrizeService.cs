using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Manages prize generation, prize pool selection, rerolling, and post-win rewards.
    /// </summary>
    public sealed class PrizeService
    {
        private static PrizeService? _instance;
        public static PrizeService Instance => _instance ??= new PrizeService();

        // Tracks how many rerolls the player has done for the current tournament.
        private int _rerollCount;
        private Town? _currentTournamentTown;

        // The prize pool generated for the current tournament.
        private List<ItemObject> _prizePool = new();

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Generates a prize pool for the given town. Call when the player visits
        /// a tournament. Returns the selected/active prize item.
        /// </summary>
        public List<ItemObject> GeneratePrizePool(Town town)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnablePrizeCustomization)
            {
                _prizePool = new List<ItemObject>();
                return _prizePool;
            }

            if (town != _currentTournamentTown)
            {
                _rerollCount = 0;
                _currentTournamentTown = town;
            }

            _prizePool = BuildPrizePool(town, settings);
            TMLog.Debug($"Prize pool generated ({_prizePool.Count} items) for {town.Name}");
            return _prizePool;
        }

        /// <summary>
        /// Rerolls the prize pool. Returns the new pool or null if reroll is disabled/unaffordable.
        /// </summary>
        public List<ItemObject>? TryReroll(Town town, Hero player)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.PrizeRerollEnabled) return null;

            int cost = CalculateRerollCost(settings);
            if (player.Gold < cost)
            {
                TMLog.Info($"Cannot reroll: need {cost} gold, player has {player.Gold}.");
                return null;
            }

            player.ChangeHeroGold(-cost);
            _rerollCount++;
            _prizePool = BuildPrizePool(town, settings);

            if (settings.EnableNotifications && settings.NotifyPrizeReroll)
            {
                var prize = _prizePool.FirstOrDefault();
                TMLog.Info(prize is not null
                    ? $"Prize rerolled to: {prize.Name} (cost {cost} gold)"
                    : "Prize rerolled (empty pool).");
            }

            return _prizePool;
        }

        public int CalculateRerollCost(TournamentMasterySettings? settings = null)
        {
            settings ??= TournamentMasterySettings.Instance;
            if (settings is null) return 0;
            double cost = settings.PrizeRerollBaseCost * Math.Pow(settings.PrizeRerollCostMultiplier, _rerollCount);
            return (int)Math.Min(cost, 1_000_000);
        }

        /// <summary>
        /// Applies post-win rewards (gold, morale, influence) to the player.
        /// Call after the vanilla TournamentGame.OnPlayerWin.
        /// </summary>
        public void ApplyWinRewards(Hero player, Town town)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnablePrizeCustomization) return;

            if (settings.BonusGoldReward > 0)
                player.ChangeHeroGold(settings.BonusGoldReward);

            if (settings.MoraleReward > 0 && MobileParty.MainParty is { } party)
                party.RecentEventsMorale += settings.MoraleReward;

            if (settings.InfluenceReward > 0 && player.Clan is { } clan)
                clan.Influence += settings.InfluenceReward;

            // Renown multiplier is handled via patch on vanilla renown gain.

            if (settings.EnableNotifications && settings.NotifyTournamentVictory)
                TMLog.Info($"Tournament victory at {town.Name}! Gold +{settings.BonusGoldReward}, Morale +{settings.MoraleReward}.");
        }

        // ── Private helpers ──────────────────────────────────────────────

        private List<ItemObject> BuildPrizePool(Town town, TournamentMasterySettings settings)
        {
            int poolSize = settings.PrizePoolSize;
            int minValue = settings.PrizeMinValue;
            int maxValue = settings.PrizeMaxValue;

            var candidates = new List<ItemObject>();

            // Pull from town inventory first if enabled.
            if (settings.PrizeFromTownInventory && town.Owner?.ItemRoster is { } roster)
            {
                foreach (var element in roster)
                {
                    if (element.EquipmentElement.Item is { } item
                        && item.Value >= minValue
                        && item.Value <= maxValue)
                        candidates.Add(item);
                }
            }

            // Fill from global item list, applying level/renown scaling.
            int targetTierValue = GetScaledTierValue(settings);

            foreach (ItemObject item in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
            {
                if (item is null) continue;
                if (item.Value < minValue || item.Value > maxValue) continue;
                if (!IsSuitablePrizeType(item)) continue;
                candidates.Add(item);
            }

            // Sort by closeness to target value and pick poolSize items.
            return candidates
                .OrderBy(i => Math.Abs(i.Value - targetTierValue))
                .Take(poolSize)
                .ToList();
        }

        private static int GetScaledTierValue(TournamentMasterySettings settings)
        {
            int baseValue = 5000;

            if (settings.PrizeScaleWithLevel && Hero.MainHero is { } hero)
            {
                int level = hero.Level;
                baseValue += level * 200;
            }

            if (settings.PrizeScaleWithRenown && Hero.MainHero is { } h2)
            {
                float renown = h2.Clan?.Renown ?? 0f;
                baseValue += (int)(renown * 2f);
            }

            return Math.Clamp(baseValue, settings.PrizeMinValue, settings.PrizeMaxValue);
        }

        private static bool IsSuitablePrizeType(ItemObject item) =>
            item.ItemType is ItemObject.ItemTypeEnum.HeadArmor
                          or ItemObject.ItemTypeEnum.BodyArmor
                          or ItemObject.ItemTypeEnum.HandArmor
                          or ItemObject.ItemTypeEnum.LegArmor
                          or ItemObject.ItemTypeEnum.OneHandedWeapon
                          or ItemObject.ItemTypeEnum.TwoHandedWeapon
                          or ItemObject.ItemTypeEnum.Polearm
                          or ItemObject.ItemTypeEnum.Bow
                          or ItemObject.ItemTypeEnum.Crossbow
                          or ItemObject.ItemTypeEnum.Shield
                          or ItemObject.ItemTypeEnum.Horse
                          or ItemObject.ItemTypeEnum.HorseHarness;
    }
}
