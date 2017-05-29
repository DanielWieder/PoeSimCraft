using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    [Flags]
    public enum CraftingStepStatus
    {
        Unreachable,
        Unusable,
        Inconsistent,
        Ok
    }
}
