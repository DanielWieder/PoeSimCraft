using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public static class AffixValueCalculator
    {
        private class ConditionContainer
        {
            public ItemBase ItemBase;
            public List<ItemProperty> Affixes;
        }

        private class ItemProperty
        {
            public string ModType;
            public AffixType Type;
            public int Value;
        }

        public static int GetAffixValue(string mod, Equipment item, AffixType type, SubconditionValueType valueType)
        {
            var conditionItem = new ConditionContainer()
            {
                ItemBase = item.ItemBase,
                Affixes = item.Prefixes.Concat(item.Suffixes).Select(x => StatToCondition(x.Affix, valueType, x.Value1)).ToList()
            };
            return GetAffixValue(mod, type, conditionItem);
        }

        public static int GetModMax(string modType, ItemBase itemBase, List<Affix> affixes, AffixType type)
        {
            var max = SubconditionValueType.Max;

            var conditionItem = new ConditionContainer
            {
                ItemBase = itemBase,
                Affixes = affixes.Select(x => StatToCondition(x, max)).ToList()
            };
            return GetAffixValue(modType, type, conditionItem);
        }

        private static ItemProperty StatToCondition(Affix affix, SubconditionValueType valueType, int value = 0)
        {
            return new ItemProperty()
            {
                ModType = affix.ModType,
                Value = GetTypedValue(affix, valueType, value),
                Type = (AffixType)Enum.Parse(typeof(AffixType), affix.Type, true)
            };
        }

        private static int GetTypedValue(Affix affix, SubconditionValueType valueType, int value = 0)
        {
            switch (valueType)
            {
                case SubconditionValueType.Flat:
                    return value;
                case SubconditionValueType.Max:
                    return affix.StatMax1;
                case SubconditionValueType.Tier:
                    return affix.Tier;
                default:
                    throw new NotImplementedException("That subcondition value type does not exist");
            }
        }

        private static int GetAffixValue(string mod, AffixType type, ConditionContainer a)
        {
            if (type == AffixType.Prefix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Prefix).FirstOrDefault(x => x.ModType == mod)?.Value ?? -1;
            }
            if (type == AffixType.Suffix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Suffix).FirstOrDefault(x => x.ModType == mod)?.Value ?? -1;
            }
            if (type == AffixType.Meta)
            {
                return GetMetaConditionValue(mod, a);
            }

            throw new NotImplementedException($"The affix type {Enum.GetName(typeof(AffixType), type)} is not recognized");
        }

        private static int GetMetaConditionValue(string modType, ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.Type == AffixType.Suffix).ToList();

            if (modType.Contains("OpenPrefix"))
            {
                return 3 - prefixes.Count();
            }
            if (modType.Contains("OpenSuffix"))
            {
                return 3 - suffixes.Count();
            }
            if (modType == "TotalEnergyShield")
            {
                return GetDefenseConditionValue(a, "EnergyShield", "IncreasedEnergyShield", "EnergyShieldPercent");
            }
            if (modType == "TotalArmour")
            {
                return GetDefenseConditionValue(a, "Armour", "IncreasedPhysicalDamageReductionRating",
                    "IncreasedPhysicalDamageReductionRatingPercent");
            }
            if (modType == "TotalEvasion")
            {
                return GetDefenseConditionValue(a, "Evasion", "IncreasedEvasionRating", "EvasionRatingPercent");
            }
            if (modType == "TotalResistances")
            {
                var singleRes = suffixes
                    .Where(x =>
                       x.ModType == "ColdResistance" ||
                       x.ModType == "FireResistance" ||
                       x.ModType == "LightningResistance" ||
                       x.ModType == "ChaosResistance")
                    .ToList();

                var allRes = suffixes
                    .Where(x => x.ModType == "AllResistances")
                    .ToList();

                return singleRes.Sum(x => x.Value) +
                    allRes.Sum(x => x.Value) * 3;
            }
            if (modType == "TotalElementalResistances")
            {
                var singleRes = suffixes
                    .Where(x =>
                       x.ModType == "ColdResistance" ||
                       x.ModType == "FireResistance" ||
                       x.ModType == "LightningResistance")
                    .ToList();

                var allRes = suffixes
                    .Where(x => x.ModType == "AllResistances")
                    .ToList();

                return singleRes.Sum(x => x.Value) +
                       allRes.Sum(x => x.Value) * 3;

            }

            throw new NotImplementedException($"The meta affix {modType} is not recognized");
        }

        private static int GetDefenseConditionValue(ConditionContainer a, string propertyName, string flatName, string percentName)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();

            var totalDefense = a.ItemBase.Properties[propertyName];

            var flatDefense = prefixes.Where(x => x.ModType == flatName).ToList();
            if (flatDefense.Any())
            {
                totalDefense += flatDefense.First().Value;
            }

            var totalPercentDefense = 100;

            // Quality bonus
            totalPercentDefense += 20;

            var percentDefense = prefixes.Where(x => x.ModType == percentName).ToList();
            if (percentDefense.Any())
            {
                totalPercentDefense += percentDefense.First().Value;
            }

            var defenseHybrid = prefixes.Where(x => x.ModType == "DefencesPercent").ToList();
            if (defenseHybrid.Any())
            {
                totalPercentDefense += defenseHybrid.First().Value;
            }

            var stunRecoveryHybrid = prefixes.Where(x => x.ModType == "DefencesPercentAndStunRecovery").ToList();
            if (stunRecoveryHybrid.Any())
            {
                totalPercentDefense += stunRecoveryHybrid.First().Value;
            }

            return (int)(totalDefense * totalPercentDefense / 100);
        }
    }
}
