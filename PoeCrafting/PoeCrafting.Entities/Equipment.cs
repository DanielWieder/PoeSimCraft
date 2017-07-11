using System.Collections.Generic;
using System.Linq;

namespace PoeCrafting.Entities
{
    public class Equipment : ITreeNavigation
    {
        public bool Completed { get; set; } = false;

        public ItemBase ItemBase { get; set; }

        public List<Affix> PossibleAffixes { get; set; }
        public List<Affix> PossiblePrefixes { get; set; }
        public List<Affix> PossibleSuffixes { get; set; }

        public List<Stat> Stats { get; set; } = new List<Stat>();
        public List<Stat> Prefixes => Stats.Where(x => x.Affix.Type == "prefix").ToList();
        public List<Stat> Suffixes => Stats.Where(x => x.Affix.Type == "suffix").ToList();

        public Stat Implicit { get; set; } = null;

        public int ItemLevel { get; set; } = 84;

        public EquipmentRarity Rarity { get; set; } = EquipmentRarity.Normal;
        public bool Corrupted { get; set; } = false;
        public int TotalWeight { get; set; }
    }
}
