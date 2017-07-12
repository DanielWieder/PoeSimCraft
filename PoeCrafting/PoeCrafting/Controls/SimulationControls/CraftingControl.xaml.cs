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
using System.Threading;
using System.Threading.Tasks;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for CraftingControl.xaml
    /// </summary>
    public partial class CraftingControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        private int _totalCurrency;
        public int Progress { get; set; } = 0;
        public string Message { get; set; } = "Crafting...";

        public List<Equipment> EquipmentList = new List<Equipment>();

        private CraftingTree _craftingTree;
        private EquipmentFactory _factory;
        private List<ItemPrototypeModel> _itemPrototypes;

        public Dictionary<ItemPrototypeModel, List<Equipment>> MatchingItems;
        public int ItemCount = 0;

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task task;

        public CraftingControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(CraftingTree craftingTree, EquipmentFactory factory, List<ItemPrototypeModel> itemPrototypes, int currency)
        {
            _totalCurrency = currency;
            _craftingTree = craftingTree;
            _craftingTree.ClearCurrencySpent();

            _factory = factory;
            _itemPrototypes = itemPrototypes.OrderByDescending(x => x.Value).ThenBy(x => x.ItemName).ToList();
            MatchingItems = new Dictionary<ItemPrototypeModel, List<Equipment>>();
            foreach (var prototype in _itemPrototypes)
            {
                MatchingItems.Add(prototype, new List<Equipment>());
            }

            if (task != null && task.Status == TaskStatus.Running)
            {
                cancellationTokenSource.Cancel();
            }

            ItemCount = 0;
            Progress = 0;
            Message = "Crafting...";

            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(Progress));

            RunTask();
        }

        public void RunTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            task = new Task(() => Run(token), token);
            task.Start();
        }

        private void Run(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            for (double currencySpent = 0; currencySpent < _totalCurrency; currencySpent =_craftingTree.GetCurrencySpent())
            {
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }

                // If an item has been created and no currency was spent then no currency will ever be spent and it will cause an infinite loop
                if (ItemCount == 1 && currencySpent <= 0)
                {
                    Progress = 100;
                    Message = "Completed";
                    OnPropertyChanged(nameof(Message));
                    OnPropertyChanged(nameof(Progress));
                    return;
                }

                var newItem = _factory.CreateEquipment();
                var result = _craftingTree.Craft(newItem, ct);
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

                var newProgress = (int) (currencySpent/_totalCurrency*100);

                if(newProgress != Progress)
                {
                    Progress = (int) (currencySpent/_totalCurrency*100);
                    OnPropertyChanged(nameof(Progress));
                }
            }
            Message = "Completed";
            Progress = 100;
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(Message));
        }

        public bool IsReady()
        {
            return Progress == 100;
        }

        public void Save()
        {
            if (task != null && task.Status == TaskStatus.Running)
            {
                cancellationTokenSource.Cancel();
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
