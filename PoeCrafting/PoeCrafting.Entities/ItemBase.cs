using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCrafting.Entities
{
    public class ItemBase : ICloneable
    {
        public string Name { get; set; }
        public string Subtype { get; set; }
        public string Type { get; set; }

        public Dictionary<string, double> Properties = new Dictionary<string, double>();

        public object Clone()
        {
            return new ItemBase()
            {
                Name = this.Name,
                Subtype = this.Subtype,
                Type = this.Type,
                Properties = Properties.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}
