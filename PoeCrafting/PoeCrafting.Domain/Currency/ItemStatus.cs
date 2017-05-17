

using System;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class ItemStatus : ICloneable
    {
        public int MinPrefixes = 0;
        public int MaxPrefixes = 0;
        public int MinSuffixes = 0;
        public int MaxSuffixes = 0;
        public int MinAffixes = 0;
        public int MaxAffixes = 0;

        public int MinCraftedPrefixes = 0;
        public int MaxCraftedPrefixes = 0;
        public int MinCraftedSuffixes= 0;
        public int MaxCraftedSuffixes = 0;

        public bool IsCorrupted = false;

        public bool HasImplicit = false;

        public EquipmentRarity Rarity = EquipmentRarity.Normal;

        public bool Validate()
        {
            return MinPrefixes <= MaxPrefixes &&
                   MinSuffixes <= MaxSuffixes &&
                   MinAffixes <= MaxAffixes &&
                   MinPrefixes <= MinAffixes &&
                   MinSuffixes <= MinAffixes &&
                   MaxPrefixes <= 3 &&
                   MaxSuffixes <= 3 &&
                   MaxAffixes <= 6;
        }

        public object Clone()
        {
            return new ItemStatus
            {
                HasImplicit = HasImplicit,
                IsCorrupted = IsCorrupted,
                MaxAffixes = MaxAffixes,
                MaxCraftedPrefixes = MaxCraftedPrefixes,
                MaxCraftedSuffixes = MaxCraftedSuffixes,
                MaxPrefixes = MaxPrefixes,
                MaxSuffixes = MaxSuffixes,
                MinAffixes = MinAffixes,
                MinCraftedPrefixes = MinCraftedPrefixes,
                MinCraftedSuffixes = MinCraftedSuffixes,
            };
        }
    }
}
