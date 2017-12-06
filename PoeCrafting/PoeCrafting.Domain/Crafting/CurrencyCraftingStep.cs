using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        [JsonIgnore]
        public List<string> Options => null;

        [JsonIgnore]
        public List<ICraftingStep> Children => null;

        [JsonIgnore]
        public CraftingCondition Condition => null;

        [JsonIgnore]
        public CraftTracker Tracker = new CraftTracker();

        [JsonIgnore]
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

        public Equipment Craft(Equipment equipment, CancellationToken ct)
        {
            bool success = _currency.Execute(equipment);
            Tracker.TrackCraft(equipment, success);
            return equipment;
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action, CancellationToken ct) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action, ct);
        }
    }
}
