using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Entities
{
    public class Equipment : ITreeNavigation
    {
        [JsonIgnore]
        public bool Completed { get; set; } = false;

        [JsonIgnore]
        public ItemBase ItemBase { get; set; }

        [JsonIgnore]
        public List<Affix> PossibleAffixes { get; set; }

        [JsonIgnore]
        public List<Affix> PossiblePrefixes { get; set; }

        [JsonIgnore]
        public List<Affix> PossibleSuffixes { get; set; }

        [JsonIgnore]
        public List<Stat> Stats { get; set; } = new List<Stat>();
        public List<Stat> Prefixes => Stats.Where(x => x.Affix.Type == TypeInfo.AffixTypePrefix).ToList();
        public List<Stat> Suffixes => Stats.Where(x => x.Affix.Type == TypeInfo.AffixTypeSuffix).ToList();

        public Stat Implicit { get; set; } = null;

        public int ItemLevel { get; set; } = 84;

        public EquipmentRarity Rarity { get; set; } = EquipmentRarity.Normal;
        public bool Corrupted { get; set; } = false;

        [JsonIgnore]
        public int TotalWeight { get; set; }

        [JsonIgnore]
        public int PrefixWeight { get; set; }

        [JsonIgnore]
        public int SuffixWeight { get; set; }
    }
}
