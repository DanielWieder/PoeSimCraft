using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Currency;

namespace PoeCrafting.Domain.Crafting
{
    public class CurrencyCraftingStep : ICraftingStep
    {
        private readonly ICurrency _currency;
        private ItemStatus _status = new ItemStatus();
        private CraftTracker tracker = new CraftTracker();

        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public bool HasWarning => _currency.IsWarning(_status);
        public bool HasError => _currency.IsError(_status);
        public bool IsCompleted => false;
        public string Name => _currency.Name;
        public bool HasChildren => false;

        public int TimesUsedCount => tracker.SuccessfulUsesCount;

        public int ItemsUsedOnCount => tracker.ItemsUsedOnCount;

        public double CurrencyUsed => tracker.SuccessfulUsesCount * _currency.Value;
        

        public CurrencyCraftingStep(ICurrency currency)
        {
            this._currency = currency;
        }

        public ItemStatus UpdateStatus(ItemStatus current)
        {
            _status = (ItemStatus)current.Clone();
            return _currency.GetNextStatus(current);
        }

        public Equipment Craft(Equipment equipment)
        {
            bool success = _currency.Execute(equipment);

            tracker.TrackCraft(equipment, success);

            return equipment;
        }
    }
}
