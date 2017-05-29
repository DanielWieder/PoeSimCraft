using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public interface ICraftingStep
    {
        List<ICraftingStep> Children { get; }
        CraftingStepStatus Status { get; }
        bool HasChildren { get; }
        string Name { get; }

        void ClearStatus();
        ItemStatus UpdateStatus(ItemStatus current);
        Equipment Craft(Equipment equipment);
        T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation;
    }
}
