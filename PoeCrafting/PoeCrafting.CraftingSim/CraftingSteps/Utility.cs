using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;

namespace PoeCrafting.CraftingSim.CraftingSteps
{
    public static class Utility
    {
        public static T DefaultNavigateTree<T>(
            this ICraftingStep craftingStep, 
            T item, 
            List<ICraftingStep> queue, 
            Func<ICraftingStep, T, T> action,
            CancellationToken ct = default(CancellationToken))
            where T : ITreeNavigation
        {
            if (item.Completed) return item;
            if (ct.IsCancellationRequested) ct.ThrowIfCancellationRequested();

            var newT = action(craftingStep, item);

            if (queue.Any())
            {
                var nextStep = queue.First();
                queue.RemoveAt(0);

                return nextStep.NavigateTree(newT, queue, action, ct);
            }
            return item;
        }
    }
}
