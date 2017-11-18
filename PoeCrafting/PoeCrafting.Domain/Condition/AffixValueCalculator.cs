using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public static class AffixValueCalculator
    {
        private static readonly List<string> PercentDefenseAffixNames = new List<string>
        {
            "Local Increased Evasion Rating Percent",
            "Local Increased Energy Shield Percent",
            "Local Increased Physical Damage Reduction Rating Percent",
            "Str Master Armour Percent Crafted",
            "Str Master Evasion Percent Crafted",
            "Local Increased Armour And Evasion",
            "Local Increased Energy Shield Percent",
            "Local Increased Evasion Rating Percent",
            "Local Increased Armour And Energy Shield",
            "Str Master Energy Shield Percent Crafted",
            "Local Increased Evasion And Energy Shield",
            "Local Increased Armour Evasion Energy Shield",
            "Str Master Armour And Evasion Percent Crafted",
            "Str Master Armour And Energy Shield Percent Crafted",
            "Str Master Evasion And Energy Shield Percent Crafted",
            "Local Increased Physical Damage Reduction Rating Percent"
        };

        private static readonly List<string> HybridDefenseAffixNames = new List<string>
        {
            "Local Increased Armour And Evasion And Stun Recovery",
            "Local Increased Energy Shield Percent And Stun Recovery",
            "Local Increased Evasion Rating Percent And Stun Recovery",
            "Local Increased Armour And Energy Shield And Stun Recovery",
            "Local Increased Evasion And Energy Shield And Stun Recovery",
            "Local Increased Armour Evasion Energy Shield Stun Recovery",
            "Local Increased Physical Damage Reduction Rating Percent And Stun Recovery"
        };

        private static List<string> EnergyShieldPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("Energy Shield")).ToList();
        private static List<string> EvasionPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("Evasion")).ToList();
        private static List<string> ArmourPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("Armour") || x.Contains("Physical Damage Reduction Rating")).ToList();

        private static List<string> HybridEnergyShieldPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("EnergyShield")).ToList();
        private static List<string> HybridEvasionPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("Evasion")).ToList();
        private static List<string> HybridArmourPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("Armour") || x.Contains("Physical Damage Reduction Rating")).ToList();

        private class ConditionContainer
        {
            public ItemBase ItemBase;
            public List<ItemProperty> Affixes;
        }

        private class ItemProperty
        {
            public string ModType;
            public AffixType Type;
            public List<int> Values;
        }

        public static List<int> GetAffixValues(string mod, Equipment item, AffixType type, StatValueType valueType)
        {
            var conditionItem = new ConditionContainer()
            {
                ItemBase = item.ItemBase,
                Affixes = item.Prefixes.Concat(item.Suffixes).Select(x => StatToCondition(x.Affix, valueType, x.Values)).ToList()
            };
            return GetAffixValue(mod, type, conditionItem);
        }

        public static List<int> GetModMax(string modType, ItemBase itemBase, List<Affix> affixes, AffixType type)
        {
            var max = StatValueType.Max;

            var conditionItem = new ConditionContainer
            {
                ItemBase = itemBase,
                Affixes = affixes.Select(x => StatToCondition(x, max)).ToList()
            };
            return GetAffixValue(modType, type, conditionItem);
        }

        private static ItemProperty StatToCondition(Affix affix, StatValueType valueType, List<int> values = null)
        {
            return new ItemProperty()
            {
                ModType = affix.ModType,
                Values = GetTypedValue(affix, valueType, values),
                Type = (AffixType)Enum.Parse(typeof(AffixType), affix.Type, true)
            };
        }

        private static List<int> GetTypedValue(Affix affix, StatValueType valueType, List<int> values = null)
        {
            switch (valueType)
            {
                case StatValueType.Flat:
                    return values;
                case StatValueType.Max:
                    return affix.MaxStats;
                case StatValueType.Tier:
                    return new List<int> { affix.Tier };
                default:
                    throw new NotImplementedException("That subcondition value type does not exist");
            }
        }

        private static List<int> GetAffixValue(string mod, AffixType type, ConditionContainer a)
        {
            if (type == AffixType.Prefix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Prefix).FirstOrDefault(x => x.ModType == mod)?.Values ?? new List<int>();
            }
            if (type == AffixType.Suffix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Suffix).FirstOrDefault(x => x.ModType == mod)?.Values ?? new List<int>();
            }
            if (type == AffixType.Meta)
            {
                return new List<int> { GetMetaConditionValue(mod, a)};
            }

            throw new NotImplementedException($"The affix type {Enum.GetName(typeof(AffixType), type)} is not recognized");
        }

        private static int GetMetaConditionValue(string modType, ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.Type == AffixType.Suffix).ToList();

            if (modType.Contains("Open Prefix"))
            {
                return prefixes.Count > 3 ? 3 : 3 - prefixes.Count;
            }
            if (modType.Contains("Open Suffix"))
            {
                return suffixes.Count > 3 ? 3 : 3 - suffixes.Count;
            }
            if (modType == "Total Energy Shield")
            {
                return GetDefenseConditionValue(a, "Energy Shield", "Local Increased Energy Shield", EnergyShieldPercentDefenseAffixNames, HybridEnergyShieldPercentDefenseAffixNames);
            }
            if (modType == "Total Armour")
            {
                return GetDefenseConditionValue(a, "Armour", "Local Increased Physical Damage Reduction Rating", ArmourPercentDefenseAffixNames, HybridArmourPercentDefenseAffixNames);
            }
            if (modType == "Total Evasion")
            {
                return GetDefenseConditionValue(a, "Evasion", "Increased Evasion Rating", EvasionPercentDefenseAffixNames, HybridEvasionPercentDefenseAffixNames);
            }
            if (modType == "Total Resistances")
            {
                var coldRes = GetMaxOrZero(suffixes, "Cold Resist");
                var fireRes = GetMaxOrZero(suffixes, "Fire Resist");
                var lightningRes = GetMaxOrZero(suffixes, "Lightning Resist");
                var chaosRes = GetMaxOrZero(suffixes, "Chaos Resist");
                var allRes = GetMaxOrZero(suffixes, "All Resistances");

                var resList = new List<int>
                {
                    coldRes,
                    fireRes,
                    lightningRes,
                    chaosRes,
                    allRes * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }
            if (modType == "Total Elemental Resistances")
            {
                var coldRes = GetMaxOrZero(suffixes, "Cold Resist");
                var fireRes = GetMaxOrZero(suffixes, "Fire Resist");
                var lightningRes = GetMaxOrZero(suffixes, "Lightning Resist");
                var allRes = GetMaxOrZero(suffixes, "All Resistances");

                var resList = new List<int>
                {
                    coldRes,
                    fireRes,
                    lightningRes,
                    allRes * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }

            throw new NotImplementedException($"The meta affix {modType} is not recognized");
        }

        private static int GetDefenseConditionValue(ConditionContainer a, string propertyName, string flatDefenseName, List<string> percentDefensesNames, List<string> hybridDefenseNames)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();

            var percentDefenses = prefixes.Where(x => percentDefensesNames.Contains(x.ModType)).ToList();
            var hybridDefenses = prefixes.Where(x => hybridDefenseNames.Contains(x.ModType)).ToList();

            var baseDefense = a.ItemBase.Properties.ContainsKey(propertyName) ? a.ItemBase.Properties[propertyName] : 0;
            var flatDefense = GetMaxOrZero(prefixes, flatDefenseName);
            var percentDefense = GetMaxOrZero(percentDefenses);
            var hybridDefense = GetMaxOrZero(hybridDefenses);

            return (int)((baseDefense + flatDefense) * (120 + percentDefense + hybridDefense) / 100);
        }

        private static int GetMaxOrZero(List<ItemProperty> items, string property, int index = 0)
        {
            var properties = items.Where(x => x.ModType == property).ToList();
            return GetMaxOrZero(properties, index);
        }

        private static int GetMaxOrZero(List<ItemProperty> items, int index = 0)
        {
            if (!items.Any())
            {
                return 0;
            }
            return items.Max(x => x.Values[index]);
        }
    }
}
