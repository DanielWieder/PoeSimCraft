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
    public class ChanceOrb : ICurrency
    {
        private IRandom Random { get; set; }
        private ICurrency Alchemy { get; set; }
        private ICurrency Transmutation { get; set; }

        public string Name => "Orb of Chance";
        public double Value { get; set; }

        public ChanceOrb(IRandom random)
        {
            Random = random;
            Alchemy = new AlchemyOrb(random);
            Transmutation = new TransmutationOrb(random);
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            // Unique items are not currently handled
            var roll = Random.Next(5);

            if (roll == 0)
            {
                return Alchemy.Execute(item);
            }
            else
            {
                return Transmutation.Execute(item);
            }
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
            // Unable to set the ItemStatus rarity.
            throw new NotImplementedException();
        }
    }
}
