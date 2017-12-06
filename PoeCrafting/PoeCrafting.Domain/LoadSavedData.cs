using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Persistance;

namespace PoeCrafting.Domain
{
    public class LoadSavedData
    {
        private readonly CurrencyFactory _currency;
        private readonly EquipmentFactory _equipment;
        private List<Affix> _affixes;

        public LoadSavedData(CurrencyFactory currency, EquipmentFactory equipment)
        {
            _currency = currency;
            _equipment = equipment;
        }

        public LoadSavedData()
        {

        }

        public SimData Execute(SimulationJson json)
        {
            //_currency.UpdateValues(json.CraftingConfig.League);
         //   _equipment.Initialize(json.ItemConfig.ItemBase, json.ItemConfig.ItemLevel);
         //   _affixes = _equipment.GetPossibleAffixes();

       //     CraftingTree tree = new CraftingTree(_currency);

       //     tree.CraftingSteps.Clear();
          //  tree.CraftingSteps.Add(new StartCraftingStep());
        //    tree.CraftingSteps.AddRange(CreateCraftingTree(json.CraftingSteps));
        //    tree.CraftingSteps.Add(new EndCraftingStep());

            return new SimData
            {
           //     EquipmentFactory = _equipment,
                ItemPrototypes = json.ItemPrototypes,
            //    Tree = tree,
                Config = json.ItemConfig
            };
        }

        public List<ICraftingStep> CreateCraftingTree(IList<JsonCraftingStep> jsonCraftingSteps)
        {
            List<ICraftingStep> tree = new List<ICraftingStep>();

            foreach (var step in jsonCraftingSteps)
            {
                ICraftingStep craftingStep;

                switch (step.Name)
                {
                    case "End":
                        craftingStep = new EndCraftingStep();
                        break;
                    case "Start":
                        craftingStep = new StartCraftingStep();
                        break;
                    case "If":
                        craftingStep = new IfCraftingStep();
                        break;
                    case "While":
                        craftingStep = new WhileCraftingStep();
                        break;
                    case "Insert":
                        continue;
                    default:
                        if (_currency.Currency.Any(x => x.Name == step.Name))
                        {
                            var currency = _currency.GetCurrencyByName(step.Name);
                            craftingStep = new CurrencyCraftingStep(currency);
                        }

                        else
                        {
                            throw new InvalidOperationException(step.Name + " is not a recognized crafting step");
                        }
                        break;
                }

                if (step.JsonCondition != null && craftingStep.Condition != null)
                {
                    craftingStep.Condition.CraftingSubConditions = step.JsonCondition.CraftingSubConditions
                        .Select(JsonToSubCondition).ToList();
                }

                if (step.Children != null)
                {
                    craftingStep.Children?.AddRange(CreateCraftingTree(step.Children));
                }
                
                tree.Add(craftingStep);
            }
            return tree;
        }

        public CraftingSubcondition JsonToSubCondition(JsonSubcondition json)
        {
            return new CraftingSubcondition()
            {
                AggregateMax = json.AggregateMax,
                AggregateMin = json.AggregateMin,
                AggregateType = (SubconditionAggregateType)json.AggregateType,
                Name = json.Name,
                ValueType = (StatValueType)json.ValueType,
                MetaConditions = json.MetaConditions.Select(JsonToConditionAffix).ToList(),
                PrefixConditions = json.PrefixConditions.Select(JsonToConditionAffix).ToList(),
                SuffixConditions = json.SuffixConditions.Select(JsonToConditionAffix).ToList()
            };
        }

        public ConditionAffix JsonToConditionAffix(JsonAffix json)
        {
            return new ConditionAffix
            {
                Max1 = json.Max1,
                Max2 = json.Max2,
                Max3 = json.Max3,
                Min1 = json.Min1,
                Min2 = json.Min2,
                Min3 = json.Min3,
                ModType = json.ModType
            };
        }
    }


    public class SimData
    {
        public ItemConfig Config { get; set; }
        public CraftingTree Tree { get; set; }
        public EquipmentFactory EquipmentFactory { get; set; }
        public List<ItemPrototype> ItemPrototypes { get; set; }
    }
}
