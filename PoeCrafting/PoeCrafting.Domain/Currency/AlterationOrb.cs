using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class AlterationOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Orb of Alteration";
        public double Value { get; set; }

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
            return !IsError(status) && status.Rarity != EquipmentRarity.Magic;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Magic) != EquipmentRarity.Magic || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(0, status.MinPrefixes);
                status.MinSuffixes = Math.Min(0, status.MinSuffixes);
                status.MinAffixes = Math.Min(2, status.MinAffixes);

                status.MaxPrefixes = Math.Max(1, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(1, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(2, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 1;

                status.MaxPrefixes = 1;
                status.MaxSuffixes = 1;
                status.MaxAffixes = 2;
            }

            return status;
        }
    }
}
