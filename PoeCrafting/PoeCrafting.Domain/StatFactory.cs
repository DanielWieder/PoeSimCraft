using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Data;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public class StatFactory
    {
        public static void AddExplicit(IRandom random, Equipment item)
        {
            AddExplicit(random, item, item.PossibleAffixes);
        }

        public static void AddExplicit(IRandom random, Equipment item, List<Affix> possibleAffixs )
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

            List<Affix> pool = new List<Affix>();

            if (item.Suffixes.Count < maxAffixCount/2)
            {
               pool.AddRange(possibleAffixs.Where(x => x.Type == "suffix")); 
            }
            if (item.Prefixes.Count < maxAffixCount/2)
            {
                pool.AddRange(possibleAffixs.Where(x => x.Type == "prefix"));
            }

            if (pool.Count == 0)
            {
                return;
            }

            var affix = SelectAffixFromPool(random, item.ItemLevel, item.Stats, pool);
            var stat = AffixToStat(random, affix);

            item.Stats.Add(stat);
        }

        public static void SetImplicit(IRandom random, Equipment item)
        {
            var pool = item.PossibleAffixes.Where(x => x.Type == "corrupted").ToList();
            var affix = SelectAffixFromPool(random, item.ItemLevel, new List<Stat>(), pool);
            var stat = AffixToStat(random, affix);

            item.Implicit = stat;
        }

        private static Affix SelectAffixFromPool(IRandom random, int itemLevel, List<Stat> current, List<Affix> pool)
        {
            var validmodifiers = pool
                .Where(x => x.Weight > 0)
                .Where(x => x.ILvl <= itemLevel)
                .Where(x => current.All(y => y.Affix.Group != x.Group))
                .ToList();

            var totalOdds = validmodifiers.Select(x => x.Weight).Sum();
            var roll = random.NextDouble()*totalOdds;

            double accumulator = 0;
            for (int i = 0; i < validmodifiers.Count(); i++)
            {
                accumulator += validmodifiers[i].Weight;

                if (roll <= accumulator)
                {
                    return validmodifiers[i];
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
