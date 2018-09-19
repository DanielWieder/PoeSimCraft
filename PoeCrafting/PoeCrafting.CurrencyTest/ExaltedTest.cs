using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency.Orbs;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    [TestClass]
    public class ExaltedTest
    {
        private CurrencyTestHelper _currencyTestHelper;
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private ExaltedOrb _exalted;
        private RegalOrb _regal;
        private TransmutationOrb _transmutation;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _exalted = new ExaltedOrb(_random.Object);
            _regal = new RegalOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void ExaltedSuccessTest()
        {
            var item = _factory.GetNormal();

            _transmutation.Execute(item);
            _regal.Execute(item);

            var previousAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _exalted.Execute(item);
            Assert.IsTrue(result);
            var currentAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(previousAffixCount + 1, currentAffixCount);
        }

        [TestMethod]
        public void ExaltedTooManyAffixesTest()
        {
            var item = _factory.GetNormal();

            _alchemy.Execute(item);
            _exalted.Execute(item);
            _exalted.Execute(item);
            _exalted.Execute(item);
            
            var result = _exalted.Execute(item);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExaltedCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_exalted));
        }

        [TestMethod]
        public void ExaltedMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_exalted));
        }

        [TestMethod]
        public void ExaltedNormalFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_exalted));
        }
    }
}
