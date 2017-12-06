using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public interface IItemConfigRepository
    {
        event EventHandler<ConfigUpdatedEventArgs> ConfigChanged;
        void SetItemConfig(ItemConfig config);
        ItemConfig GetItemConfig();
    }
}
