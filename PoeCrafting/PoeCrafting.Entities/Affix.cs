using System.Collections.Generic;

namespace PoeCrafting.Entities
{
    public class Affix
    {
        public int Tier { get; set; }
        public string Name { get; set; }
        public string ModName { get; set; }
        public string ModType { get; set; }
        public int ILvl { get; set; }
        public int Weight { get; set; }

        public int CraftingOrb { get; set; }
        public int CraftingCost { get; set; }

        public string Type { get; set; }
        public string Group { get; set; }

        public int ModTypeWeight { get; set; }

        public string StatName1 { get; set; }
        public int StatMin1 { get; set; }
        public int StatMax1 { get; set; }

        public string StatName2 { get; set; }
        public int StatMin2 { get; set; }
        public int StatMax2 { get; set; }

        public string StatName3 { get; set; }
        public int StatMin3 { get; set; }
        public int StatMax3 { get; set; }

        public string SpawnTag { get; set; }

        public  int Priority { get; set; }

        public List<int> MaxStats
        {
            get
            {
                var list = new List<int>();
                if (!string.IsNullOrEmpty(StatName1))
                {
                    list.Add(StatMax1);
                }
                if (!string.IsNullOrEmpty(StatName2))
                {
                    list.Add(StatMax2);
                }
                if (!string.IsNullOrEmpty(StatName3))
                {
                    list.Add(StatMax3);
                }
                return list;
            }
        }
    }
}
