using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    public class ConditionResolution
    {
        public bool IsPresent { get; set; }
        public bool IsMatch { get; set; }
        public List<int> Values { get; set; }
    }
}
