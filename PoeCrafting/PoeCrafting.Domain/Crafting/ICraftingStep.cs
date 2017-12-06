using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public interface ICraftingStep
    {
        [JsonIgnore]
        CraftingStepStatus Status { get; }
        string Name { get; }

        List<ICraftingStep> Children { get; }
        CraftingCondition Condition { get; }

        [JsonIgnore]
        List<string> Options { get; }

        void ClearStatus();
        ItemStatus UpdateStatus(ItemStatus current);
        Equipment Craft(Equipment equipment, CancellationToken ct);
        T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action, CancellationToken ct = default(CancellationToken)) where T : ITreeNavigation;
    }
}
