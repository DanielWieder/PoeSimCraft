using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Crafting
{
    public class InsertCraftingStep : ICraftingStep
    {
        private ItemStatus _status = new ItemStatus();
        private readonly  CurrencyFactory _currencyFactory;

        public List<ICraftingStep> Children => new List<ICraftingStep>();
        public bool HasWarning => false;
        public bool HasError => false;
        public bool IsCompleted => false;
        public string Name => "Insert";

        public InsertCraftingStep(CurrencyFactory factory)
        {
            _currencyFactory = factory;
        }

        public List<string> Options
        {
            get
            {
                var currencyList = _currencyFactory.GetValidCurrency(_status).Select(x => x.Name);

                List<string> options = new List<string>
                {
                    "End"
                };
                options.AddRange(currencyList);

                return options;
            }
        }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            _status = (ItemStatus)_status.Clone();
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            return equipment;
        }
    }
}
