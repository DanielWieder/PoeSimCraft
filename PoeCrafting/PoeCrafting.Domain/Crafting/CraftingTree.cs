using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingTree
    {
        private CurrencyFactory factory;

        public InsertCraftingStep BeforeSelected;
        public InsertCraftingStep AfterSelected;
        public InsertCraftingStep InsideSelected;

        public List<ICraftingStep> CraftingSteps;

        public void OnSelection(ICraftingStep selected)
        {
            if (selected.GetType() == typeof(InsertCraftingStep))
            {
                return;
            }

            if (BeforeSelected != null)
            {
                Remove(BeforeSelected, CraftingSteps);
            }
            if (AfterSelected != null)
            {
                Remove(AfterSelected, CraftingSteps);
            }
            if (InsideSelected != null)
            {
                Remove(InsideSelected, CraftingSteps);
            }

            bool located = false;
            IList<ICraftingStep> beforeList = null;
            int beforeIndex = -1;

            IList<ICraftingStep> afterList = null;
            int afterIndex = -1;

            Action<ICraftingStep, int, IList<ICraftingStep>> action = (current, index, list) =>
            {
                if (located)
                {
                    afterList = list;
                    afterIndex = index;
                    return;
                }

                if (current == selected)
                {
                    located = true;
                }

                if (!located)
                {
                    beforeList = list;
                    beforeIndex = index;
                }
            };

            IterateSteps(action, CraftingSteps);

            if (beforeList != null)
            {
                var beforeSelected = new InsertCraftingStep(factory);
                BeforeSelected = beforeSelected;
                beforeList.Insert(beforeIndex, beforeSelected);
            }

            if (beforeList == afterList)
            {
                afterIndex++;
            }

            if (afterList != null)
            {
                var afterSelected = new InsertCraftingStep(factory);
                AfterSelected = afterSelected;
                afterList.Insert(afterIndex, afterSelected);
            }

            if (selected.HasChildren && !selected.Children.Any())
            {
                var insideSelected = new InsertCraftingStep(factory);
                InsideSelected = insideSelected;
                selected.Children.Add(insideSelected);
            }
        }

        public void Craft(Equipment equipment)
        {
            IterateSteps((craftingStep, index, list) => craftingStep.Craft(equipment), CraftingSteps);
        }

        public void UpdateStatus()
        {
            ItemStatus status = new ItemStatus();
            IterateSteps((craftingStep, index, list) => status = craftingStep.UpdateStatus(status), CraftingSteps);
        }

        private double CurrencySpent()
        {
            var allNodes = CraftingSteps.Map(x => true, x => x.Children);
            return allNodes.OfType<CurrencyCraftingStep>().Sum(x => x.CurrencyUsed);
        }

        private bool Remove(ICraftingStep toRemove, IList<ICraftingStep> craftingSteps)
        {
            if (craftingSteps.Contains(toRemove))
            {
                craftingSteps.Remove(toRemove);
                return true;
            }
            foreach (var craftingStep in craftingSteps)
            {
                var isRemoved = Remove(toRemove, craftingSteps);
                if (isRemoved)
                {
                    return true;
                }
            }
            return false;
        }

        private void IterateSteps(Action<ICraftingStep, int, IList<ICraftingStep>> action, IList<ICraftingStep> craftingSteps)
        {
            for (int i = 0; i < craftingSteps.Count; i++)
            {
                action(craftingSteps[i], i, craftingSteps);

                if (craftingSteps[i].HasChildren && craftingSteps[i].Children.Any())
                {
                    IterateSteps(action, craftingSteps[i].Children);
                }
            }
        }
    }
}
