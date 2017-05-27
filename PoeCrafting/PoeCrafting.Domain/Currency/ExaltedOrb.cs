﻿using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class ExaltedOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => "Exalted Orb";
        public double Value { get; set; }

        public ExaltedOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.PossiblePrefixes.Count == 0 || item.PossiblePrefixes.Count == 0)
            {
                throw new InvalidOperationException("The item has no available prefixes or suffixes");
            }

            if (item.Corrupted || item.Rarity != EquipmentRarity.Rare || (item.Prefixes.Count >= 3 && item.Suffixes.Count >= 3))
            {
                return false;
            }

            if (item.Suffixes.Count >= 3)
            {
                item.Prefixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Prefixes, item.ItemLevel));
            }
            else if (item.Prefixes.Count >= 3)
            {
                item.Suffixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Suffixes, item.ItemLevel));
            }
            else if (Random.Next(2) == 0)
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
            return status.MaxPrefixes + status.MaxSuffixes == 6;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity != EquipmentRarity.Rare || status.MinPrefixes + status.MinSuffixes == 6 || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
                return status;

            status.MaxSuffixes = Math.Min(status.MaxSuffixes + 1, 3);
            status.MaxPrefixes = Math.Min(status.MaxPrefixes + 1, 3);

            status.MinAffixes = Math.Min(status.MinAffixes + 1, 6);
            status.MaxAffixes = Math.Min(status.MaxAffixes + 1, 6);

            status.MinPrefixes = Math.Min(status.MinPrefixes + 1, status.MinAffixes - 3);
            status.MinSuffixes = Math.Min(status.MinSuffixes + 1, status.MinAffixes - 3);

            return status;
        }
    }
}