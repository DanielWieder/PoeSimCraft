using PoeCrafting.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Domain.Condition
{
    public class CraftingCondition
    {
        public List<CraftingSubcondition> CraftingSubConditions = new List<CraftingSubcondition>();

        public bool IsValid(Equipment item)
        {
            foreach (var subcondition in CraftingSubConditions)
            {
                if (!subcondition.IsValid(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
