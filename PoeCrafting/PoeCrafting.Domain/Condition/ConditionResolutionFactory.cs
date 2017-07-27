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
        public static ConditionResolution ResolveCondition(ConditionAffix affix, Equipment item, AffixType type, SubconditionValueType valueType)
        {
            var value = AffixValueCalculator.GetAffixValue(affix.ModType, item, type, valueType);

            return new ConditionResolution()
            {
                IsPresent = value != -1,
                IsMatch = value != -1 && IsValueWithinBounds(affix, value),
                Value = value
            };
        }

        private static bool IsValueWithinBounds(ConditionAffix affix, int value)
        {
            return
                (!affix.Min.HasValue || value >= affix.Min.Value) &&
                (!affix.Max.HasValue || value <= affix.Max.Value);
        }
    }
}
