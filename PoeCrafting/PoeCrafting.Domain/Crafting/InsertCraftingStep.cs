﻿using System;
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
        public CraftingStepStatus Status => _status.Initialized ? CraftingStepStatus.Ok : CraftingStepStatus.Unreachable;
        public string Name => "Insert";
        public bool HasChildren => false;

        public InsertCraftingStep(CurrencyFactory factory)
        {
            _currencyFactory = factory;
        }

        public List<string> Options
        {
            get
            {
                var currencyList = _currencyFactory.GetValidCurrency(_status).Select(x => x.Name);
                List<string> options = new List<string>();

                options.AddRange(currencyList);

                options.AddRange(new[]{
                    "If",
                    "While",
                    "End"
                });

                return options;
            }
        }

        public void ClearStatus()
        {
            _status = new ItemStatus();
        }

        public ItemStatus UpdateStatus(ItemStatus status)
        {
            _status = (ItemStatus)status.Clone();
            return status;
        }

        public Equipment Craft(Equipment equipment)
        {
            throw new InvalidOperationException("All Insert crafting steps should be removed before an item is crafted");
        }

        public T NavigateTree<T>(T item, List<ICraftingStep> queue, Func<ICraftingStep, T, T> action) where T : ITreeNavigation
        {
            return this.DefaultNavigateTree(item, queue, action);
        }
    }
}
