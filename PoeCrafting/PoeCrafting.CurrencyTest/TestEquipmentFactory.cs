using System;
using System.Collections.Generic;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    public class TestEquipmentFactory
    {
        public Equipment GetNormal()
        {
            var prefixes = getTestPrefixes();
            var suffixes = getTestSuffixes();
            var affixes = new List<Affix>();
            affixes.AddRange(prefixes);
            affixes.AddRange(suffixes);

            return new Equipment()
            {
                PossiblePrefixes = prefixes,
                PossibleSuffixes = suffixes,
                PossibleAffixes = affixes,
                ItemLevel = 1,
                PrefixWeight = 4,
                SuffixWeight = 4,
                TotalWeight = 8,
                ItemBase = getTestBase()
            };
        }

        private ItemBase getTestBase()
        {
            return new ItemBase()
            {
                Level = 0,
                Name = "Test",
                Subtype = "Test",
                Type = "Test"
            };
        }

        private List<Affix> getTestPrefixes()
        {
            return new List<Affix>()
            {
                getTestAffix(1, "Prefix"),
                getTestAffix(2, "Prefix"),
                getTestAffix(3, "Prefix"),
                getTestAffix(4, "Prefix"),
            };
        }

        private List<Affix> getTestSuffixes()
        {
            return new List<Affix>()
            {
                getTestAffix(1, "Suffix"),
                getTestAffix(2, "Suffix"),
                getTestAffix(3, "Suffix"),
                getTestAffix(4, "Suffix"),
            };
        }

        private Affix getTestAffix(int index, String type)
        {
            return new Affix
            {
                Faction = 1,
                Group = "Test",
                ILvl = 0,
                ModName = "Test" + type + index,
                ModType = "Test" + type + index,
                ModTypeWeight = 1,
                Name = "Test",
                Priority = 0,
                SpawnTag = "Test",
                Tier = 0,
                Type = type,
                Weight = 1,
                StatName1 = "Test",
                StatMin1 = 0,
                StatMax1 = 1
            };
        }
    }
}
