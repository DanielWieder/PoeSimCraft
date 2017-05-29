using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class DivineOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => "Divine Orb";
        public double Value { get; set; }

        public DivineOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return false;
            }

            foreach (var prefix in item.Prefixes)
            {
                StatFactory.Reroll(Random, prefix);
            }

            foreach (var suffix in item.Prefixes)
            {
                StatFactory.Reroll(Random, suffix);
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity | EquipmentRarity.Normal) == EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            return status;
        }
    }
}
