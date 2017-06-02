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
using PoeCrafting.Domain.Crafting;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for CraftingTree.xaml
    /// </summary>
    public partial class CraftingTreeControl : UserControl, INotifyPropertyChanged
    {
        private readonly CraftingTree _tree;
        public CraftingTreeViewModel Tree { get; set; }


        public CraftingTreeControl(CurrencyFactory factory)
        {
            _tree = new CraftingTree(factory);
            Tree = new CraftingTreeViewModel(_tree);

            InitializeComponent();
            DataContext = Tree;
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

            Tree.UpdateTree(_tree);
        }

        private void OnCraftingStepSelected(object sender, MouseButtonEventArgs e)
        {
            var comboBox = sender as TextBlock;

            if (comboBox.DataContext == BindingOperations.DisconnectedSource)
            {
                return;
            }

            var model = comboBox.DataContext as CraftingStepViewModel;
            var craftingStep = model.Value;
            selected = model;
            _tree.Select(craftingStep);

            Tree.UpdateTree(_tree);
        }

        private CraftingStepViewModel selected = null;

        public void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (selected != null)
            {
                _tree.Delete(selected.Value);

                Tree.UpdateTree(_tree);
                selected = null;
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
