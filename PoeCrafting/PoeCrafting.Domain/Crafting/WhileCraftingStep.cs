using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    public class WhileCraftingStep : ICraftingStep
    {
        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();
        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "While";
        private bool _initialized = false;
        public bool HasChildren => true;
        public List<string> Options => new List<string>();
        public ICraftingCondition Condition { get; set; }

        public void ClearStatus()
        {
            _initialized = false;
        }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            if (status.Completed) return status;

            _initialized = true;

            if (Children.Count > 0)
            {
                var first = Children.First();
                var nextSteps = Children.ToList();
                nextSteps.RemoveAt(0);

                ItemStatus endStatus = (ItemStatus) status.Clone();
                bool isStatusEquilibrium = false;

                // We do not know how many times this loop will run for 
                // so we combine the status of each iteration until it reaches equilibrium
                // Combining status always take the widest value within the max/min so it should always converge.
                while (!isStatusEquilibrium)
                {
                    var nextStatus = first.NavigateTree((ItemStatus) endStatus.Clone(), nextSteps,
                        (step, item) => step.UpdateStatus(status));

                    if (nextStatus.Completed)
                    {
                        isStatusEquilibrium = true;
                    }
                    else
                    {

                        var combinedStatus = ItemStatus.Combine(new List<ItemStatus> {endStatus, nextStatus});
                        if (endStatus.AreEqual(combinedStatus))
                        {
                            isStatusEquilibrium = true;
                        }
                        else
                        {
                            endStatus = combinedStatus;
                        }
                    }
                }
                return endStatus;
            }
            else
            {
                return status;
            }
        }

        public Equipment Craft(Equipment equipment)
        {
            if (equipment.Completed) return equipment;

            if (Children.Count > 0)
            {
                var first = Children.First();
                var nextSteps = Children.ToList();
                nextSteps.RemoveAt(0);
                while (Condition.IsValid(equipment))
                {
                    first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item));
                }
            }
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
