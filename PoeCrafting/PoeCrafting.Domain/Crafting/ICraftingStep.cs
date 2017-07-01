using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public interface ICraftingStep
    {
        CraftingStepStatus Status { get; }
        string Name { get; }

        List<ICraftingStep> Children { get; }
        CraftingCondition Condition { get; }
        List<string> Options { get; }

        void ClearStatus();
        ItemStatus UpdateStatus(ItemStatus current);
        Equipment Craft(Equipment equipment);
        T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation;
    }
}
