using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class StartCraftingStep : ICraftingStep
    {
        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public CraftingStepStatus Status => _initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;

        public string Name => "Start";
        public bool HasChildren => false;

        private bool _initialized = false;

        public void ClearStatus()
        {
            _initialized = false;
        }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            _initialized = true;
            status.Initialized = true;
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
