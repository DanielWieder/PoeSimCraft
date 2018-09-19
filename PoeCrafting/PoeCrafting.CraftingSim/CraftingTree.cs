using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PoeCrafting.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using ItemStatus = PoeCrafting.Entities.ItemStatus;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingTree
    {
        public enum InsertPosition
        {
            Before,
            After, 
            Inside
        }

        private readonly CurrencyFactory _factory;

        public List<ICraftingStep> CraftingSteps { get; set; }

        public CraftingTree(CurrencyFactory factory)
        {
            _factory = factory;
            CraftingSteps = new List<ICraftingStep>();

            var start = new StartCraftingStep();
            var end = new EndCraftingStep();
            CraftingSteps.Add(start);
            CraftingSteps.Add(end);
        }

        public void Initialize()
        {
            ClearConditions();
            ClearCurrencySpent();
        }

        public Equipment Craft(Equipment equipment, CancellationToken ct)
        {

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item, ct), ct);
            return equipment;
        }

        public double GetCurrencySpent(int scourCount, int baseItemCount, double baseItemCost)
        {
            double totalCraftingCost = 0;
            Action<int, IList<ICraftingStep>> action = (index, list) =>
            {
                if (list[index] is CurrencyCraftingStep)
                {
                    var currencyCraftingStep = list[index] as CurrencyCraftingStep;
                    totalCraftingCost += currencyCraftingStep.Value*currencyCraftingStep.CurrencyTracker.SuccessfulUsesCount;
                }
            };

            Iterate(action, CraftingSteps);

            var totalScourCost = scourCount * _factory.Currency.First(x => x.Name == CurrencyNames.ScouringOrb).Value;
            var totalBaseCost = baseItemCost * baseItemCount;

            return totalCraftingCost + totalScourCost + totalBaseCost;
        }

        public double ClearCurrencySpent()
        {
            double total = 0;
            Action<int, IList<ICraftingStep>> action = (index, list) =>
            {
                if (list[index] is CurrencyCraftingStep)
                {
                    var currencyCraftingStep = list[index] as CurrencyCraftingStep;
                    currencyCraftingStep.CurrencyTracker.Clear();
                }
            };

            Iterate(action, CraftingSteps);

            return total;
        }

        public void Insert(ICraftingStep neighbor, InsertPosition position, String name)
        {
            if (neighbor == null)
            {
                throw new ArgumentNullException(nameof(neighbor));
            }

            // The start node must be the first
            if (position == InsertPosition.Before && neighbor.Name == "Start")
            {
                throw new InvalidOperationException("Unable to insert a new node before the start node");
            }

            // The end node must be the last
            if (position == InsertPosition.After && neighbor.Name == "End")
            {
                throw new InvalidOperationException("Unable to insert a new node after the end node");
            }

            // Only if/while nodes can have children
            if (position == InsertPosition.Inside && (neighbor.Name != "If" || neighbor.Name != "While"))
            {
                throw new InvalidOperationException("Unable to insert a new node inside a " + neighbor.Name + "node");
            }

            if (position == InsertPosition.Inside)
            {
                neighbor.Children.Insert(0, CreateCraftingStep(name));
            }
            else
            {
                Func<int, IList<ICraftingStep>, bool> action = (index, list) =>
                {
                    if (list[index] == neighbor)
                    {
                        if (position == InsertPosition.Before)
                        {
                            list.Insert(index, CreateCraftingStep(name));
                        }
                        else if (position == InsertPosition.After)
                        {
                            list.Insert(index + 1, CreateCraftingStep(name));
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                "Unable to insert the crafting step. Invalid position.");
                        }
                        return true;

                    }
                    return false;
                };

                IterateUntilCompleted(action, CraftingSteps);

            }
        }

        public void Delete(ICraftingStep toRemove)
        {
            if (toRemove == null || toRemove == CraftingSteps.First() || toRemove == CraftingSteps.Last())
            {
                return;
            }

            Func<int, IList<ICraftingStep>, bool> action = (index, list) =>
            {
                if (list.Contains(toRemove))
                {
                    list.Remove(toRemove);
                    return true;
                }
                return false;
            };
               IterateUntilCompleted(action, CraftingSteps);

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

        private void UpdateStatus()
        {
            Iterate((index, crafingSteps) => CraftingSteps[index].ClearStatus(), CraftingSteps);

            var first = CraftingSteps.First();
            var nextSteps = CraftingSteps.ToList();
            nextSteps.RemoveAt(0);
            first.NavigateTree(new ItemStatus(), nextSteps, (step, item) => step.UpdateStatus(item));
        }

        private ICraftingStep CreateCraftingStep(string name)
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
            return craftingStep;
        }

        private void Iterate(Action<int, IList<ICraftingStep>> action, IList<ICraftingStep> craftingSteps)
        {
            for (int i = 0; i < craftingSteps.Count; i++)
            {
                action(i, craftingSteps);

                if (craftingSteps[i].Children != null && craftingSteps[i].Children.Any())
                {
                    Iterate(action, craftingSteps[i].Children);
                }
            }
        }

        private bool IterateUntilCompleted(Func<int, IList<ICraftingStep>, bool> action, IList<ICraftingStep> craftingSteps)
        {
            for (int i = 0; i < craftingSteps.Count; i++)
            {
                bool isCompleted = action(i, craftingSteps);
                if (isCompleted)
                {
                    return true;
                }

                if (craftingSteps[i].Children != null && craftingSteps[i].Children.Any())
                {
                    isCompleted = IterateUntilCompleted(action, craftingSteps[i].Children);
                    if (isCompleted)
                    {
                        return true;
                    }
                }
            }
            return false;
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
