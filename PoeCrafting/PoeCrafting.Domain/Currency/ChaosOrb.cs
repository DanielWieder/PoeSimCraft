using System;
using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Currency
{
    public class ChaosOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => "Chaos Orb";
        public double Value { get; set; }

        public ChaosOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Rare)
            {
                return false;
            }

            item.Prefixes.Clear();
            item.Suffixes.Clear();

            // This is the number of times I observed the following number of mods of the mods during my testing. I will need to do more tests later but this is a halfway decent baseline
            // For the moment I'm just going to use these numbers for generation. It will do for now even if it's a really small sample size
            // Number of Mods: Times Seen
            // 4: 72
            // 5: 41
            // 6: 13

            item.Prefixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Prefixes, item.ItemLevel));
            item.Suffixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Suffixes, item.ItemLevel));

            int fourMod = 72;
            int fiveMod = 41;
            int sixMod = 13;

            var sum = fourMod + fiveMod + sixMod;

            var roll = Random.Next(sum);
            int modCount = roll < fourMod ? 4 : 
                           roll < fourMod + fiveMod ? 5 : 
                           6;

            for (int i = 2; i < modCount; i++)
            {
                if ( item.Prefixes.Count < 3 && (item.Suffixes.Count >= 3 ||Random.Next(2) == 0))
                {
                    item.Prefixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Prefixes, item.ItemLevel));
                }
                else if (item.Suffixes.Count < 3)
                {
                    item.Suffixes.Add(StatFactory.Get(Random, item.PossiblePrefixes, item.Suffixes, item.ItemLevel));
                }
                else
                {
                    throw new InvalidOperationException("Unable to add an additional mod to the item. It already has 3 suffixes and 3 prefixes");
                }
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Rare;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Rare) != EquipmentRarity.Rare || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(1, status.MinPrefixes);
                status.MinSuffixes = Math.Min(1, status.MinSuffixes);
                status.MinAffixes = Math.Min(4, status.MinAffixes);

                status.MaxPrefixes = Math.Max(3, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(3, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(6, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 1;
                status.MinSuffixes = 1;
                status.MinAffixes = 4;

                status.MaxPrefixes = 3;
                status.MaxSuffixes = 3;
                status.MaxAffixes = 6;
            }

            return status;
        }
    }
}
