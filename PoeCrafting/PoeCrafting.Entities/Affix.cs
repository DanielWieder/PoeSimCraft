using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoeCrafting.Entities
{
    public class Affix
    {
        /// <summary>
        /// The strength of the mod compared to others of the same type
        /// </summary>
        public int Tier { get; set; }

        /// <summary>
        /// The name of the affix. This is what get added to the name of magic items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string ModName { get; set; }

        /// <summary>
        /// Mods of the same type do the same thing and are exclusive among each other
        /// </summary>
        public string ModType { get; set; }

        /// <summary>
        /// The minimum item level for the affix to spawn
        /// </summary>
        public int ILvl { get; set; }

        /// <summary>
        /// The frequency that the affix occurs
        /// </summary>
        [JsonIgnore]
        public int Weight { get; set; }

        /// <summary>
        /// The total weight of all mods of that type
        /// </summary>
        [JsonIgnore]
        public int ModTypeWeight { get; set; }

        /// <summary>
        /// Where the goes. (Prefix, Suffix, Corruption)
        /// </summary>
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

        /// <summary>
        /// If the affix belongs to a faction then the item must belong to the faction in order to spawn it (Elder/Shaper)
        /// </summary>
        public int Faction { get; set; }

        [JsonIgnore]
        public string SpawnTag { get; set; }

        [JsonIgnore]
        public int Priority { get; set; }

        [JsonIgnore]
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
