using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingTree
    {
        private readonly CurrencyFactory _factory;

        [JsonIgnore]
        public InsertCraftingStep BeforeSelected { get; set; }
        [JsonIgnore]
        public InsertCraftingStep AfterSelected { get; set; }
        [JsonIgnore]
        public InsertCraftingStep InsideSelected { get; set; }

        public List<ICraftingStep> CraftingSteps { get; set; }

        public CraftingTree(CurrencyFactory factory)
        {
            _factory = factory;
            CraftingSteps = new List<ICraftingStep>();

            var start = new StartCraftingStep();
            var end = new EndCraftingStep();
            CraftingSteps.Add(start);
            CraftingSteps.Add(end);

            Select(start);
        }

        public void Initialize()
        {
            ClearConditions();
            ClearCurrencySpent();
        }

        public Equipment Craft(Equipment equipment, CancellationToken ct)
        {
            RemoveInsertNodes();

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item, ct), ct);
            return equipment;
        }

        public double GetCurrencySpent(int scourCount, int baseItemCount, double baseItemCost)
        {
            double totalCraftingCost = 0;
            Action<ICraftingStep, int, IList<ICraftingStep>> action = (current, index, list) =>
            {
                if (current is CurrencyCraftingStep)
                {
                    var currencyCraftingStep = current as CurrencyCraftingStep;
                    totalCraftingCost += currencyCraftingStep.Value*currencyCraftingStep.Tracker.SuccessfulUsesCount;
                }
            };

            IterateSteps(action, CraftingSteps);

            var totalScourCost = scourCount * _factory.Currency.First(x => x.Name == CurrencyNames.ScouringOrb).Value;
            var totalBaseCost = baseItemCost * baseItemCount;

            return totalCraftingCost + totalScourCost + totalBaseCost;
        }

        public double GetScourCost()
        {
            return _factory.Currency.First(x => x.Name == CurrencyNames.ScouringOrb).Value;
        }

        public double ClearCurrencySpent()
        {
            double total = 0;
            Action<ICraftingStep, int, IList<ICraftingStep>> action = (current, index, list) =>
            {
                if (current is CurrencyCraftingStep)
                {
                    var currencyCraftingStep = current as CurrencyCraftingStep;
                    currencyCraftingStep.Tracker.Clear();
                }
            };

            IterateSteps(action, CraftingSteps);

            return total;
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

            if (selected.Children != null)
            {
                var insideSelected = new InsertCraftingStep(_factory);
                InsideSelected = insideSelected;
                selected.Children.Insert(0, InsideSelected);
            }

            UpdateStatus();

            if (selected.Status == CraftingStepStatus.Unreachable)
            {
                RemoveInsertNodes();
            }
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
            if (craftingStep == null || craftingStep == CraftingSteps.First() || craftingStep == CraftingSteps.Last())
            {
                return;
            }

            Remove(craftingStep, CraftingSteps);
            
            RemoveInsertNodes();
            UpdateStatus();
        }

        public void ClearConditions()
        {
            ClearConditions(CraftingSteps);
        }

        private void ClearConditions(IList<ICraftingStep> craftingSteps)
        {
            if (craftingSteps == null)
            {
                return;
            }

            foreach (var craftingStep in craftingSteps)
            {
                if (craftingStep.GetType() == typeof(IfCraftingStep))
                {
                    (craftingStep as IfCraftingStep).Condition.CraftingSubConditions.Clear();
                }

                if (craftingStep.GetType() == typeof(WhileCraftingStep))
                {
                    (craftingStep as WhileCraftingStep).Condition.CraftingSubConditions.Clear();
                }

                ClearConditions(craftingStep.Children);
            }
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
                if (craftingStep.Children != null)
                {
                    var isRemoved = Remove(toRemove, craftingStep.Children);
                    if (isRemoved)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void IterateSteps(Action<ICraftingStep, int, IList<ICraftingStep>> action, IList<ICraftingStep> craftingSteps)
        {
            for (int i = 0; i < craftingSteps.Count; i++)
            {
                action(craftingSteps[i], i, craftingSteps);

                if (craftingSteps[i].Children != null && craftingSteps[i].Children.Any())
                {
                    IterateSteps(action, craftingSteps[i].Children);
                }
            }
        }

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
