using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class TransmutationOrb : ICurrency
    {
        private IRandom Random { get; set; }
        private ICurrency Alteration { get; set; }

        public string Name => "Orb of Transmutation";
        public double Value { get; set; }

        public TransmutationOrb(IRandom random)
        {
            Random = random;
            Alteration = new AlterationOrb(random);
        }

        public bool Execute(Equipment item)
        {

            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Magic;

            return Alteration.Execute(item);
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity != EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            status.MinPrefixes = 0;
            status.MaxPrefixes = 1;
            status.MinSuffixes = 0;
            status.MaxSuffixes = 1;
            status.MinAffixes = 1;
            status.MaxAffixes = 2;
            status.Rarity = EquipmentRarity.Magic;
            return status;
        }
    }
}
