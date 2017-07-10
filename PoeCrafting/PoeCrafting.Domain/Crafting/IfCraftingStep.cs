using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;

namespace PoeCrafting.Domain.Crafting
{
    public class IfCraftingStep : ICraftingStep
    {
        private bool _initialized = false;

        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "If";

        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();
        public CraftingCondition Condition { get; set; } = new CraftingCondition();
        public List<string> Options => null;

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

                var initialStatus = (ItemStatus) status.Clone();
                var endStatus = first.NavigateTree(initialStatus, nextSteps, (step, nextStatus) => step.UpdateStatus(nextStatus));

                if (endStatus.Completed)
                {
                    return status;
                }

                return ItemStatus.Combine(new List<ItemStatus> {status, endStatus });

            }
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            if (equipment.Completed) return equipment;

            if (Children.Count > 0 && Condition.IsValid(equipment))
            {
                var first = Children.First();
                var nextSteps = Children.ToList();
                nextSteps.RemoveAt(0);

                first.NavigateTree(equipment, nextSteps, (step, item) => step.Craft(item));
            }
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
