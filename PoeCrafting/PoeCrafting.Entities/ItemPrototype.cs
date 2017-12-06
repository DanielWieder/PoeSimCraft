using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    public class ItemPrototype
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public CraftingCondition Condition { get; set; }

    }
}
