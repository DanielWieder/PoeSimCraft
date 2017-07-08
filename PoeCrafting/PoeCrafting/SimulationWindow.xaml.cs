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
        public bool IsReady => _controls[_currentControlIndex].IsReady();


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

            SelectedStep = BaseSelection;
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
            if (_currentControlIndex > 0)
            {
                _currentControlIndex--;
            }

            SelectedStep = _controls[_currentControlIndex] as ContentControl;
            OnPropertyChanged(nameof(SelectedStep));
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (!_controls[_currentControlIndex].IsReady() || _currentControlIndex >= _controls.Count - 1)
            {
                return;
            }
            
            _controls[_currentControlIndex].Save();
            _currentControlIndex++;

            SelectedStep = _controls[_currentControlIndex] as ContentControl;
            OnPropertyChanged(nameof(SelectedStep));

            if (_controls[_currentControlIndex] == CraftingTree)
            {
                _factory.Initialize(BaseSelection.SelectedBase, BaseSelection.ItemLevel);
                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();
                CraftingTree.Initialize(affixes, baseItem);
            }
            else if (_controls[_currentControlIndex] == ItemList)
            {
                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();

                ItemList.Initialize(affixes, baseItem);
            }
            else if (_controls[_currentControlIndex] == Crafting)
            {
                var craftingTree = CraftingTree.CraftingTree;
                var itemList = ItemList.ItemPrototypes.ToList();

                Crafting.Initialize(craftingTree, _factory, itemList, BaseSelection.Currency);
            }
            else if (_controls[_currentControlIndex] == Results)
            {
                Results.Initialize(Crafting.MatchingItems);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
