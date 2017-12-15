using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using PoeCrafting.Domain;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;
using Prism.Commands;

namespace WorkspacesModule.ItemBase
{
    public class ItemBaseViewModel : WorkspaceViewModel
    {
        private ItemConfig _config;
        private readonly IItemConfigRepository _configRepository;
        private readonly EquipmentFetch _equipmentFetch;

        private string _itemType;
        private string _itemBase;
        private string[] _itemTypeOptions;
        private string[] _itemBaseOptions;
        private string _itemLevel;
        bool _isSelected;
        DelegateCommand _saveCommand;

        public ItemBaseViewModel(IItemConfigRepository configRepository, EquipmentFetch equipmentFetch, ItemConfig config)
        {
            this._configRepository = configRepository;
            base.DisplayName = "Item Config";
            _equipmentFetch = equipmentFetch;
            _itemTypeOptions = _equipmentFetch.FetchSubtypes().ToArray();
            _config = new ItemConfig();

            ItemLevel = config.ItemLevel.ToString();
            ItemType = config.ItemType;
            ItemBase = config.ItemBase;
        }

        public string[] ItemTypeOptions => _itemTypeOptions;
        public string[] ItemBaseOptions => _itemBaseOptions;
        public bool CanSave => _config.IsValid;

        public string ItemLevel
        {
            get { return _itemLevel; }
            set
            {
                _itemLevel = value;

                Regex regex = new Regex(@"^\d*$");
                bool isInteger = regex.IsMatch(value);

                if (string.IsNullOrEmpty(value) || !isInteger)
                {
                    _config.ItemLevel = -1;
                    return;
                }

                var val = int.Parse(value);

                if (val == _config.ItemLevel)
                    return;

                _config.ItemLevel = val;

                _saveCommand.RaiseCanExecuteChanged();

                base.OnPropertyChanged("ItemLevel");
            }
        }

        public string ItemType
        {
            get { return _itemType; }
            set
            {
                if (value == _itemType || String.IsNullOrEmpty(value))
                    return;

                _itemType = value;
                _config.ItemType = value;
                _itemBaseOptions = _equipmentFetch.FetchBasesBySubtype(_itemType).ToArray();

                base.OnPropertyChanged("ItemType");
                base.OnPropertyChanged("ItemBaseOptions");
            }
        }

        public string ItemBase
        {
            get { return _itemBase; }
            set
            {
                if (value == _itemBase || String.IsNullOrEmpty(value))
                    return;

                _itemBase = value;
                _config.ItemBase = value;

                _saveCommand.RaiseCanExecuteChanged();

                base.OnPropertyChanged("ItemBase");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;

                base.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand(Save, () => CanSave);
                }
                return _saveCommand;
            }
        }

        private void Save()
        {
            if (!_config.IsValid)
                throw new InvalidOperationException("Invalid configuration");

            _configRepository.SetItemConfig(_config);
        }
    }
}
