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
            "LocalIncreasedEvasionRatingPercent",
            "LocalIncreasedEnergyShieldPercent",
            "LocalIncreasedPhysicalDamageReductionRatingPercent",
            "StrMasterArmourPercentCrafted",
            "StrMasterEvasionPercentCrafted",
            "LocalIncreasedArmourAndEvasion",
            "LocalIncreasedEnergyShieldPercent",
            "LocalIncreasedEvasionRatingPercent",
            "LocalIncreasedArmourAndEnergyShield",
            "StrMasterEnergyShieldPercentCrafted",
            "LocalIncreasedEvasionAndEnergyShield",
            "LocalIncreasedArmourEvasionEnergyShield",
            "StrMasterArmourAndEvasionPercentCrafted",
            "StrMasterArmourAndEnergyShieldPercentCrafted",
            "StrMasterEvasionAndEnergyShieldPercentCrafted",
            "LocalIncreasedPhysicalDamageReductionRatingPercent"
        };

        private static readonly List<string> HybridDefenseAffixNames = new List<string>
        {
            "LocalIncreasedArmourAndEvasionAndStunRecovery",
            "LocalIncreasedEnergyShieldPercentAndStunRecovery",
            "LocalIncreasedEvasionRatingPercentAndStunRecovery",
            "LocalIncreasedArmourAndEnergyShieldAndStunRecovery",
            "LocalIncreasedEvasionAndEnergyShieldAndStunRecovery",
            "LocalIncreasedArmourEvasionEnergyShieldStunRecovery",
            "LocalIncreasedPhysicalDamageReductionRatingPercentAndStunRecovery"
        };

        private static List<string> EnergyShieldPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("EnergyShield")).ToList();
        private static List<string> EvasionPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("Evasion")).ToList();
        private static List<string> ArmourPercentDefenseAffixNames => PercentDefenseAffixNames.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();

        private static List<string> HybridEnergyShieldPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("EnergyShield")).ToList();
        private static List<string> HybridEvasionPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("Evasion")).ToList();
        private static List<string> HybridArmourPercentDefenseAffixNames => HybridDefenseAffixNames.Where(x => x.Contains("Armour") || x.Contains("PhysicalDamageReductionRating")).ToList();

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

            if (modType.Contains("OpenPrefix"))
            {
                return prefixes.Count > 3 ? 3 : 3 - prefixes.Count;
            }
            if (modType.Contains("OpenSuffix"))
            {
                return suffixes.Count > 3 ? 3 : 3 - suffixes.Count;
            }
            if (modType == "TotalEnergyShield")
            {
                return GetDefenseConditionValue(a, "EnergyShield", "LocalIncreasedEnergyShield", EnergyShieldPercentDefenseAffixNames, HybridEnergyShieldPercentDefenseAffixNames);
            }
            if (modType == "TotalArmour")
            {
                return GetDefenseConditionValue(a, "Armour", "LocalIncreasedPhysicalDamageReductionRating", ArmourPercentDefenseAffixNames, HybridArmourPercentDefenseAffixNames);
            }
            if (modType == "TotalEvasion")
            {
                return GetDefenseConditionValue(a, "Evasion", "IncreasedEvasionRating", EvasionPercentDefenseAffixNames, HybridEvasionPercentDefenseAffixNames);
            }
            if (modType == "TotalResistances")
            {
                var coldRes = GetMaxOrZero(suffixes, "ColdResist");
                var fireRes = GetMaxOrZero(suffixes, "FireResist");
                var lightningRes = GetMaxOrZero(suffixes, "LightningResist");
                var chaosRes = GetMaxOrZero(suffixes, "ChaosResist");
                var allRes = GetMaxOrZero(suffixes, "AllResistances");

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
            if (modType == "TotalElementalResistances")
            {
                var coldRes = GetMaxOrZero(suffixes, "ColdResist");
                var fireRes = GetMaxOrZero(suffixes, "FireResist");
                var lightningRes = GetMaxOrZero(suffixes, "LightningResist");
                var allRes = GetMaxOrZero(suffixes, "AllResistances");

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
