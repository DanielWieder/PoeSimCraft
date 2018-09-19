using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Entities
{
    public class CraftingSubcondition
    {
        public SubconditionAggregateType AggregateType { get; set; } = SubconditionAggregateType.And;
        public int? AggregateMin { get; set; }
        public int? AggregateMax { get; set; }

        public StatValueType ValueType { get; set; }

        public List<ConditionAffix> PrefixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> SuffixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> MetaConditions { get; set; } = new List<ConditionAffix>();
        public string Name { get; set; }
    }
}
