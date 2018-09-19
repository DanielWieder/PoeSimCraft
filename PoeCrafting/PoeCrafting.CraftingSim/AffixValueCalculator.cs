using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public class AffixValueCalculator
    {
        public List<int> GetAffixValues(string mod, Equipment item, AffixType type, StatValueType valueType)
        {
            var conditionItem = new ConditionContainer()
            {
                ItemBase = item.ItemBase,
                Affixes = item.Prefixes.Concat(item.Suffixes).Select(x => StatToCondition(x.Affix, valueType, x.Values)).ToList()
            };
            return GetAffixValue(mod, type, conditionItem);
        }

        public List<int> GetModMax(string modType, ItemBase itemBase, List<Affix> affixes, AffixType type)
        {
            var max = StatValueType.Max;

            var conditionItem = new ConditionContainer
            {
                ItemBase = itemBase,
                Affixes = affixes.Select(x => StatToCondition(x, max)).ToList()
            };
            return GetAffixValue(modType, type, conditionItem);
        }

        private ItemProperty StatToCondition(Affix affix, StatValueType valueType, List<int> values = null)
        {
            return new ItemProperty()
            {
                ModType = affix.ModType,
                Values = GetTypedValue(affix, valueType, values),
                Type = (AffixType)Enum.Parse(typeof(AffixType), affix.Type, true)
            };
        }

        private List<int> GetTypedValue(Affix affix, StatValueType valueType, List<int> values = null)
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
                MetaAffixValueCalculator calculator = new MetaAffixValueCalculator();
                return new List<int> { calculator.GetMetaConditionValue(mod, a)};
            }

            throw new NotImplementedException($"The affix type {Enum.GetName(typeof(AffixType), type)} is not recognized");
        }
    }
}
