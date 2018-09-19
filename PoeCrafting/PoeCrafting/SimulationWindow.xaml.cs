using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PoeCrafting.Currency;
using PoeCrafting.Domain;
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
        private readonly EquipmentFactory _equipmentFactory;
        private readonly CurrencyFactory _currencyFactory;

        private BaseSelectionControl BaseSelection { get; }
        private ItemListControl ItemList { get; }
        private CraftingControl Crafting { get; }
        private CraftingResultsControl Results { get;  }

        private int _currentControlIndex = 0;

        public ContentControl SelectedStep { get; set; }
        public bool IsReady => _controls[_currentControlIndex].CanComplete();

        public SimulationWindow(
            BaseSelectionControl baseSelection, 
            ItemListControl itemList, 
            CraftingControl crafting, 
            CraftingResultsControl results,
            EquipmentFactory equipmentFactory,
            CurrencyFactory currencyFactory)
        {
            BaseSelection = baseSelection;
            ItemList = itemList;
            Crafting = crafting;
            Results = results;

            _controls = new List<ISimulationControl>
            {
                BaseSelection,
                ItemList,
                Crafting,
                Results
            };

            Crafting.OnCompletion = () => OnNextClick(this, null);

            SelectedStep = BaseSelection;
            BaseSelection.Initialize(() => OnPropertyChanged(nameof(IsReady)));

            InitializeComponent();
            DataContext = this;

            _equipmentFactory = equipmentFactory;
            _currencyFactory = currencyFactory;
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


            if (_controls[_currentControlIndex] == ItemList)
            {
                var baseInfo = BaseSelection.BaseInformation;
                var affixes = _equipmentFactory.GetPossibleAffixes();
                var baseItem = _equipmentFactory.GetBaseItem();

                ItemList.Initialize(affixes, baseItem, baseInfo);
            }
            else if (_controls[_currentControlIndex] == Crafting)
            {
                var itemList = ItemList.ItemPrototypes.ToList();

                Crafting.Initialize(null, _equipmentFactory, _currencyFactory, itemList, BaseSelection.ItemCost);
            }
            else if (_controls[_currentControlIndex] == Results)
            {
                var currencySpent = -1;
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
