using System;
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
