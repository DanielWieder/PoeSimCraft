

using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Entities;

namespace PoeCrafting.Entities
{
    public class ItemStatus : ICloneable, ITreeNavigation
    {
        public bool Initialized = false;

        public int MinPrefixes = 0;
        public int MaxPrefixes = 0;
        public int MinSuffixes = 0;
        public int MaxSuffixes = 0;
        public int MinAffixes = 0;
        public int MaxAffixes = 0; 

        public bool IsCorrupted = false;

        public bool HasImplicit = false;

        public bool Completed { get; set; } = false;

        public EquipmentRarity Rarity = EquipmentRarity.Normal;

        public bool Validate()
        {
            return Initialized &&
                   MinPrefixes <= MaxPrefixes &&
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
                Rarity = Rarity,
                Initialized = Initialized,
                HasImplicit = HasImplicit,
                IsCorrupted = IsCorrupted,
                MaxAffixes = MaxAffixes,
                MaxPrefixes = MaxPrefixes,
                MaxSuffixes = MaxSuffixes,
                MinPrefixes = MinPrefixes,
                MinSuffixes = MinSuffixes,
                MinAffixes = MinAffixes,
                Completed = Completed
            };
        }

        public static ItemStatus Combine(List<ItemStatus> status)
        {
            if (status.Any(x => x.Completed))
            {
                throw new InvalidOperationException("Unable to combine completed item statuses. This should have already completed");
            }

            if (status.Any(x => !x.Initialized))
            {
                throw new InvalidOperationException("Unable to combine invalid item statuses. This should not have been reached");
            }

            return new ItemStatus
            {
                Completed = false,
                Initialized = true,
                Rarity = status.Select(x => x.Rarity).Aggregate((x, y) => x | y),
                HasImplicit = status.Any(x => x.HasImplicit),
                IsCorrupted = status.Any(x => x.IsCorrupted),
                MaxAffixes = status.Max(x => x.MaxAffixes),
                MaxPrefixes = status.Max(x => x.MaxPrefixes),
                MaxSuffixes = status.Max(x => x.MaxSuffixes),
                MinPrefixes = status.Min(x => x.MinPrefixes),
                MinSuffixes = status.Min(x => x.MinSuffixes),
                MinAffixes = status.Min(x => x.MinAffixes)
            };
        }

        public bool AreEqual(ItemStatus other)
        {
            return
                Initialized == other.Initialized &&
                Completed == other.Completed &&
                Rarity == other.Rarity && 
                HasImplicit == other.HasImplicit &&
                IsCorrupted == other.IsCorrupted &&
                MaxAffixes == other.MaxAffixes &&
                MaxPrefixes == other.MaxPrefixes &&
                MaxSuffixes == other.MaxSuffixes &&
                MinPrefixes == other.MinPrefixes &&
                MinSuffixes == other.MinSuffixes &&
                MinAffixes == other.MinAffixes;
        }
    }
}
