using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Crafting
{
    public class CraftingCondition
    {
        public bool MeetsCondition(Equipment item)
        {
            return true;
        }
    }
}
