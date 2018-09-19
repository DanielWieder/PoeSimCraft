using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency.Orbs;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest
{
    [TestClass]
    public class AnullmentTest
    {
        private AlchemyOrb _alchemy;
        private Mock<IRandom> _random;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;
        private AnullmentOrb _anullment;
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
            _anullment = new AnullmentOrb(_random.Object);
            _vaal = new VaalOrb(_random.Object);
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void AnullmentMagicTest()
        {
            var item = _factory.GetNormal();
            _transmutation.Execute(item);
            var oldAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _anullment.Execute(item);
            Assert.IsTrue(result);
            var newAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(oldAffixCount, newAffixCount + 1);
        }

        [TestMethod]
        public void AnullmentRareTest()
        {
            var item = _factory.GetNormal();
            _alchemy.Execute(item);
            var oldAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _anullment.Execute(item);
            Assert.IsTrue(result);
            var newAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(oldAffixCount, newAffixCount + 1);
        }

        [TestMethod]
        public void AnullmentInvalidNormalTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidCorruptionMagicTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidCorruptionRareTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidNoAffixesTest()
        {
            var item = _factory.GetNormal();
            _transmutation.Execute(item);
            _anullment.Execute(item);
            _anullment.Execute(item);
            var result = _anullment.Execute(item);
            Assert.IsFalse(result);
        }
    }
}
