using System;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain
{
    public class DataRepository : IItemConfigRepository
    {
        public event EventHandler<ConfigUpdatedEventArgs> ConfigChanged;

        private ItemConfig _config = new ItemConfig();
        public DataRepository()
        {}

        public void SetItemConfig(ItemConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (!config.Equals(_config))
            {
                _config = config;
                ConfigChanged?.Invoke(this, new ConfigUpdatedEventArgs(config));
            }
        }

        public ItemConfig GetItemConfig()
        {
            return (ItemConfig)_config.Clone();
        }


    }
}
