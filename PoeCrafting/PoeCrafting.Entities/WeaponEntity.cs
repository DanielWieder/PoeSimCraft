using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    public class WeaponEntity
    {
        public int Level;
        public int Int;
        public int Dex;
        public int Str;
        public int MinDamage;
        public int MaxDamage;
        public double APS;
        public double CritChance;
        public double DPS;
        public string Name { get; set; }
        public string Subtype { get; set; }
        public string Type { get; set; }
    }
}
