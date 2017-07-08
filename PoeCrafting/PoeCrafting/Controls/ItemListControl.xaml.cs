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
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Controls;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for ItemListControl.xaml
    /// </summary>
    public partial class ItemListControl : UserControl, INotifyPropertyChanged
    {
        private List<Affix> _affixes;
        private ItemBase _itemBase;

        public ObservableCollection<ItemPrototypeModel> ItemPrototypes { get; set; }
        public ItemPrototypeModel SelectedItem { get; set; }

        public ItemControl ItemControl { get; set; }

        public ItemListControl()
        {
            ItemPrototypes = new ObservableCollection<ItemPrototypeModel>();
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(List<Affix> affixes, ItemBase itemBase)
        {
            _itemBase = itemBase;
            _affixes = affixes;
            SelectedItem = new ItemPrototypeModel();
            ItemPrototypes.Clear();
            ItemControl = null;

            AddItem();
            SelectedItem = ItemPrototypes[0];
            OnPropertyChanged(nameof(SelectedItem));
        }

        public void Save()
        {
            ItemControl?.Save();
        }

        private void AddOnClick(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        private void DeleteOnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                ItemPrototypes.Remove(SelectedItem);
                SelectedItem = null;
                ItemControl = null;

                if (!ItemPrototypes.Any())
                {
                    AddItem();
                }

                SelectedItem = ItemPrototypes[0];

                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(ItemControl));
            }
        }

        private void AddItem()
        {
            var index = GetNextIndex();
            ItemPrototypeModel prototypeModel = new ItemPrototypeModel
            {
                Condition = new CraftingCondition(),
                ItemName = "Item " + index,
                Value = 0,
                Index = index
            };
            ItemPrototypes.Add(prototypeModel);
            OnPropertyChanged(nameof(ItemPrototypes));
        }

        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem == null)
            {
                ItemControl = null;
                OnPropertyChanged(nameof(ItemControl));
                return;
            }

            ItemControl?.Save();

            ItemControl item = new ItemControl(SelectedItem, _affixes, _itemBase);

            ItemControl = item;
            OnPropertyChanged(nameof(ItemControl));
        }

        private int GetNextIndex()
        {
            if (!ItemPrototypes.Any())
            {
                return 1;
            }

            return ItemPrototypes.Max(x => x.Index) + 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsReady()
        {
            return true;
        }
    }
}
