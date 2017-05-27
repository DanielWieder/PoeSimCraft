using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;


namespace PoeCrafting.Domain.Currency
{
    public class MasterCraft : ICurrency
    {
        public Affix Affix { get; set; }

        private IRandom Random { get; set; }

        public string Name => "Master Craft - ";
        public double Value { get; set; }

        public MasterCraft(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return false;
            }

            if (Affix.Type == "prefix")
            {
                return AddAffix(item, Random, Affix, item.PossiblePrefixes, item.Prefixes);
            }

            if (Affix.Type == "suffix")
            {
                return AddAffix(item, Random, Affix, item.PossiblePrefixes, item.Suffixes);
            }

            throw new InvalidOperationException("Implicit affixes cannot be crafted");
        }

        private static bool AddAffix(Equipment item, IRandom random, Affix affix, List<Affix> possibleAffixes, List<Stat> itemAffixes)
        {
            if (itemAffixes.Count >= 3)
            {
                return false;
            }

            if (!possibleAffixes.Contains(affix))
            {
                throw new InvalidOperationException("That crafted prefix is not in the list of available prefixes");
            }

            var stat = StatFactory.Get(random, affix);

            item.Prefixes.Add(stat);
            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            if (Affix.Type.Contains("prefix"))
            {
                return status.Rarity == EquipmentRarity.Magic && status.MaxPrefixes == 1 ||
                    status.Rarity == EquipmentRarity.Rare && status.MaxPrefixes == 3;
            }
            if (Affix.Type.Contains("suffix"))
            {
                return status.Rarity == EquipmentRarity.Magic && status.MaxSuffixes == 1 ||
                    status.Rarity == EquipmentRarity.Rare && status.MaxSuffixes == 3;
            }

            return false;
        }

        public bool IsError(ItemStatus status)
        {
            if (status.Rarity == EquipmentRarity.Normal || status.IsCorrupted)
            {
                return true;
            }
            if (Affix != null && Affix.Type.Contains("prefix"))
            {
                return status.Rarity == EquipmentRarity.Magic && status.MinPrefixes == 1 ||
                    status.Rarity == EquipmentRarity.Rare && status.MinPrefixes == 3;
            }
            if (Affix != null && Affix.Type.Contains("suffix"))
            {
                return status.Rarity == EquipmentRarity.Magic && status.MinSuffixes == 1 ||
                    status.Rarity == EquipmentRarity.Rare && status.MinSuffixes == 3;
            }

            return false;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
                return status;

            if (Affix.Type.Contains("prefix"))
            {
                status.MinPrefixes = Math.Min(status.MinPrefixes + 1, 3);
                status.MaxPrefixes = Math.Max(status.MinPrefixes, status.MaxPrefixes);
            }
            if (Affix.Type.Contains("suffix"))
            {
                status.MinSuffixes = Math.Min(status.MinSuffixes + 1, 3);
                status.MaxSuffixes = Math.Max(status.MinSuffixes, status.MaxSuffixes);
            }

            status.MinAffixes = Math.Min(status.MinAffixes + 1, 6);
            status.MaxAffixes = Math.Max(status.MinAffixes, status.MaxAffixes);

            return status;
        }
    }
}
