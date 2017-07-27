using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Data;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public static class StatFactory
    {
        public static void AddExplicit(IRandom random, Equipment item)
        {
            AddExplicit(random, item, item.PossibleAffixes);
        }

        public static void AddExplicit(IRandom random, Equipment item, List<Affix> possibleAffixes )
        {
            int maxAffixCount;
            
            switch (item.Rarity)
            {
                case EquipmentRarity.Magic:
                    maxAffixCount = 2;
                    break;
                case EquipmentRarity.Rare:
                    maxAffixCount = 6;
                    break;
                default:
                    maxAffixCount = 0;
                    break;
            }

            if (item.Stats.Count >= maxAffixCount)
            {
                return;
            }

            var totalWeight = item.TotalWeight;

            List<Affix> pool = new List<Affix>();

            if (item.Suffixes.Count < maxAffixCount/2)
            {
                pool.AddRange(possibleAffixes.Where(x => x.Type == "suffix"));
                totalWeight -= item.Suffixes.Sum(x => x.Affix.ModTypeWeight);
            }
            else
            {
                totalWeight -= item.SuffixWeight;
            }

            if (item.Prefixes.Count < maxAffixCount/2)
            {
                pool.AddRange(possibleAffixes.Where(x => x.Type == "prefix"));
                totalWeight -= item.Prefixes.Sum(x => x.Affix.ModTypeWeight);
            }
            else
            {
                totalWeight -= item.PrefixWeight;
            }

            if (pool.Count == 0)
            {
                return;
            }

            var affix = SelectAffixFromPool(random, item.Stats, pool, totalWeight);
            var stat = AffixToStat(random, affix);

            item.Stats.Add(stat);
        }

        public static void SetImplicit(IRandom random, Equipment item)
        {
            var pool = item.PossibleAffixes.Where(x => x.Type == "corrupted").ToList();
            var affix = SelectAffixFromPool(random, new List<Stat>(), pool, item.TotalWeight);
            var stat = AffixToStat(random, affix);

            item.Implicit = stat;
        }

        private static Affix SelectAffixFromPool(IRandom random, List<Stat> current, List<Affix> pool, int totalWeight)
        {

            var currentModTypes = current.Select(x => x.Affix.ModType).ToArray();
            var filtered = new List<Affix>(pool);
            for (int i = filtered.Count - 1; i >= 0; i--)
            {
                if (currentModTypes.Contains(filtered[i].ModType))
                {
                    filtered.RemoveAt(i);
                }
            }

            var roll = random.NextDouble()* totalWeight;

            double accumulator = 0;
            foreach (var modifier in filtered)
            {
                accumulator += modifier.Weight;

                if (roll <= accumulator)
                {
                    return modifier;
                }
            }
            return null;
        }

        public static void Reroll(IRandom random, Stat stat)
        {
            if (stat?.Affix == null)
                return;

            var mod = stat.Affix;

            if (!string.IsNullOrEmpty(mod.StatName1))
                stat.Value1 = random.Next(mod.StatMin1, mod.StatMax1);
            if (!string.IsNullOrEmpty(mod.StatName2))
                stat.Value2 = random.Next(mod.StatMin2, mod.StatMax2);
            if (!string.IsNullOrEmpty(mod.StatName3))
                stat.Value3 = random.Next(mod.StatMin3, mod.StatMax3);
        }

        public static Stat AffixToStat(IRandom random, Affix affix)
        {
            Stat stat = new Stat();
            stat.Affix = affix;
            Reroll(random, stat);
            return stat;
        }
    }
}
