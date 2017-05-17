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
        private IFetchArmourByItemName _fetchArmorByItemName;
        private IFetchAccessoriesByItemName _fetchAccessoriesByName;
        private IFetchWeaponsByItemName _fetchWeaponsByName;
        private IFetchAffixesByItemName _fetchAffixesByItemName;
        private IFetchTypeByItemName _fetchTypeByItemName;

        private IRandom _random;
        private List<Affix> _implicits;
        private List<Affix> _prefixes;
        private List<Affix> _suffixes;
        private Affix _baseImplicit;
        private ItemBase _baseItem;

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

        public void Initialize(string baseItemName)
        {
            if (string.IsNullOrEmpty(baseItemName)) return;

            _fetchTypeByItemName.Name = baseItemName;
            var type = _fetchTypeByItemName.Execute();

            if (type.Contains("Armour"))
            {
                _fetchArmorByItemName.Name = baseItemName;
                _baseItem = _fetchArmorByItemName.Execute();
            }
            else if (type.Contains("Accessory"))
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

            _baseImplicit = null;
            _fetchAffixesByItemName.Name = baseItemName;
            var affixes = _fetchAffixesByItemName.Execute();

            _suffixes = affixes.Where(x => x.Type.Contains("suffix")).ToList();
            _prefixes = affixes.Where(x => x.Type.Contains("prefix")).ToList();
            _implicits = affixes.Where(x => x.Type.Contains("corrupted")).ToList();
        }

        public Equipment CreateEquipment()
        {
            if (_baseItem == null) throw new InvalidOperationException("The equipment factory must be initialized before it can produce equipment");

            return new Equipment
            {
                PossibleImplicits = this._implicits,
                PossiblePrefixes = this._prefixes,
                PossibleSuffixes = this._suffixes,
                ItemBase = (ItemBase)_baseItem.Clone(),
                Implicit = _baseImplicit != null ? StatFactory.Get(_random, _baseImplicit) : null
            };
        }
    }
}
