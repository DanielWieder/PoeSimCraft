using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoeCrafting.Currency;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Domain
{
    public class CraftingSim
    {
        readonly ConditionResolver _conditionResolution = new ConditionResolver();
        private CraftingTree _craftingTree;
        private EquipmentFactory _equipmentFactory;
        private CurrencyFactory _currencyFactory;
        private List<ItemPrototype> _itemPrototypes;

        // generated
        public List<Equipment> EquipmentList = new List<Equipment>();
        public Dictionary<ItemPrototype, List<Equipment>> MatchingItems = new Dictionary<ItemPrototype, List<Equipment>>();

        // local
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        Task _task;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);

        private int _currency;

        private int _scourCount;

        private int _baseItemCount;

        private double _baseItemCost;

        public delegate void ProgressUpdateEventHandler(ProgressUpdateEventArgs e);
        public ProgressUpdateEventHandler OnProgressUpdate;

        public delegate void SimulationCompleteEventHandler(SimulationCompleteEventArgs a);
        public SimulationCompleteEventHandler OnSimulationComplete;
        private double _scourCost;

        public bool IsCancelled { get; set; }

        public CraftingSim()
        {}

        public void Initialize(
            CraftingTree craftingTree, 
            EquipmentFactory factory, 
            List<ItemPrototype> itemPrototypes, 
            double scourCost,
            double baseItemCost, 
            int currency)
        {
            _craftingTree = craftingTree;
            _craftingTree.ClearCurrencySpent();

            _equipmentFactory = factory;
            _itemPrototypes = itemPrototypes.OrderByDescending(x => x.Value).ThenBy(x => x.Name).ToList();
            _currency = currency;
            _baseItemCost = baseItemCost;
            _scourCost = scourCost;

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

            _scourCount = 0;
            _baseItemCount = 0;
        }

        public void Run()
        {
            IsCancelled = false;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(_timeout);
            var token = _cancellationTokenSource.Token;
            token.Register(() => IsCancelled = true);

            _task = new Task(() => Run(token), token);
            _task.Start();
        }

        private void Run(CancellationToken ct)
        {
            for (var progress = 0d; progress < 100; progress = _craftingTree.GetCurrencySpent(_scourCount, _baseItemCount, _baseItemCost) / _currency * 100)
            {
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }

                var newItem = _equipmentFactory.CreateEquipment();
                var result = _craftingTree.Craft(newItem, ct);
                EquipmentList.Add(result);

                if (!result.Corrupted && result.Rarity == EquipmentRarity.Normal)
                {
                    continue;
                }

                bool added = false;

                foreach (var prototype in _itemPrototypes)
                {
                    if (prototype.Condition != null && _conditionResolution.IsValid(prototype.Condition, result))
                    {
                        MatchingItems[prototype].Add(result);
                        added = true;
                        _baseItemCount++;
                        break;
                    }
                }

                if (!added)
                {
                    if (!result.Corrupted && result.Rarity != EquipmentRarity.Normal && _scourCost < _baseItemCount)
                    {
                        _scourCount++;
                    }
                    else
                    {
                        _baseItemCount++;
                    }
                }

                if (OnProgressUpdate != null)
                {
                    var args = new ProgressUpdateEventArgs
                    {
                        Progress = progress
                    };

                    OnProgressUpdate(args);
                }
            }

            if (OnSimulationComplete != null)
            {
                var args = new SimulationCompleteEventArgs
                {
                    ScourCount = _scourCount,
                    BaseItemCost = _baseItemCost,
                    BaseItemCount = _baseItemCount,
                    Items = MatchingItems
                };

                OnSimulationComplete(args);
            }
        }

        public void Cancel()
        {
            if (_task != null && _task.Status == TaskStatus.Running)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }

    public class SimulationCompleteEventArgs
    {
        public int ScourCount { get; set; }
        public double BaseItemCost { get; set; }
        public int BaseItemCount { get; set; }
        public Dictionary<ItemPrototype, List<Equipment>> Items { get; set; }
    }

    public class ProgressUpdateEventArgs
    {
        public double Progress { get; set; }
    }
}
