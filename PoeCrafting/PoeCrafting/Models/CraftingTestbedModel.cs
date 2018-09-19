using System.Collections.Generic;
using PoeCrafting.Currency;
using PoeCrafting.Domain;
using PoeCrafting.Entities;
using ItemStatus = PoeCrafting.Entities.ItemStatus;

namespace PoeCrafting.UI.Pages
{
    public class CraftingTestbedModel
    {
        public string SelectedSubtype { get; set; }
        public string SelectedItemBase { get; set; }
        public Equipment Equipment { get; set; }
        public ItemStatus Status { get; set; }
        public CurrencyFactory CurrencyFactory { get; set; }
        public List<string> BaseItemNames { get; set; }
        public List<string> ItemSubtypeNames { get; set; }
        public int Category { get; set; }

        private readonly EquipmentFetch _equipmentFetch;
        private readonly EquipmentFactory _factory;

        public CraftingTestbedModel(EquipmentFactory factory, CurrencyFactory currencyFactory, EquipmentFetch equipmentFetch)
        {
            _equipmentFetch = equipmentFetch;
            _factory = factory;
            CurrencyFactory = currencyFactory;
            ItemSubtypeNames = equipmentFetch.FetchSubtypes();
            Status = new ItemStatus();
        }

        public void SelectSubtype(string subtype)
        {
            SelectedSubtype = subtype;
            BaseItemNames = _equipmentFetch.FetchBasesBySubtype(subtype);
        }

        public void SelectBase(string itemBase)
        {
            SelectedItemBase = itemBase;
        }

        public void CreateItem()
        {
            _factory.Initialize(SelectedItemBase, Category);
            Equipment = _factory.CreateEquipment();
            Status = new ItemStatus();
        }

        public void Reset()
        {
            if (_factory != null)
            {
                Equipment = _factory.CreateEquipment();
                Status = new ItemStatus();
            }
        }

        public void Craft(ICurrency currency)
        {
            if (Equipment != null)
            {
                currency.Execute(Equipment);
                Status = currency.GetNextStatus(Status);
            }
        }

    }
}
