using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class CurrencyCraftingStep : ICraftingStep
    {
        private readonly ICurrency _currency;
        private ItemStatus _status = new ItemStatus();
        private CraftTracker _tracker = new CraftTracker();

        public List<ICraftingStep> Children => new List<ICraftingStep>();

        public CraftingStepStatus Status
        {
            get
            {
                if (!_status.Initialized)
                {
                    return CraftingStepStatus.Unreachable;
                }
                if (_currency.IsWarning(_status))
                {
                    return CraftingStepStatus.Inconsistent;
                }
                if (_currency.IsError(_status))
                {
                    return CraftingStepStatus.Unusable;
                }
                else
                {
                    return CraftingStepStatus.Ok;
                }
            }
        }

        public bool IsCompleted => false;
        public string Name => _currency.Name;

        public bool HasChildren => false;
        public int TimesUsedCount => _tracker.SuccessfulUsesCount;
        public int ItemsUsedOnCount => _tracker.ItemsSuccessfullyUsedOnCount;
        public double CurrencyUsed => _tracker.SuccessfulUsesCount * _currency.Value;

        public CurrencyCraftingStep(ICurrency currency)
        {
            this._currency = currency;
        }

        public void ClearStatus()
        {
            _status = new ItemStatus();
            _tracker.Clear();
        }

        public ItemStatus UpdateStatus(ItemStatus current)
        {
            if (!_status.Initialized)
            {
                _status = current;
            }
            else
            {
                // We combine the status since update status can be run multiple times due to if/while statements
                _status = current.Initialized ? ItemStatus.Combine(new List<ItemStatus> { current, _status }) : current;
            }

            return _currency.GetNextStatus(current);
        }

        public Equipment Craft(Equipment equipment)
        {
            bool success = _currency.Execute(equipment);
            _tracker.TrackCraft(equipment, success);
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
