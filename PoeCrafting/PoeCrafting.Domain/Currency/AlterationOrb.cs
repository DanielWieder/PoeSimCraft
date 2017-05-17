using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class AlterationOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Alteration";

        public AlterationOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return false;
            }

            item.Prefixes.Clear();
            item.Suffixes.Clear();

            var roll = Random.Next(3);

            if (roll == 0 || roll == 2)
            {
                item.Prefixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Prefixes, item.ItemLevel));
            }
            if(roll == 1 || roll == 2)
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

            status.MinPrefixes = 0;
            status.MinSuffixes = 0;
            status.MaxPrefixes = 1;
            status.MaxSuffixes = 1;
            status.MinAffixes = 1;
            status.MaxAffixes = 2;

            return status;
        }
    }
}
