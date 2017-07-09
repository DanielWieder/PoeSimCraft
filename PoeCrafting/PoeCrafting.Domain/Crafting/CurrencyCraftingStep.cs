using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class CurrencyCraftingStep : ICraftingStep
    {
        private readonly ICurrency _currency;
        private ItemStatus _status = new ItemStatus();
        public string Name => _currency.Name;

        public List<string> Options => null;
        public List<ICraftingStep> Children => null;
        public CraftingCondition Condition => null;

        public CraftTracker Tracker = new CraftTracker();
        public double Value => _currency.Value;

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

        public CurrencyCraftingStep(ICurrency currency)
        {
            this._currency = currency;
        }

        public void ClearStatus()
        {
            _status = new ItemStatus();
        }

        public ItemStatus UpdateStatus(ItemStatus current)
        {
            _status = (ItemStatus)current.Clone();

            return _currency.GetNextStatus(current);
        }

        public Equipment Craft(Equipment equipment)
        {
            bool success = _currency.Execute(equipment);
            Tracker.TrackCraft(equipment, success);
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
