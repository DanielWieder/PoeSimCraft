using System.Collections.Generic;

namespace PoeCrafting.Entities
{
    public class Equipment : ITreeNavigation
    {
        public bool Completed { get; set; } = false;

        public ItemBase ItemBase { get; set; }

        public List<Affix> PossiblePrefixes { get; set; }
        public List<Affix> PossibleSuffixes { get; set; }
        public List<Affix> PossibleImplicits { get; set; }

        public List<Stat> Prefixes { get; set; } = new List<Stat>();
        public List<Stat> Suffixes { get; set; } = new List<Stat>();
        public Stat Implicit { get; set; } = null;

        public int ItemLevel { get; set; } = 84;

        public EquipmentRarity Rarity { get; set; } = EquipmentRarity.Normal;
        public bool Corrupted { get; set; } = false;
    }
}
