using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
