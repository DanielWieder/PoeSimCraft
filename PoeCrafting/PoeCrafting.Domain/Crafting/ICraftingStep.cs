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
        // X Currency
        // X Start
        // X Stop
        // If
        // While
        // X Insert

        List<ICraftingStep> Children { get; }
        bool HasWarning { get; }
        bool HasError { get; }
        bool IsCompleted { get; }
        string Name { get; }

        ItemStatus UpdateStatus(ItemStatus status);
        Equipment Craft(Equipment equipment);
    }
}
