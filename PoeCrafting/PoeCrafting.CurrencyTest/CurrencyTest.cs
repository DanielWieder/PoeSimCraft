using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Orbs;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    public class CurrencyTestHelper
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;

        public CurrencyTestHelper()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);
        }

        public bool CanUseOnNormal(ICurrency currency)
        {
            var item = _factory.GetNormal();
            return currency.Execute(item);
        }

        public bool CanUseOnMagic(ICurrency currency)
        {
            var item = _factory.GetNormal();
            _transmutation.Execute(item);
            return currency.Execute(item);
        }

        public bool CanUseOnRare(ICurrency currency)
        {
            var item = _factory.GetNormal();
            _alchemy.Execute(item);
            return currency.Execute(item);
        }

        public bool CanUseOnCorruptedNormal(ICurrency currency)
        {
            var item = _factory.GetNormal();
            item.Corrupted = true;
            return currency.Execute(item);
        }

        public bool CanUseOnCorruptedMagic(ICurrency currency)
        {
            var item = _factory.GetNormal();
            _transmutation.Execute(item);
            item.Corrupted = true;
            return currency.Execute(item);
        }

        public bool CanUseOnCorruptedRare(ICurrency currency)
        {
            var item = _factory.GetNormal();
            _alchemy.Execute(item);
            item.Corrupted = true;
            return currency.Execute(item);
        }
    }
}
