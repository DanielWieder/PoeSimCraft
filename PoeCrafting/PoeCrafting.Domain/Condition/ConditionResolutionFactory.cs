using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public class ConditionResolutionFactory
    {
        public static ConditionResolution ResolveCondition(ConditionAffix affix, Equipment item, AffixType type, StatValueType valueType)
        {
            AffixValueCalculator calculator = new AffixValueCalculator();
            var value = calculator.GetAffixValues(affix.ModType, item, type, valueType);

            return new ConditionResolution()
            {
                IsPresent = value.Any(),
                IsMatch = value.Any() && IsValueWithinBounds(affix, value),
                Values = value
            };
        }

        private static bool IsValueWithinBounds(ConditionAffix affix, List<int> value)
        {
            bool hasRequirement1 = affix.Min1.HasValue || affix.Max1.HasValue;
            bool hasRequirement2 = affix.Min2.HasValue || affix.Max2.HasValue;
            bool hasRequirement3 = affix.Min3.HasValue || affix.Max3.HasValue;

            bool meetsRequirement1 = !hasRequirement1 || (value.Count >= 1 && 
                        (!affix.Min1.HasValue || value[0] >= affix.Min1.Value) &&
                        (!affix.Max1.HasValue || value[0] <= affix.Max1.Value));

            bool meetsRequirement2 = !hasRequirement2 || (value.Count >= 2 &&
                         (!affix.Min2.HasValue || value[1] >= affix.Min2.Value) &&
                         (!affix.Max2.HasValue || value[1] <= affix.Max2.Value));

            bool meetsRequirement3 = !hasRequirement3 || (value.Count >= 3 &&
                         (!affix.Min3.HasValue || value[2] >= affix.Min3.Value) &&
                         (!affix.Max3.HasValue || value[2] <= affix.Max3.Value));

            return meetsRequirement1 && meetsRequirement2 && meetsRequirement3;
        }
    }
}
