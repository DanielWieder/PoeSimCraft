namespace PoeCrafting.Entities
{
    public class Affix
    {
        public int Tier { get; set; }
        public string Name { get; set; }
        public string ModName { get; set; }
        public int ILvl { get; set; }
        public int Weight { get; set; }

        public int CraftingOrb { get; set; }
        public int CraftingCost { get; set; }

        public string Type { get; set; }
        public string Group { get; set; }

        public string StatName1 { get; set; }
        public int StatMin1 { get; set; }
        public int StatMax1 { get; set; }

        public string StatName2 { get; set; }
        public int StatMin2 { get; set; }
        public int StatMax2 { get; set; }

        public string StatName3 { get; set; }
        public int StatMin3 { get; set; }
        public int StatMax3 { get; set; }
    }
}
