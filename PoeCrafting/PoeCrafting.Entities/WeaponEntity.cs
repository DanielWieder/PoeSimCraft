using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    public class WeaponEntity : ItemBase
    {
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Str { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public double APS { get; set; }
        public double Crit { get; set; }
        public double DPS { get; set; }
    }
}
