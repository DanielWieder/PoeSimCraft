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
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for CraftingControl.xaml
    /// </summary>
    public partial class CraftingControl : UserControl, INotifyPropertyChanged
    {
        private int RunCount = 1000;
        public int Progress { get; set; } = 0;
        public List<Equipment> EquipmentList = new List<Equipment>();

        private CraftingTree _craftingTree;
        private EquipmentFactory _factory;
        private List<ItemPrototypeModel> _itemPrototypes;

        public Dictionary<ItemPrototypeModel, List<Equipment>> MatchingItems;
        public int ItemCount = 0;


        public CraftingControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(CraftingTree craftingTree, EquipmentFactory factory, List<ItemPrototypeModel> itemPrototypes)
        {
            _craftingTree = craftingTree;
            _factory = factory;
            _itemPrototypes = itemPrototypes.OrderBy(x => x.Value).ThenBy(x => x.ItemName).ToList();
            MatchingItems = new Dictionary<ItemPrototypeModel, List<Equipment>>();
            foreach (var prototype in _itemPrototypes)
            {
                MatchingItems.Add(prototype, new List<Equipment>());
            }


            ItemCount = 0;
        }

        public void Run()
        {
            for (int i = 0; i < RunCount; i++)
            {
                var newItem = _factory.CreateEquipment();
                var result = _craftingTree.Craft(newItem);
                EquipmentList.Add(result);

                foreach (var prototype in _itemPrototypes)
                {
                    if (prototype.Condition.IsValid(result))
                    {
                        MatchingItems[prototype].Add(result);
                        break;
                    }
                }

                ItemCount++;

                Progress = (int)(EquipmentList.Count/(float)RunCount * 100f);
                OnPropertyChanged(nameof(Progress));
            }
        }

        public bool IsReady()
        {
            return Progress == 100;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
