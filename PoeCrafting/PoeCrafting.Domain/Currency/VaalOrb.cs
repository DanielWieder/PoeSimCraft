using System;
using System.Collections.Generic;
using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Currency
{
    public class VaalOrb : ICurrency
    {
        private IRandom Random { get; }
        private ICurrency Chaos { get; }

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
            if (IsError(status))
            {
                return status;
            }

            status.MinPrefixes = Math.Min(1, status.MinPrefixes);
            status.MinSuffixes = Math.Min(1, status.MinSuffixes);
            status.MinAffixes = Math.Min(4, status.MinAffixes);
            status.MaxPrefixes = 3;
            status.MaxSuffixes = 3;
            status.MaxAffixes = 6;

            status.Rarity = status.Rarity |= EquipmentRarity.Rare;
            status.IsCorrupted = true;
            return status;
        }
    }
}
