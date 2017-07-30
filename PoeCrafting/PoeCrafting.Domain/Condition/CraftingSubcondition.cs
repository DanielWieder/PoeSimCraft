using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Condition
{
    public class CraftingSubcondition : ICraftingSubcondition
    {
        public SubconditionAggregateType AggregateType { get; set; } = SubconditionAggregateType.And;
        public int? AggregateMin { get; set; }
        public int? AggregateMax { get; set; }

        public StatValueType ValueType { get; set; }

        public List<ConditionAffix> PrefixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> SuffixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> MetaConditions { get; set; } = new List<ConditionAffix>();
        public string Name { get; set; }

        public bool IsValid(Equipment item)
        {
            var prefixResolutions =
                PrefixConditions.Select(
                    x => ConditionResolutionFactory.ResolveCondition(x, item, AffixType.Prefix, ValueType)).ToList();

            var suffixResolutions =
                SuffixConditions.Select(
                    x => ConditionResolutionFactory.ResolveCondition(x, item, AffixType.Suffix, ValueType)).ToList();

            var metaResolutions =
                MetaConditions.Select(
                    x => ConditionResolutionFactory.ResolveCondition(x, item, AffixType.Meta, ValueType)).ToList();

            var allResolutions = prefixResolutions
                .Concat(suffixResolutions)
                .Concat(metaResolutions)
                .ToList();

            var matched = allResolutions.Count(x => x.IsMatch);
            var sum = allResolutions.Where(x => x.IsPresent).Sum(x => x.Values.Sum());

            switch (AggregateType)
            {
                case SubconditionAggregateType.Count:
                    return (AggregateMin.HasValue || matched >= AggregateMin) && (AggregateMax.HasValue || matched <= AggregateMax );
                case SubconditionAggregateType.And:
                    return allResolutions.All(x => x.IsPresent && x.IsMatch);
                case SubconditionAggregateType.If:
                    return allResolutions.Where(x => x.IsPresent).All(x => x.IsMatch);
                case SubconditionAggregateType.Not:
                    return !allResolutions.Any(x => x.IsMatch);
                case SubconditionAggregateType.Sum:
                    return (AggregateMin.HasValue || sum >= AggregateMin) && (AggregateMax.HasValue || sum <= AggregateMax);
                default:
                    throw new InvalidOperationException($"The subcondition aggregate type {Enum.GetName(typeof(SubconditionAggregateType), AggregateType)} is not recognized");
            }
        }
    }

    public interface ICraftingSubcondition
    {
        bool IsValid(Equipment item);
    }
}
