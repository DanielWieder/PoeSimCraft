﻿using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Domain.Currency
{
    public class ExaltedOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.ExaltedOrb;
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

            StatFactory.AddExplicit(Random, item);

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity != EquipmentRarity.Rare || status.MaxPrefixes + status.MaxSuffixes == 6);
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Rare) != EquipmentRarity.Rare || status.MinPrefixes + status.MinSuffixes == 6 || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (status.Rarity != EquipmentRarity.Rare && IsWarning(status))
            {
                status.MaxPrefixes = Math.Max(Math.Min(status.MaxSuffixes + 1, 3), status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(Math.Min(status.MaxPrefixes + 1, 3), status.MaxSuffixes);
                status.MaxAffixes = Math.Max(Math.Min(status.MaxAffixes + 1, 6), status.MaxAffixes);
            }
            else
            {
                status.MinAffixes = Math.Min(status.MinAffixes + 1, 6);
                status.MinPrefixes = Math.Max(status.MinPrefixes, status.MinAffixes - 3);
                status.MinSuffixes = Math.Max(status.MinSuffixes, status.MinAffixes - 3);

                status.MaxAffixes = Math.Min(status.MaxAffixes + 1, 6);
                status.MaxSuffixes = Math.Min(status.MaxSuffixes + 1, 3);
                status.MaxPrefixes = Math.Min(status.MaxPrefixes + 1, 3);
            }

            return status;
        }
    }
}