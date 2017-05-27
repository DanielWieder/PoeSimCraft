using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    class EndCraftingStep : ICraftingStep
    {
        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public bool HasWarning => false;
        public bool HasError => false;
        public bool IsCompleted => true;
        public string Name => "End";
        public bool HasChildren => false;

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            return equipment;
        }
    }
}
