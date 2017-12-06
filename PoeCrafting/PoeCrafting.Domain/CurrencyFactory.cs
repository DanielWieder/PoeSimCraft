using System.Collections.Generic;
using System.Linq;
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
            TransmutationOrb transmutation,
            AlterationOrb alteration,
            AugmentationOrb augmentation,
            AlchemyOrb alchemy,
            ChaosOrb chaos,
            RegalOrb regal,
        //    [Named("Blessed")] ICurrency blessed,
            ChanceOrb chance,
            DivineOrb divine,
            ExaltedOrb exalted,
        //    [Named("MasterCraft")] ICurrency masterCraft,
            ScouringOrb scouring,
            VaalOrb vaal,
            AnullmentOrb anull
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
                vaal,
                anull
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
