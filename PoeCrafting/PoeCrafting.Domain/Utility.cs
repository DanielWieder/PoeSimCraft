using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
namespace PoeCrafting.Domain
{
    public static class Utility
    {
        /// Traverses an object hierarchy and return a flattened list of elements
        /// based on a predicate.
        /// 
        /// TSource: The type of object in your collection.</typeparam>
        /// source: The collection of your topmost TSource objects.</param>
        /// selectorFunction: A predicate for choosing the objects you want.
        /// getChildrenFunction: A function that fetches the child collection from an object.
        /// returns: A flattened list of objects which meet the criteria in selectorFunction.
        public static IEnumerable<TSource> Map<TSource>(
          this IEnumerable<TSource> source,
          Func<TSource, bool> selectorFunction,
          Func<TSource, IEnumerable<TSource>> getChildrenFunction)
        {
            // Add what we have to the stack
            var flattenedList = source.Where(selectorFunction);

            // Go through the input enumerable looking for children,
            // and add those if we have them
            foreach (TSource element in source)
            {
                flattenedList = flattenedList.Concat(
                  getChildrenFunction(element).Map(selectorFunction,
                                                   getChildrenFunction)
                );
            }
            return flattenedList;
        }

        public static T DefaultNavigateTree<T>(
            this ICraftingStep craftingStep, 
            T item, 
            List<ICraftingStep> queue, 
            Func<ICraftingStep, T, T> action,
            CancellationToken ct = default(CancellationToken))
            where T : ITreeNavigation
        {
            if (item.Completed) return item;

            if (ct.IsCancellationRequested)
            {
                ct.ThrowIfCancellationRequested();
            }

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
