using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Data;
using PoeCrafting.Entities;


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
        private Affix _baseImplicit;
        private ItemBase _baseItem;
        private int _itemLevel;

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

        public void Initialize(string baseItemName, int itemLevel = 84)
        {
            if (string.IsNullOrEmpty(baseItemName)) return;

            _fetchTypeByItemName.Name = baseItemName;
            var type = _fetchTypeByItemName.Execute();

            if (type.Contains("Armour"))
            {
                _fetchArmorByItemName.Name = baseItemName;
                _baseItem = _fetchArmorByItemName.Execute();
            }
            else if (type.Contains("Jewelry"))
            {
                _fetchAccessoriesByName.Name = baseItemName;
                _baseItem = _fetchAccessoriesByName.Execute();
            }
            else if (type.Contains("Weapon"))
            {
                _fetchWeaponsByName.Name = baseItemName;
                _baseItem = _fetchWeaponsByName.Execute();
            }
            else
            {
                throw new InvalidOperationException("The equipment type does not match the passed type");
            }
            _itemLevel = itemLevel;
            _baseImplicit = null;
            _fetchAffixesByItemName.Name = baseItemName;
            var affixes = _fetchAffixesByItemName.Execute();

            _affixes = affixes.ToList();
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
                ItemBase = (ItemBase)_baseItem.Clone(),
                Implicit = _baseImplicit != null ? StatFactory.AffixToStat(_random, _baseImplicit) : null,
            };
        }
    }
}
