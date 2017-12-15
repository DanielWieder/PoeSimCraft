using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.UI.Annotations;
using PoeCrafting.Entities;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for CraftingTree.xaml
    /// </summary>
    public partial class CraftingTreeControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        private readonly CurrencyFactory _factory;
        public CraftingTree CraftingTree { get; set; }
        private ICraftingStep _selected;
        private ObservableCollection<Affix> _affixes;
        private ItemBase _itemBase;
        public CraftingTreeViewModel Tree { get; set; }
        public ConditionControl Condition { get; set; }

        private BaseInfomation _baseInfo = null;

        public CraftingTreeControl(CurrencyFactory factory)
        {
            _factory = factory;
            CraftingTree = new CraftingTree(factory);
            Tree = new CraftingTreeViewModel(CraftingTree, _selected);
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(List<Affix> affixes, ItemBase itemBase, BaseInfomation baseInfo)
        {
            if (_baseInfo == null || !_baseInfo.Equals(baseInfo))
            {
                _itemBase = itemBase;
                _affixes = new ObservableCollection<Affix>(affixes);
                CraftingTree.ClearConditions();
                Condition = null;
                _selected = Tree.Tree[0].Value;
                Tree.UpdateTree(CraftingTree, _selected);
                OnPropertyChanged(nameof(Condition));
                OnPropertyChanged(nameof(Tree));
                _baseInfo = baseInfo;
                _factory.UpdateValues(baseInfo.League);
            }
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSelectedStepChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            var model = treeView.SelectedItem as CraftingStepViewModel;

            if (model?.Value == null || _selected == model.Value)
            {
                return;
            }

            var craftingStep = model.Value;

            Condition?.Save();

            if (model.HasCondition)
            {
                Condition = new ConditionControl(model.Condition, _itemBase, _affixes.ToList());
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
            Condition?.Save();
        }
    }
}
