using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Entities
{
    public class ConfigUpdatedEventArgs
    {
        public ConfigUpdatedEventArgs(ItemConfig config)
        {
            this.Config = config;
        }

        private ItemConfig Config { get; set; }
    }
}
