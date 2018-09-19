using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PoeCrafting.Currency;
using PoeCrafting.Data;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;


namespace PoeCrafting.Domain
{
    public class EquipmentFactory
    {
        private readonly IFetchArmourByItemName _fetchArmorByItemName;
        private readonly IFetchAccessoriesByItemName _fetchAccessoriesByName;
        private readonly IFetchWeaponsByItemName _fetchWeaponsByName;
        private readonly IFetchAffixesByItemName _fetchAffixesByItemName;
        private readonly IFetchTypeByItemName _fetchTypeByItemName;

        private readonly IRandom _random;
        private List<Affix> _affixes;
        private List<Affix> _prefixes;
        private List<Affix> _suffixes;
        private Affix _baseImplicit;
        private ItemBase _baseItem;
        private int _itemLevel;
        private int _suffixWeight;
        private int _prefixWeight;
        private int _totalWeight;
        private string _baseItemName;
        private int _category;

        public EquipmentFactory(
            IRandom random, 
            IFetchArmourByItemName fetchArmorByItemName, 
            IFetchAffixesByItemName fetchAffixesByItemName, 
            IFetchTypeByItemName fetchTypeByItemName, IFetchAccessoriesByItemName fetchAccessoriesByName, IFetchWeaponsByItemName fetchWeaponsByName)
        {
            _random = random;
            _fetchArmorByItemName = fetchArmorByItemName;
            _fetchAffixesByItemName = fetchAffixesByItemName;
            _fetchTypeByItemName = fetchTypeByItemName;
            _fetchAccessoriesByName = fetchAccessoriesByName;
            _fetchWeaponsByName = fetchWeaponsByName;
        }

        public void Initialize(string baseItemName, int category, int itemLevel = 84)
        {
            if (string.IsNullOrEmpty(baseItemName)) return;
            if (_itemLevel == itemLevel && baseItemName == _baseItemName && category == _category) return;

            _baseItemName = baseItemName;
            _category = category;
            _itemLevel = itemLevel;

            _fetchTypeByItemName.Name = baseItemName;
            var type = _fetchTypeByItemName.Execute();

            if (type.Contains(TypeInfo.ItemTypeArmour))
            {
                _fetchArmorByItemName.Name = baseItemName;
                _baseItem = _fetchArmorByItemName.Execute();
            }
            else if (type.Contains(TypeInfo.ItemTypeJewelry))
            {
                _fetchAccessoriesByName.Name = baseItemName;
                _baseItem = _fetchAccessoriesByName.Execute();
            }
            else if (type.Contains(TypeInfo.ItemTypeWeapon))
            {
                _fetchWeaponsByName.Name = baseItemName;
                _baseItem = _fetchWeaponsByName.Execute();
            }
            else
            {
                throw new InvalidOperationException("The equipment type does not match the passed type");
            }
            _baseImplicit = null;
            _fetchAffixesByItemName.Name = baseItemName;
            var affixes = _fetchAffixesByItemName.Execute();

            affixes = affixes.GroupBy(x => x.ModName).Select(x => x.OrderBy(y => y.Priority).First()).ToList();

            foreach (var affix in affixes)
            {
                affix.ModType = Regex.Replace(affix.ModName, "[0-9_]", "").Trim();
            }

            var modTiers = affixes.GroupBy(x => x.ModType);

            foreach (var mods in modTiers)
            {
                var modList = mods.Where(x => x.Weight > 0).OrderByDescending(x => ParseInteger(x.ModName)).ToList();

                for (int i = 0; i < modList.Count; i++)
                {
                    modList[i].Tier = i + 1;
                }
            }

            affixes = affixes.Where(x => x.ILvl <= itemLevel)
                             .Where(x => x.Faction == 0 || x.Faction == category)
                             .Where(x => x.Type == TypeInfo.AffixTypeMeta || ((x.Type == TypeInfo.AffixTypePrefix || x.Type == TypeInfo.AffixTypeSuffix) && x.Weight > 0))
                             .ToList();

            var rollableAffixes = affixes.Where(x => x.Type == TypeInfo.AffixTypePrefix || x.Type == TypeInfo.AffixTypeSuffix).ToList();

            var modTypeWeights = rollableAffixes
                .GroupBy(x => x.ModType)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.Weight));

            foreach(var affix in rollableAffixes)
            {
                affix.ModTypeWeight = modTypeWeights[affix.ModType];
            }

            _affixes = affixes.ToList();
            _prefixes = affixes.Where(x => x.Type == TypeInfo.AffixTypePrefix).ToList();
            _suffixes = affixes.Where(x => x.Type == TypeInfo.AffixTypeSuffix).ToList();

            _suffixWeight = this._suffixes.Sum(x => x.Weight);
            _prefixWeight = this._prefixes.Sum(x => x.Weight);

            _totalWeight = rollableAffixes.Sum(x => x.Weight);

        }

        private static int ParseInteger(string str)
        {
            var matchedString =  Regex.Match(str, "\\d+").Value;

            return matchedString == string.Empty ? 0 : int.Parse(matchedString);
        }

        public ItemBase GetBaseItem()
        {
            if (_baseItem == null) throw new InvalidOperationException("The equipment factory must be initialized before it can produce equipment");

            return (ItemBase) _baseItem.Clone();
        }

        public List<Affix> GetPossibleAffixes()
        {
            if (_baseItem == null) throw new InvalidOperationException("The equipment factory must be initialized before it can produce equipment");

            return this._affixes;
        }

        public Equipment CreateEquipment()
        {
            if (_baseItem == null) throw new InvalidOperationException("The equipment factory must be initialized before it can produce equipment");

            return new Equipment
            {
                ItemLevel = this._itemLevel,
                PossibleAffixes = this._affixes,
                PossiblePrefixes = this._prefixes,
                PossibleSuffixes = this._suffixes,
                ItemBase = (ItemBase)_baseItem.Clone(),
                Implicit = _baseImplicit != null ? StatFactory.AffixToStat(_random, _baseImplicit) : null,
                TotalWeight = _totalWeight,
                PrefixWeight = _prefixWeight,
                SuffixWeight = _suffixWeight
            };
        }
    }
}
