using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Data;
using PoeCrafting.Domain.Currency;

namespace PoeCrafting.Domain
{
    public class CurrencyFactory
    {
        public readonly List<ICurrency> Currency;
        private readonly IFetchCurrencyValues _currencyFetch;
        public CurrencyFactory(
            IFetchCurrencyValues currencyValueFetch,
            [Named("Transmutation")] ICurrency transmutation,
            [Named("Alteration")] ICurrency alteration,
            [Named("Augmentation")] ICurrency augmentation,
            [Named("Alchemy")] ICurrency alchemy,
            [Named("Chaos")] ICurrency chaos,
            [Named("Regal")] ICurrency regal,
        //    [Named("Blessed")] ICurrency blessed,
            [Named("Chance")] ICurrency chance,
            [Named("Divine")] ICurrency divine,
            [Named("Exalted")] ICurrency exalted,
        //    [Named("MasterCraft")] ICurrency masterCraft,
            [Named("Scouring")] ICurrency scouring,
            [Named("Vaal")] ICurrency vaal
        )
        {
            Currency = new List<ICurrency>
            {
                transmutation,
                alteration,
                augmentation,
                alchemy,
                chaos,
                regal,
                // blessed,
                chance,
                divine,
                exalted,
                // masterCraft,
                scouring,
                vaal
            };

            _currencyFetch = currencyValueFetch;
        }

        public void UpdateValues(string leagueName)
        {
            _currencyFetch.League = leagueName;
            var data = _currencyFetch.Execute();
            foreach (var currency in Currency)
            {
                if (data.ContainsKey(currency.Name))
                {
                    currency.Value = data[currency.Name];
                }
            }

            Currency.First(x => x.Name == "Chaos Orb").Value = 1;
        }

        public ICurrency GetCurrencyByName(string name)
        {
            return Currency.First(x => x.Name == name);
        }

        public List<ICurrency> GetValidCurrency(ItemStatus status)
        {
            return Currency.Where(x => !x.IsError(status)).ToList();
        }
    }
}
