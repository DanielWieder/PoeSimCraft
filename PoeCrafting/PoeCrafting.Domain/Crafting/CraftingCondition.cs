using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingCondition : ICraftingCondition
    {
        public bool IsValid(Equipment item)
        {
            return true;
        }
    }

    public interface ICraftingCondition
    {
        bool IsValid(Equipment item);
    }
}
