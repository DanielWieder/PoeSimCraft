﻿using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class ScouringOrb : ICurrency
    {
        public string Name => "Orb of Scouring";
        public double Value { get; set; }

        public ScouringOrb(IRandom random)
        {
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal || item.Rarity == EquipmentRarity.Unique)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Normal;

            item.Prefixes.Clear();
            item.Suffixes.Clear();

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity & EquipmentRarity.Normal) == EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted;
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
                status.MinAffixes = Math.Min(0, status.MinAffixes);

                status.Rarity = status.Rarity & EquipmentRarity.Magic & EquipmentRarity.Rare | EquipmentRarity.Normal;
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 0;

                status.MaxPrefixes = 0;
                status.MaxSuffixes = 0;
                status.MaxAffixes = 0;

                status.Rarity = EquipmentRarity.Normal;
            }

            return status;
        }
    }
}
