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
        private readonly CurrencyFactory _factory;

        public InsertCraftingStep BeforeSelected;
        public InsertCraftingStep AfterSelected;
        public InsertCraftingStep InsideSelected;

        public List<ICraftingStep> CraftingSteps;

        public CraftingTree(CurrencyFactory factory)
        {
            _factory = factory;

            CraftingSteps = new List<ICraftingStep>();

            var start = new StartCraftingStep();
            CraftingSteps.Add(start);

            Select(start);
        }

        public void UpdateStatus()
        {
            IterateSteps((craftingStep, x, y) => craftingStep.ClearStatus(), CraftingSteps);

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(new ItemStatus(), nextSteps, (step, item) => step.UpdateStatus(item));
        }

        public Equipment Craft(Equipment equipment)
        {
            RemoveSelected();

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item));
            return equipment;
        }

        public void Select(ICraftingStep selected)
        {
            if (selected.GetType() == typeof(InsertCraftingStep))
            {
                return;
            }

            RemoveSelected();

            int selectedIndex = -1;
            IList<ICraftingStep> selectedList = null;

            IList<ICraftingStep> beforeList = null;
            int beforeIndex = -1;

            IList<ICraftingStep> afterList = null;
            int afterIndex = -1;

            Action<ICraftingStep, int, IList<ICraftingStep>> action = (current, index, list) =>
            {
                if (selectedIndex != -1)
                {
                    afterList = list;
                    afterIndex = index;
                    return;
                }

                if (current == selected)
                {
                    selectedIndex = index;
                    selectedList = list;
                }

                if (selectedIndex == -1)
                {
                    beforeList = list;
                    beforeIndex = index;
                }
            };

            IterateSteps(action, CraftingSteps);

            if (beforeList != null)
            {
                var beforeSelected = new InsertCraftingStep(_factory);
                BeforeSelected = beforeSelected;
                beforeList.Insert(beforeIndex + 1, beforeSelected);
            }

            if (beforeList == afterList)
            {
                afterIndex++;
            }

            if (afterList != null && afterIndex + 1 < afterList.Count)
            {
                var afterSelected = new InsertCraftingStep(_factory);
                AfterSelected = afterSelected;
                afterList.Insert(afterIndex + 1, afterSelected);
            }
            else
            {
                var afterSelected = new InsertCraftingStep(_factory);
                selectedList.Add(afterSelected);
                AfterSelected = afterSelected;
            }

            if (selected.HasChildren && !selected.Children.Any())
            {
                var insideSelected = new InsertCraftingStep(_factory);
                InsideSelected = insideSelected;
                selected.Children.Add(insideSelected);
            }
        }

        public double GetCurrencySpent()
        {
            var allNodes = CraftingSteps.Map(x => true, x => x.Children);
            return allNodes.OfType<CurrencyCraftingStep>().Sum(x => x.CurrencyUsed);
        }

        public void Replace(InsertCraftingStep selected, string name)
        {
            int selectedIndex = -1;
            IList<ICraftingStep> selectedList = null;

            Action<ICraftingStep, int, IList<ICraftingStep>> action = (current, index, list) =>
            {
                if (current == selected)
                {
                    selectedIndex = index;
                    selectedList = list;
                }
            };

            IterateSteps(action, CraftingSteps);

            if (selectedList != null && selectedIndex != -1)
            {
                ICraftingStep craftingStep = null;

                switch (name)
                {
                    case "End":
                        craftingStep = new EndCraftingStep();
                        break;
                    case "If":
                        craftingStep = new IfCraftingStep();
                        break;
                    case "While":
                        craftingStep = new WhileCraftingStep();
                        break;
                    case "Start":
                        craftingStep = new StartCraftingStep();
                        break;
                }

                if (craftingStep == null && _factory.Currency.Any(x => x.Name == name))
                {
                    var currency = _factory.GetCurrencyByName(name);
                    craftingStep = new CurrencyCraftingStep(currency);
                }

                selectedList.RemoveAt(selectedIndex);
                selectedList.Insert(selectedIndex, craftingStep);

                Select(craftingStep);
            }

        }

        private void RemoveSelected()
        {
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
                var isRemoved = Remove(toRemove, craftingStep.Children);
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
