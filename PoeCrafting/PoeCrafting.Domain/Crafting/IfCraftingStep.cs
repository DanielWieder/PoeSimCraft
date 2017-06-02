using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    public class IfCraftingStep : ICraftingStep
    {
        private bool _initialized = false;

        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();

        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "If";
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

                var nextStatus = (ItemStatus) status.Clone();
                nextStatus = first.NavigateTree(nextStatus, nextSteps, (step, item) => step.UpdateStatus(nextStatus));

                if (nextStatus.Completed)
                {
                    return status;
                }

                return ItemStatus.Combine(new List<ItemStatus> {status, nextStatus});

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
