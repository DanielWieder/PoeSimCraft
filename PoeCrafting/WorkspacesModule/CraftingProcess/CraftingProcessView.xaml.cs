using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;
using Prism.Regions;
using WorkspacesModule.Condition;

namespace WorkspacesModule.CraftingProcess
{
    /// <summary>
    /// Interaction logic for CraftingProcessView.xaml
    /// </summary>
    public partial class CraftingProcessView : UserControl, INotifyPropertyChanged, INavigationAware
    {
        private readonly IItemConfigRepository _configRepository;
        private readonly EquipmentFactory _equipmentFactory;
        public CraftingTree CraftingTree { get; set; }
        private ICraftingStep _selected;
        private ObservableCollection<Affix> _affixes;
        private PoeCrafting.Entities.ItemBase _itemBase;
        public CraftingTreeViewModel Tree { get; set; }
        public ConditionControlView Condition { get; set; }
        public ConditionControlViewModel ConditionModel { get; set; }

        private ItemConfig _baseInfo = null;

        public CraftingProcessView(CurrencyFactory factory, IItemConfigRepository configRepository, EquipmentFactory equipmentFactory)
        {
            _configRepository = configRepository;
            _equipmentFactory = equipmentFactory;
            CraftingTree = new CraftingTree(factory);
            Tree = new CraftingTreeViewModel(CraftingTree, _selected);
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize()
        {
            var baseInfo = _configRepository.GetItemConfig();
            if (_baseInfo != null && _baseInfo.Equals(baseInfo)) return;

            _baseInfo = baseInfo;
            _equipmentFactory.Initialize(baseInfo.ItemBase, baseInfo.Category, baseInfo.ItemLevel);
            _itemBase = _equipmentFactory.GetBaseItem();
            _affixes = new ObservableCollection<Affix>(_equipmentFactory.GetPossibleAffixes());

            CraftingTree.ClearConditions();
            CraftingTree.Initialize();
            Condition = null;
            _selected = Tree.Tree[0].Value;
            Tree.UpdateTree(CraftingTree, _selected);
            OnPropertyChanged(nameof(Condition));
            OnPropertyChanged(nameof(Tree));

        }

        private void OnInsertOptionSelected(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox.DataContext == BindingOperations.DisconnectedSource)
            {
                return;
            }

            var model = comboBox.DataContext as CraftingStepViewModel;
            var craftingStep = model.Value;
            var selection = comboBox.SelectedItem as string;

            CraftingTree.Replace(craftingStep, selection);

            Tree.UpdateTree(CraftingTree, _selected);
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selected != null)
            {
                CraftingTree.Delete(_selected);

                Tree.UpdateTree(CraftingTree, _selected);
                _selected = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSelectedStepChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            var model = treeView.SelectedItem as CraftingStepViewModel;

            if (model?.Value == null || _selected == model.Value)
            {
                return;
            }

            var craftingStep = model.Value;

            ConditionModel?.Save();

            if (model.HasCondition)
            {
                ConditionModel = new ConditionControlViewModel(model.Condition, _itemBase, _affixes.ToList());
                Condition = new ConditionControlView(ConditionModel);
                OnPropertyChanged(nameof(Condition));
            }
            else
            {
                Condition = null;
                OnPropertyChanged(nameof(Condition));
            }

            CraftingTree.Select(craftingStep);
            _selected = model.Value;
            Tree.UpdateTree(CraftingTree, _selected);
        }

        public bool CanComplete()
        {
            return true;
        }

        public void OnClose()
        {
            ConditionModel?.Save();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Initialize();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
