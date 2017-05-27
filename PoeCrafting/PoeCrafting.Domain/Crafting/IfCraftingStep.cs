using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    class IfCraftingStep : ICraftingStep
    {
        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public bool HasWarning => false;
        public bool HasError => false;
        public bool IsCompleted => false;
        public string Name => "If";
        public bool HasChildren => true;

        public CraftingCondition Condition { get; set; }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            return new ItemStatus();
        }

        public Equipment Craft(Equipment equipment)
        {
            return equipment;
        }
    }
}
