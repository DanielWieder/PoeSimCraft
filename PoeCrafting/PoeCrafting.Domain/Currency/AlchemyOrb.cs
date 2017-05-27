using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Ninject;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class AlchemyOrb : ICurrency
    {
        private IRandom Random { get; set; }
        private ICurrency Chaos { get; set; }

        public string Name => "Orb of Alchemy";
        public double Value { get; set; }


        public AlchemyOrb(IRandom random)
        {
            Random = random;
            Chaos = new ChaosOrb(random);
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;
            return Chaos.Execute(item);
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
            if (IsError(status))
                return status;

            status.Rarity = EquipmentRarity.Rare;

            return Chaos.GetNextStatus(status);
        }
    }
}
