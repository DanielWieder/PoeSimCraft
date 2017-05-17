using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class ScouringOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Scouring";

        public ScouringOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal || item.Rarity == EquipmentRarity.Unique)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Normal;

            item.Prefixes.Clear();
            item.Suffixes.Clear();

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            status.MinPrefixes = 0;
            status.MaxPrefixes = 0;
            status.MinSuffixes = 0;
            status.MaxSuffixes = 0;
            status.MinAffixes = 0;
            status.MaxAffixes = 0;

            status.Rarity = EquipmentRarity.Normal;
            return status;
        }
    }
}
