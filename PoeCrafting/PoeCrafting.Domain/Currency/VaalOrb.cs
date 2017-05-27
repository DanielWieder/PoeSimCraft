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
    public class VaalOrb : ICurrency
    {
        private IRandom Random { get; set; }
        private ICurrency Chaos { get; set; }

        public string Name => "Vaal Orb";
        public double Value { get; set; }

        public VaalOrb(IRandom random)
        {
            Random = random;
            Chaos = new ChaosOrb(random);
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted)
            {
                return false;
            }

            var roll = Random.Next(4);

            if (roll == 0)
            {
                item.Implicit = StatFactory.Get(Random, item.PossibleImplicits, new List<Stat>(), item.ItemLevel);
            }
            if (roll == 1)
            {
                item.Rarity = EquipmentRarity.Rare;

                return Chaos.Execute(item);
            }

            item.Corrupted = true;

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            status.IsCorrupted = true;
            return status;
        }
    }
}
