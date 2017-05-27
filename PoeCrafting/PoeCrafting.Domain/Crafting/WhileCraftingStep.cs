using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    public class WhileCraftingStep : ICraftingStep
    {
        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public bool HasWarning => false;
        public bool HasError => false;
        public bool IsCompleted => false;
        public string Name => "While";
        public bool HasChildren => true;

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
