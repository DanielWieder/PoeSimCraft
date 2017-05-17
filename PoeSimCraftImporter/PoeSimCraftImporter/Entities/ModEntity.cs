using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSimCraftImporter.Entities
{
    public class ModEntity
    {
        public string Name;
        public string ModName;
        public string GenerationType;
        public string Domain;
        public string Group;
        public string LevelReq;
        public List<SpawnTag> SpawnTags;
        public List<Stat> Stats;
        public List<string> Tags;
    }

    public class Stat
    {
        public int Min;
        public int Max;
        public string Id;

    }

    public class SpawnTag
    {
        public string Name;
        public bool Value;
    }
}
