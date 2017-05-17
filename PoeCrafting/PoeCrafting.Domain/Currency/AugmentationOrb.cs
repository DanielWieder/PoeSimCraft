using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class AugmentationOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Augmentation";

        public AugmentationOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic || item.Prefixes.Count + item.Suffixes.Count != 1)
            {
                return false;
            }

            if (item.Suffixes.Count == 1)
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
            return status.MaxPrefixes + status.MaxPrefixes == 2;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity != EquipmentRarity.Magic || status.IsCorrupted || status.MinPrefixes + status.MinSuffixes == 2;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
                return status;

            status.MinPrefixes = 1;
            status.MinSuffixes = 1;
            status.MaxPrefixes = 1;
            status.MinSuffixes = 1;
            status.MinAffixes = 2;
            status.MaxAffixes = 2;

            return status;
        }
    }
}
