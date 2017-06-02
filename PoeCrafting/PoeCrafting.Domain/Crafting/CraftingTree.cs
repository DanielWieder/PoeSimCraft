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

        public InsertCraftingStep BeforeSelected { get; set; }
        public InsertCraftingStep AfterSelected { get; set; }
        public InsertCraftingStep InsideSelected { get; set; }
        public InsertCraftingStep LastAfterConditional { get; set; }

        public List<ICraftingStep> CraftingSteps { get; set; }

        public CraftingTree(CurrencyFactory factory)
        {
            _factory = factory;

            CraftingSteps = new List<ICraftingStep>();

            var start = new StartCraftingStep();
            CraftingSteps.Add(start);

            Select(start);
        }

        public Equipment Craft(Equipment equipment)
        {
            RemoveInsertNodes();

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

            RemoveInsertNodes();

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

            var afterIndex = selectedIndex + 1;

            if (selected != CraftingSteps.First())
            {
                var beforeSelected = new InsertCraftingStep(_factory);
                BeforeSelected = beforeSelected;
                selectedList.Insert(selectedIndex, beforeSelected);
                afterIndex++;
            }

            if (selected.Name != "End")
            {
                var afterSelected = new InsertCraftingStep(_factory);
                AfterSelected = afterSelected;
                selectedList.Insert(afterIndex, afterSelected);
            }

            if (selected.HasChildren && !selected.Children.Any())
            {
                var insideSelected = new InsertCraftingStep(_factory);
                InsideSelected = insideSelected;
                selected.Children.Add(insideSelected);
            }

            if ( selected == CraftingSteps.Last() && CraftingSteps.Last().HasChildren)
            {
                var lastAfterConditional = new InsertCraftingStep(_factory);
                LastAfterConditional = lastAfterConditional;
                CraftingSteps.Add(lastAfterConditional);
            }

            UpdateStatus();

            if (selected.Status == CraftingStepStatus.Unreachable)
            {
                RemoveInsertNodes();
            }
        }

        public double GetCurrencySpent()
        {
            var allNodes = CraftingSteps.Map(x => true, x => x.Children);
            return allNodes.OfType<CurrencyCraftingStep>().Sum(x => x.CurrencyUsed);
        }

        public void Replace(ICraftingStep selected, string name)
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
            UpdateStatus();
        }

        public void Delete(ICraftingStep craftingStep)
        {
            if (craftingStep == null || craftingStep == CraftingSteps[0])
            {
                return;
            }

            Remove(craftingStep, CraftingSteps);
            
            RemoveInsertNodes();
            UpdateStatus();
        }

        private void RemoveInsertNodes()
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
            if (LastAfterConditional != null)
            {
                Remove(LastAfterConditional, CraftingSteps);
            }
        }

        private void UpdateStatus()
        {
            IterateSteps((craftingStep, x, y) => craftingStep.ClearStatus(), CraftingSteps);

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(new ItemStatus(), nextSteps, (step, item) => step.UpdateStatus(item));
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
