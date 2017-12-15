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
using System.Windows.Shapes;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Controls;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for SimulationWindow.xaml
    /// </summary>
    public partial class SimulationWindow : Window, INotifyPropertyChanged
    {
        private readonly List<ISimulationControl> _controls;
        private readonly EquipmentFactory _factory;

        private CraftingTreeControl CraftingTree { get; }
        private BaseSelectionControl BaseSelection { get; }
        private ItemListControl ItemList { get; }
        private CraftingControl Crafting { get; }
        private CraftingResultsControl Results { get;  }

        private int _currentControlIndex = 0;

        public ContentControl SelectedStep { get; set; }
        public bool IsReady => _controls[_currentControlIndex].CanComplete();

        public SimulationWindow(
            CraftingTreeControl craftingTree, 
            BaseSelectionControl baseSelection, 
            ItemListControl itemList, 
            CraftingControl crafting, 
            CraftingResultsControl results,
            EquipmentFactory factory)
        {
            CraftingTree = craftingTree;
            BaseSelection = baseSelection;
            ItemList = itemList;
            Crafting = crafting;
            Results = results;

            _controls = new List<ISimulationControl>
            {
                BaseSelection,
                CraftingTree,
                ItemList,
                Crafting,
                Results
            };

            Crafting.OnCompletion = () => OnNextClick(this, null);

            SelectedStep = BaseSelection;
            BaseSelection.Initialize(() => OnPropertyChanged(nameof(IsReady)));

            InitializeComponent();
            DataContext = this;

            _factory = factory;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }

        private void OnPreviousClick(object sender, RoutedEventArgs e)
        {
            _controls[_currentControlIndex].OnClose();

            if (_currentControlIndex > 0)
            {
                _currentControlIndex--;
            }

            SelectedStep = _controls[_currentControlIndex] as ContentControl;
            OnPropertyChanged(nameof(SelectedStep));
            OnPropertyChanged(nameof(IsReady));
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (_currentControlIndex >= _controls.Count - 1)
            {
                return;
            }
            
            _controls[_currentControlIndex].OnClose();
            _currentControlIndex++;

            SelectedStep = _controls[_currentControlIndex] as ContentControl;
            OnPropertyChanged(nameof(SelectedStep));

            if (_controls[_currentControlIndex] == CraftingTree)
            {
                var baseInfo = BaseSelection.BaseInformation;
                _factory.Initialize(baseInfo.SelectedBase, baseInfo.Category, baseInfo.ItemLevel);
                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();
                CraftingTree.Initialize(affixes, baseItem, baseInfo);
            }
            else if (_controls[_currentControlIndex] == ItemList)
            {
                var baseInfo = BaseSelection.BaseInformation;
                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();

                ItemList.Initialize(affixes, baseItem, baseInfo);
            }
            else if (_controls[_currentControlIndex] == Crafting)
            {
                var craftingTree = CraftingTree.CraftingTree;
                var itemList = ItemList.ItemPrototypes.ToList();

                Crafting.Initialize(craftingTree, _factory, itemList, BaseSelection.ItemCost);
            }
            else if (_controls[_currentControlIndex] == Results)
            {
                var currencySpent = CraftingTree.CraftingTree.GetCurrencySpent(Crafting.ScourCount, BaseSelection.ItemCost, Crafting.BaseItemCount);
                Results.Initialize(Crafting.MatchingItems, currencySpent);
            }

            OnPropertyChanged(nameof(IsReady));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
