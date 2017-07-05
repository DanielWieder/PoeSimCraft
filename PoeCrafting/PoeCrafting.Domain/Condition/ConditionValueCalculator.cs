using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public static class ConditionValueCalculator
    {
        private class ConditionContainer
        {
            public ItemBase ItemBase;
            public List<ItemProperty> Affixes;
        }

        private class ItemProperty
        {
            public string Group;
            public AffixType Type;
            public int Value;
        }

        public static int GetConditionValue(string group, Equipment item, AffixType type, SubconditionValueType valueType)
        {
            var conditionItem = new ConditionContainer()
            {
                ItemBase = item.ItemBase,
                Affixes = item.Prefixes.Concat(item.Suffixes).Select(x => StatToCondition(x.Affix, valueType, x.Value1)).ToList()
            };
            return GetConditionValue(group, type, conditionItem);
        }

        public static int GetGroupMax(string group, ItemBase itemBase, List<Affix> affixes, AffixType type)
        {
            var max = SubconditionValueType.Max;

            var conditionItem = new ConditionContainer
            {
                ItemBase = itemBase,
                Affixes = affixes.Select(x => StatToCondition(x, max)).ToList()
            };
            return GetConditionValue(group, type, conditionItem);
        }

        private static ItemProperty StatToCondition(Affix affix, SubconditionValueType valueType, int value = 0)
        {
            return new ItemProperty()
            {
                Group = affix.Group,
                Value = GetTypedValue(affix, valueType, value)
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

        private static int GetConditionValue(string group, AffixType type, ConditionContainer a)
        {
            if (type == AffixType.Prefix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Prefix).FirstOrDefault(x => x.Group == group)?.Value ?? -1;
            }
            if (type == AffixType.Suffix)
            {
                return a.Affixes.Where(x => x.Type == AffixType.Suffix).FirstOrDefault(x => x.Group == group)?.Value ?? -1;
            }
            if (type == AffixType.Meta)
            {
                return GetMetaConditionValue(group, a);
            }

            throw new NotImplementedException($"The affix type {Enum.GetName(typeof(AffixType), type)} is not recognized");
        }

        private static int GetMetaConditionValue(string group, ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.Type == AffixType.Suffix).ToList();

            if (group.Contains("OpenPrefix"))
            {
                return prefixes.Count();
            }
            if (group.Contains("OpenSuffix"))
            {
                return suffixes.Count();
            }
            if (group == "TotalEnergyShield")
            {
                return GetDefenseConditionValue(a, "EnergyShield", "IncreasedEnergyShield", "EnergyShieldPercent");
            }
            if (group == "TotalArmour")
            {
                return GetDefenseConditionValue(a, "Armour", "IncreasedPhysicalDamageReductionRating",
                    "IncreasedPhysicalDamageReductionRatingPercent");
            }
            if (group == "TotalEvasion")
            {
                return GetDefenseConditionValue(a, "Evasion", "IncreasedEvasionRating", "EvasionRatingPercent");
            }
            if (group == "TotalResistances")
            {
                var singleRes = suffixes
                    .Where(x =>
                       x.Group == "ColdResistance" ||
                       x.Group == "FireResistance" ||
                       x.Group == "LightningResistance" ||
                       x.Group == "ChaosResistance")
                    .ToList();

                var allRes = suffixes
                    .Where(x => x.Group == "AllResistances")
                    .ToList();

                return singleRes.Sum(x => x.Value) +
                    allRes.Sum(x => x.Value) * 3;
            }
            if (group == "TotalElementalResistances")
            {
                var singleRes = suffixes
                    .Where(x =>
                       x.Group == "ColdResistance" ||
                       x.Group == "FireResistance" ||
                       x.Group == "LightningResistance")
                    .ToList();

                var allRes = suffixes
                    .Where(x => x.Group == "AllResistances")
                    .ToList();

                return singleRes.Sum(x => x.Value) +
                       allRes.Sum(x => x.Value) * 3;

            }

            throw new NotImplementedException($"The meta affix {group} is not recognized");
        }

        private static int GetDefenseConditionValue(ConditionContainer a, string propertyName, string flatName, string percentName)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.Type == AffixType.Suffix).ToList();

            var totalDefense = a.ItemBase.Properties[propertyName];

            var flatDefense = prefixes.Where(x => x.Group == flatName).ToList();
            if (flatDefense.Any())
            {
                totalDefense += flatDefense.First().Value;
            }

            var totalPercentDefense = 100;

            var percentDefense = prefixes.Where(x => x.Group == percentName).ToList();
            if (percentDefense.Any())
            {
                totalPercentDefense += percentDefense.First().Value;
            }

            var defenseHybrid = prefixes.Where(x => x.Group == "DefencesPercent").ToList();
            if (defenseHybrid.Any())
            {
                totalPercentDefense += defenseHybrid.First().Value;
            }

            var stunRecoveryHybrid = prefixes.Where(x => x.Group == "DefencesPercentAndStunRecovery").ToList();
            if (stunRecoveryHybrid.Any())
            {
                totalPercentDefense += stunRecoveryHybrid.First().Value;
            }

            return (int)(totalDefense * totalPercentDefense / 100);
        }
    }
}
