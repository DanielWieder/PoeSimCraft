using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class RegalOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Regal";

        public RegalOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;

            if (Random.Next(2) == 0)
            {
                item.Prefixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Prefixes, item.ItemLevel));
            }
            else
            {
                item.Suffixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Suffixes, item.ItemLevel));
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity != EquipmentRarity.Magic || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
                return status;

            status.MaxPrefixes++;
            status.MaxSuffixes++;
            status.MinAffixes++;
            status.MaxAffixes++;

            status.Rarity = EquipmentRarity.Rare;

            return status;
        }
    }
}
