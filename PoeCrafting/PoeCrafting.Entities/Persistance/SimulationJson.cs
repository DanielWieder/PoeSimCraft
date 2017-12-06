using PoeCrafting.Entities.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities.Persistance
{
    public class JsonAffix
    {
        public string ModType { get; set; }
        public int Min1 { get; set; }
        public int Min2 { get; set; }
        public int Min3 { get; set; }
        public int Max1 { get; set; }
        public int Max2 { get; set; }
        public int Max3 { get; set; }
    }

    public class JsonSubcondition
    {
        public string Name { get; set; }
        public int ValueType { get; set; }
        public int AggregateType { get; set; }
        public int AggregateMin { get; set; }
        public int AggregateMax { get; set; }
        public int AggregateValueType { get; set; }
        public List<JsonAffix> PrefixConditions { get; set; }
        public List<JsonAffix> SuffixConditions { get; set; }
        public List<JsonAffix> MetaConditions { get; set; }
    }

    public class JsonCondition
    {
        public IList<JsonSubcondition> CraftingSubConditions { get; set; }
    }

    public class JsonCraftingStep
    {
        public string Name { get; set; }
        public IList<JsonCraftingStep> Children { get; set; }
        public JsonCondition JsonCondition { get; set; }
    }

    public class SimulationJson
    {
        public CraftingConfig CraftingConfig { get; set; }
        public ItemConfig ItemConfig { get; set; } 
        public IList<JsonCraftingStep> CraftingSteps { get; set; }
        public List<ItemPrototype> ItemPrototypes { get; set; }
    }
}

