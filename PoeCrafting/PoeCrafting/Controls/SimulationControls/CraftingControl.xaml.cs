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

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for CraftingControl.xaml
    /// </summary>
    public partial class CraftingControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        // dependencies
        private CraftingTree _craftingTree;
        private EquipmentFactory _factory;
        private List<ItemPrototypeModel> _itemPrototypes;

        // generated
        public List<Equipment> EquipmentList = new List<Equipment>();
        public Dictionary<ItemPrototypeModel, List<Equipment>> MatchingItems = new Dictionary<ItemPrototypeModel, List<Equipment>>();

        // local
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        Task _task;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

        public int Currency { get; set; } = 1000;

        public int ScourCount { get; set; }

        public int BaseItemCount { get; set; }

        public double BaseItemCost { get; set; }

        public Action OnCompletion { get; set; } = null;

        // Output
        private bool _isCrafting = false;

        public bool IsCrafting
        {
            get { return _isCrafting; }
            set
            {
                _isCrafting = value;
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(IsCrafting));
            }
        }

        private bool _isCancelled = false;
        public bool IsCancelled
        {
            get { return _isCancelled; }
            set
            {
                _isCancelled = value;
                OnPropertyChanged(nameof(Message));
                OnPropertyChanged(nameof(IsCancelled));
            }
        }

        public bool IsStarted => Progress >= 0;
        public bool IsCompleted => Progress >= 100;
        public string Message => IsCancelled ? "Cancelled" : 
                                IsCrafting ? "Crafting..." :
                                !IsCrafting && Progress >= 100 ? "Completed" : 
                                "Waiting";

        public Visibility MessageVisibility => IsStarted ? Visibility.Visible : Visibility.Hidden; 

        public double Progress { get; set; }

        public CraftingControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(CraftingTree craftingTree, EquipmentFactory factory, List<ItemPrototypeModel> itemPrototypes, double baseItemCost)
        {
            _craftingTree = craftingTree;
            _craftingTree.ClearCurrencySpent();

            _factory = factory;
            _itemPrototypes = itemPrototypes.OrderByDescending(x => x.Value).ThenBy(x => x.ItemName).ToList();
            MatchingItems.Clear();
            EquipmentList.Clear();

            foreach (var prototype in _itemPrototypes)
            {
                MatchingItems.Add(prototype, new List<Equipment>());
            }

            if (_task != null && _task.Status == TaskStatus.Running)
            {
                _cancellationTokenSource.Cancel();
            }

            Progress = -1;
            ScourCount = 0;
            BaseItemCount = 0;
            BaseItemCost = baseItemCost;

            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(MessageVisibility));
        }

        public void Craft()
        {
            if (Progress >= 100)
            {
                return;
            }

            IsCrafting = true;
            IsCancelled = false;
            Progress = 0;

            OnPropertyChanged(nameof(MessageVisibility));

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(_timeout);
            var token = _cancellationTokenSource.Token;
            token.Register(() => IsCancelled = true);

            _task = new Task(() => Run(token), token);
            _task.Start();
        }

        private void Run(CancellationToken ct)
        {
            var scourCost = _craftingTree.GetScourCost();

            for (var progress = 0d; progress < 100; progress = _craftingTree.GetCurrencySpent(ScourCount, BaseItemCount, BaseItemCost) / Currency* 100)
            {
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }

                var newItem = _factory.CreateEquipment();
                var result = _craftingTree.Craft(newItem, ct);
                EquipmentList.Add(result);

                if (!result.Corrupted && result.Rarity == EquipmentRarity.Normal)
                {
                    continue;
                }

                bool added = false;

                foreach (var prototype in _itemPrototypes)
                {
                    if (prototype.Condition.IsValid(result))
                    {
                        MatchingItems[prototype].Add(result);
                        added = true;
                        BaseItemCount++;
                        break;
                    }
                }

                if (!added)
                {
                    if (!result.Corrupted && result.Rarity != EquipmentRarity.Normal && scourCost < BaseItemCost)
                    {
                        ScourCount++;
                    }
                    else
                    {
                        BaseItemCount++;
                    }
                }

                Progress = progress;
                OnPropertyChanged(nameof(Progress));
            }

            Progress = 100;
            IsCrafting = false;
            OnPropertyChanged(nameof(Progress));


            App.Current.Dispatcher.Invoke((Action) delegate
            {
                OnCompletion?.Invoke();
            });
        }

        public bool CanComplete()
        {
            return !IsCancelled && IsCompleted;
        }

        public void OnClose()
        {
            if (_task != null && _task.Status == TaskStatus.Running)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCraftClick(object sender, RoutedEventArgs e)
        {
            Craft();
        }
    }
}
