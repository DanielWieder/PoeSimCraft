using System;
using System.Collections.Generic;
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
    public partial class CraftingTreeControl : UserControl, INotifyPropertyChanged
    {
        private readonly CraftingTree _tree;
        private ICraftingStep _selected;
        private List<Affix> _affixes;
        private ItemBase _itemBase;
        public CraftingTreeViewModel Tree { get; set; }
        public ConditionControl Condition { get; set; }

        public CraftingTreeControl(CurrencyFactory factory)
        {
            _tree = new CraftingTree(factory);
            Tree = new CraftingTreeViewModel(_tree, _selected);
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(List<Affix> affixes, ItemBase itemBase)
        {
            _itemBase = itemBase;
            _affixes = affixes;
            _tree.ClearConditions();
            Tree.UpdateTree(_tree, _selected);
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

            _tree.Replace(craftingStep, selection);

            Tree.UpdateTree(_tree, _selected);
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selected != null)
            {
                _tree.Delete(_selected);

                Tree.UpdateTree(_tree, _selected);
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

            if (model.HasCondition)
            {
                Condition = new ConditionControl(model.Condition, _itemBase, _affixes);
                OnPropertyChanged(nameof(Condition));
            }
            else
            {
                Condition = null;
                OnPropertyChanged(nameof(Condition));
            }

            _tree.Select(craftingStep);
            _selected = model.Value;
            Tree.UpdateTree(_tree, _selected);

            var item = FindItem(x => x.Value == _selected, treeView.Items.OfType<CraftingStepViewModel>().ToList());
            if (item != null)
            {
        //        item.Selected = true;
            }
        }

        private CraftingStepViewModel FindItem(Func<CraftingStepViewModel, bool> findItem, List<CraftingStepViewModel> collection )
        {
            foreach (CraftingStepViewModel item in collection)
            {
                if (findItem(item))
                {
                    return item;
                }

                var result = FindItem(findItem, item.Children.ToList());
                {
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}
