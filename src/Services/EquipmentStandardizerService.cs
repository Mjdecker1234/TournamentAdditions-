using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using MCM.Abstractions.Dropdown;
using TournamentMastery.Settings;
using TournamentMastery.Utils;

namespace TournamentMastery.Services
{
    /// <summary>
    /// Armor tier enum. Ordered from weakest to strongest.
    /// </summary>
    public enum ArmorTier
    {
        Naked = 0,
        Peasant = 1,
        Low = 2,
        Mid = 3,
        High = 4,
        Elite = 5,
        Max = 6
    }

    /// <summary>
    /// Generates standardized tournament loadouts and temporarily overrides character equipment.
    /// Never mutates hero equipment permanently. Restores original equipment after tournament.
    /// </summary>
    public sealed class EquipmentStandardizerService
    {
        private static EquipmentStandardizerService? _instance;
        public static EquipmentStandardizerService Instance => _instance ??= new EquipmentStandardizerService();

        // Track original equipment so we can restore it post-tournament.
        private readonly Dictionary<Hero, Equipment> _originalEquipment = new();

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>Applies standardized equipment to all tournament participants.</summary>
        public void StandardizeParticipants(IEnumerable<CharacterObject> participants, Town hostTown)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null || !settings.EnableEquipmentStandardizer) return;

            ArmorTier tier = ParseTier(GetSelectedTierName(settings));
            var pool = BuildArmorPool(tier, hostTown, settings.EquipmentUseCulturePools);

            foreach (CharacterObject character in participants)
            {
                if (character is null) continue;

                Hero? hero = character.IsHero ? character.HeroObject : null;

                if (hero is not null)
                {
                    if (settings.EquipmentAllowPlayerOverride && hero == Hero.MainHero) continue;
                    BackupEquipment(hero);
                    ApplyArmorToHero(hero, pool, settings);
                }
                // Non-hero characters use the generated equipment element for their slot
                // via the patch (EquipmentNormalizePatch).
            }
        }

        /// <summary>Restores all backed-up hero equipment. Call when a tournament ends.</summary>
        public void RestoreAll()
        {
            foreach (var (hero, equipment) in _originalEquipment)
            {
                try { RestoreHeroEquipment(hero, equipment); }
                catch (Exception ex) { TMLog.Exception(ex, $"Restoring equipment for {hero?.Name}"); }
            }
            _originalEquipment.Clear();
        }

        /// <summary>Generates a standalone equipment element for non-hero participants.</summary>
        public EquipmentElement GetArmorElement(EquipmentIndex slot, Town hostTown, CultureObject? culture)
        {
            var settings = TournamentMasterySettings.Instance;
            if (settings is null) return default;

            ArmorTier tier = ParseTier(GetSelectedTierName(settings));
            var pool = BuildArmorPool(tier, hostTown, settings.EquipmentUseCulturePools);
            return PickSlotItem(slot, pool, settings);
        }

        // ── Private helpers ──────────────────────────────────────────────

        private void BackupEquipment(Hero hero)
        {
            if (!_originalEquipment.ContainsKey(hero))
            {
                var backup = new Equipment(false);
                for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumEquipmentSetSlots; i++)
                    backup[i] = hero.BattleEquipment[i];
                _originalEquipment[hero] = backup;
            }
        }

        private static void RestoreHeroEquipment(Hero hero, Equipment original)
        {
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumEquipmentSetSlots; i++)
                hero.BattleEquipment[i] = original[i];
        }

        private static void ApplyArmorToHero(Hero hero, ArmorPool pool, TournamentMasterySettings s)
        {
            if (s.EquipmentStandardizeHead)
                hero.BattleEquipment[EquipmentIndex.Head] = PickSlotItem(EquipmentIndex.Head, pool, s);

            if (s.EquipmentStandardizeBody)
                hero.BattleEquipment[EquipmentIndex.Body] = PickSlotItem(EquipmentIndex.Body, pool, s);

            if (s.EquipmentStandardizeArms)
                hero.BattleEquipment[EquipmentIndex.Gloves] = PickSlotItem(EquipmentIndex.Gloves, pool, s);

            if (s.EquipmentStandardizeLegs)
                hero.BattleEquipment[EquipmentIndex.Leg] = PickSlotItem(EquipmentIndex.Leg, pool, s);

            if (!s.EquipmentPreserveHorses)
                hero.BattleEquipment[EquipmentIndex.Horse] = default;

            if (s.EquipmentDebugLog)
                TMLog.Debug($"Standardized armor for {hero.Name} (tier: {s.EquipmentArmorTier.SelectedValue})");
        }

        private static EquipmentElement PickSlotItem(EquipmentIndex slot, ArmorPool pool, TournamentMasterySettings s)
        {
            ItemObject.ItemTypeEnum itemType = slot switch
            {
                EquipmentIndex.Head   => ItemObject.ItemTypeEnum.HeadArmor,
                EquipmentIndex.Body   => ItemObject.ItemTypeEnum.BodyArmor,
                EquipmentIndex.Gloves => ItemObject.ItemTypeEnum.HandArmor,
                EquipmentIndex.Leg    => ItemObject.ItemTypeEnum.LegArmor,
                _                     => ItemObject.ItemTypeEnum.Invalid
            };

            if (itemType == ItemObject.ItemTypeEnum.Invalid) return default;

            List<ItemObject>? candidates = pool.GetItems(itemType);
            if (candidates is null || candidates.Count == 0) return default;

            // In strict mode pick index 0 (deterministic); otherwise pick a random one.
            var item = s.EquipmentStrictFairness
                ? candidates[0]
                : candidates[MBRandom.RandomInt(candidates.Count)];

            return item is not null ? new EquipmentElement(item) : default;
        }

        private static ArmorPool BuildArmorPool(ArmorTier tier, Town hostTown, bool useCulture)
        {
            var pool = new ArmorPool(tier);

            // Attempt culture-aware fill first.
            CultureObject? culture = useCulture ? hostTown?.Culture : null;

            foreach (ItemObject item in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
            {
                if (item is null || !IsArmorItem(item)) continue;
                if (!IsInTierRange(item, tier)) continue;
                if (culture is not null && item.Culture is not null && item.Culture != culture) continue;

                pool.Add(item);
            }

            // Fallback: if culture pool is empty, fill from all cultures.
            if (culture is not null && pool.IsEmpty)
            {
                TMLog.Debug($"Culture pool empty for {culture.Name}, falling back to generic pool.");
                foreach (ItemObject item in MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
                {
                    if (item is null || !IsArmorItem(item)) continue;
                    if (!IsInTierRange(item, tier)) continue;
                    pool.Add(item);
                }
            }

            return pool;
        }

        private static bool IsArmorItem(ItemObject item) =>
            item.ItemType is ItemObject.ItemTypeEnum.HeadArmor
                          or ItemObject.ItemTypeEnum.BodyArmor
                          or ItemObject.ItemTypeEnum.HandArmor
                          or ItemObject.ItemTypeEnum.LegArmor;

        private static bool IsInTierRange(ItemObject item, ArmorTier tier)
        {
            int value = item.Value;
            return tier switch
            {
                ArmorTier.Naked   => value == 0,
                ArmorTier.Peasant => value is >= 1 and <= 500,
                ArmorTier.Low     => value is >= 501 and <= 2000,
                ArmorTier.Mid     => value is >= 2001 and <= 6000,
                ArmorTier.High    => value is >= 6001 and <= 15000,
                ArmorTier.Elite   => value is >= 15001 and <= 40000,
                ArmorTier.Max     => value > 0,
                _                 => true
            };
        }

        private static ArmorTier ParseTier(string? tierName) => tierName switch
        {
            "Naked"   => ArmorTier.Naked,
            "Peasant" => ArmorTier.Peasant,
            "Low"     => ArmorTier.Low,
            "High"    => ArmorTier.High,
            "Elite"   => ArmorTier.Elite,
            "Max"     => ArmorTier.Max,
            _         => ArmorTier.Mid
        };

        private static string GetSelectedTierName(TournamentMasterySettings s)
        {
            try { return s.EquipmentArmorTier.SelectedValue; }
            catch { return "Mid"; }
        }

        // ── Inner pool class ─────────────────────────────────────────────

        private sealed class ArmorPool
        {
            private readonly Dictionary<ItemObject.ItemTypeEnum, List<ItemObject>> _items = new();

            public ArmorTier Tier { get; }
            public bool IsEmpty => _items.Values.All(l => l.Count == 0);

            public ArmorPool(ArmorTier tier)
            {
                Tier = tier;
                _items[ItemObject.ItemTypeEnum.HeadArmor] = new List<ItemObject>();
                _items[ItemObject.ItemTypeEnum.BodyArmor] = new List<ItemObject>();
                _items[ItemObject.ItemTypeEnum.HandArmor] = new List<ItemObject>();
                _items[ItemObject.ItemTypeEnum.LegArmor]  = new List<ItemObject>();
            }

            public void Add(ItemObject item)
            {
                if (_items.TryGetValue(item.ItemType, out var list))
                    list.Add(item);
            }

            public List<ItemObject>? GetItems(ItemObject.ItemTypeEnum type)
                => _items.TryGetValue(type, out var list) ? list : null;
        }
    }
}
