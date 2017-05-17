using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Data;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public class StatFactory
    {
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

        public static Stat Get(IRandom random, Affix affix)
        {
            Stat stat = new Stat();
            stat.Affix = affix;
            Reroll(random, stat);
            return stat;
        }

        public static Stat Get(IRandom random, IList<Affix> possible, IList<Stat> current, int ilvl)
        {
            var validmodifiers = possible
                .Where(x => x.Weight > 0)
                .Where(x => x.ILvl <= ilvl)
                .Where(x => !current.Any(y => y.Affix.Group == x.Group))
                .ToList();

            if (validmodifiers.Count == 0)
            {
                throw new InvalidOperationException("There are no valid affixes that can be chosen");
            }

            var totalOdds = validmodifiers.Select(x => x.Weight).Sum();
            var roll = random.NextDouble() * totalOdds;

            double accumulator = 0;
            for (int i = 0; i < validmodifiers.Count(); i++)
            {
                accumulator += validmodifiers[i].Weight;

                if (roll <= accumulator)
                {
                    var selected = validmodifiers[i];
                    return Get(random, selected);
                }
            }

            throw new InvalidOperationException($"The roll ({roll}) and the prefix odds ({totalOdds}) do not match");
        }
    }
}
