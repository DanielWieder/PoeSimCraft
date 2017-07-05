﻿using System;
using System.Collections.Generic;
using System.Linq;

using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain.Currency
{
    public class RegalOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => "Regal Orb";
        public double Value { get; set; }

        public RegalOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;

            StatFactory.AddExplicit(Random, item);

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
                status.MaxPrefixes = Math.Min(3, status.MaxPrefixes++);
                status.MaxSuffixes = Math.Min(3, status.MaxSuffixes++);
                status.MaxAffixes = Math.Min(6, status.MaxAffixes++);

                status.Rarity = status.Rarity & ~EquipmentRarity.Magic | EquipmentRarity.Rare;
            }
            else
            {
                status.Rarity = EquipmentRarity.Rare;

                status.MinAffixes++;
                status.MaxPrefixes++;
                status.MaxSuffixes++;
                status.MaxAffixes++;
            }

            return status;
        }
    }
}
