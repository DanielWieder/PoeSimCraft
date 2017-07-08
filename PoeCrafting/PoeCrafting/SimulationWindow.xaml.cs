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

        private CraftingTreeControl CraftingTree { get; }
        private BaseSelectionControl BaseSelection { get; }
        private ItemListControl ItemList { get; }
        private CraftingControl Crafting { get; }

        public ContentControl SelectedStep { get; set; }
        private EquipmentFactory _factory;

        public SimulationWindow(CraftingTreeControl craftingTree, BaseSelectionControl baseSelection, ItemListControl itemList, CraftingControl crafting, EquipmentFactory factory)
        {
            CraftingTree = craftingTree;
            BaseSelection = baseSelection;
            ItemList = itemList;
            Crafting = crafting;

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
            if (SelectedStep == CraftingTree)
            {
                SelectedStep = BaseSelection;
                OnPropertyChanged(nameof(SelectedStep));
            }
            else if (SelectedStep == ItemList)
            {
                SelectedStep = CraftingTree;
                OnPropertyChanged(nameof(SelectedStep));
            }
            else if (SelectedStep == Crafting)
            {
                SelectedStep = ItemList;
                OnPropertyChanged(nameof(SelectedStep));
            }
        }

        private void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (SelectedStep == BaseSelection && BaseSelection.IsReady())
            {
                _factory.Initialize(BaseSelection.SelectedBase, BaseSelection.ItemLevel);

                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();
                CraftingTree.Initialize(affixes, baseItem);

                SelectedStep = CraftingTree;
                OnPropertyChanged(nameof(SelectedStep));
            }
            else if (SelectedStep == CraftingTree && CraftingTree.IsReady())
            {
                CraftingTree.Save();

                var affixes = _factory.GetPossibleAffixes();
                var baseItem = _factory.GetBaseItem();

                ItemList.Initialize(affixes, baseItem);

                SelectedStep = ItemList;
                OnPropertyChanged(nameof(SelectedStep));
            }
            else if (SelectedStep == ItemList && ItemList.IsReady())
            {
                ItemList.Save();

                var craftingTree = CraftingTree.CraftingTree;
                var itemList = ItemList.ItemPrototypes.ToList();

                Crafting.Initialize(craftingTree, _factory, itemList);
                Crafting.Run();
                SelectedStep = Crafting;
                OnPropertyChanged(nameof(SelectedStep));
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
