using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency.Orbs;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    [TestClass]
    public class ChaosTest
    {
        private AlchemyOrb _alchemy;
        private Mock<IRandom> _random;
        private TestEquipmentFactory _factory;
        private ChaosOrb _chaos;
        private CurrencyTestHelper _currencyTestHelper;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _chaos = new ChaosOrb(_random.Object);
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void ChaosSuccessTest()
        {
            var item = _factory.GetNormal();

            _alchemy.Execute(item);

            var result = _chaos.Execute(item);
            Assert.IsTrue(result);

            Assert.IsTrue(item.Suffixes.Count >= 1);
            Assert.IsTrue(item.Prefixes.Count >= 1);
            Assert.AreEqual(EquipmentRarity.Rare, item.Rarity);
        }

        [TestMethod]
        public void ChaosCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_chaos));
        }

        [TestMethod]
        public void ChaosMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_chaos));
        }

        [TestMethod]
        public void ChaosNormalFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_chaos));
        }
    }
}
