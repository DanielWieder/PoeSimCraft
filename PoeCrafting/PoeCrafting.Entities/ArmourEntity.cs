namespace PoeCrafting.Entities
{
    public class ArmourEntity : ItemBase
    {
        public string Name { get; set; }
        public string Subtype { get; set; }
        public string Type { get; set; }

        public int Level { get; set; }
        public int Int { get; set; }
        public int Dex { get; set; }
        public int Str { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }
        public int EnergyShield { get; set; }

        public int BlockChance { get; set; }
    }
}
