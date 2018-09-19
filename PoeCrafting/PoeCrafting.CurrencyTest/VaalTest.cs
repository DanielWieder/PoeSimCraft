using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency.Orbs;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    [TestClass]
    public class VaalTest
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;
        private VaalOrb _vaal;
        private CurrencyTestHelper _currencyTestHelper;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);
            _vaal = new VaalOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void VaalNormalTest()
        {
            var item = _factory.GetNormal();
            var result = _vaal.Execute(item);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalMagicTest()
        {
            var item = _factory.GetNormal();
            _transmutation.Execute(item);
            var result = _vaal.Execute(item);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalRareTest()
        {
            var item = _factory.GetNormal();
            _alchemy.Execute(item);
            var result = _vaal.Execute(item);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_vaal));
        }
    }
}