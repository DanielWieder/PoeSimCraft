using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;

namespace PoeCrafting.Domain.Crafting
{
    public class WhileCraftingStep : ICraftingStep
    {
        private bool _initialized = false;

        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "While";

        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();
        public CraftingCondition Condition { get; } = new CraftingCondition();
        public List<string> Options => null;

        ConditionResolver _conditionResolution = new ConditionResolver();

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

                ItemStatus endStatus = (ItemStatus) status.Clone();
                bool isStatusEquilibrium = false;

                // We do not know how many times this loop will run for 
                // so we combine the status of each iteration until it reaches equilibrium
                // Combining status always take the widest value within the max/min so it should always converge.
                while (!isStatusEquilibrium)
                {
                    var nextSteps = Children.ToList();
                    nextSteps.RemoveAt(0);

                    var lastStatus = first.NavigateTree((ItemStatus) endStatus.Clone(), nextSteps,
                        (step, nextStatus) => step.UpdateStatus(nextStatus));

                    if (lastStatus.Completed)
                    {
                        isStatusEquilibrium = true;
                    }
                    else
                    {

                        var combinedStatus = ItemStatus.Combine(new List<ItemStatus> {endStatus, lastStatus });
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

        public Equipment Craft(Equipment equipment, CancellationToken ct)
        {
            if (equipment.Completed) return equipment;

            if (Children.Count > 0)
            {
                var first = Children.First();
                while (_conditionResolution.IsValid(Condition, equipment))
                {
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }

                    if (equipment.Completed)
                    {
                        return equipment;
                    }

                    var nextSteps = Children.ToList();
                    nextSteps.RemoveAt(0);
                    first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item, ct), ct);
                }
            }
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action, CancellationToken ct) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action, ct);
        }
    }
}
