using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.Modules;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Domain.Currency;
using PoeCrafting.UI;


namespace PoeCrafting.Test
{
    [TestClass]
    public class CurrencyTests
    {
        string testBase = "Vaal Regalia";

        private StandardKernel container;

        [TestInitialize]
        public void Initialize()
        {
            this.container = new StandardKernel();
            container.Load(new INinjectModule[] {
                new IocDataModule(),
                new IocDomainModule()}
            );
        }

        [TestMethod]
        public void TestItemCreation()
        {
            EquipmentType testType = EquipmentType.Armour;
            string testSubtype = "Body Armour";
            string testBase = "Vaal Regalia";

            EquipmentFetch fetch = container.Get<EquipmentFetch>();

            var bases = fetch.FetchBasesBySubtype(testSubtype);

            Assert.IsTrue(bases.Contains(testBase));
            var regaliaIndex = bases.IndexOf(testBase);

            var regaliaFactory = container.Get<EquipmentFactory>();
            regaliaFactory.Initialize(bases[regaliaIndex]);

            var regalia = regaliaFactory.CreateEquipment();

            Assert.IsNotNull(regalia);
        }

        [TestMethod]
        public void TestAlterationOrbLowPrefixRoll()
        {
            Mock<IRandom> random = new Mock<IRandom>();

            random.Setup(x => x.Next(It.IsAny<int>())).Returns(0);
            random.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            random.SetupSequence(x => x.NextDouble()).Returns(0);

            container.Rebind<IRandom>().ToConstant(random.Object);

            var regalia = GetMagicRegalia();

            Assert.AreEqual(0, regalia.Suffixes.Count);
            Assert.AreEqual(1, regalia.Prefixes.Count);
        }

        [TestMethod]
        public void TestAlterationOrbHighPrefixRoll()
        {
            Mock<IRandom> random = new Mock<IRandom>();

            random.Setup(x => x.Next(It.IsAny<int>())).Returns(0);
            random.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            random.SetupSequence(x => x.NextDouble()).Returns(1);

            container.Rebind<IRandom>().ToConstant(random.Object);

            var regalia = GetMagicRegalia();

            Assert.AreEqual(0, regalia.Suffixes.Count);
            Assert.AreEqual(1, regalia.Prefixes.Count);
        }

        [TestMethod]
        public void TestAlterationOrbMidPrefixRoll()
        {
            Mock<IRandom> random = new Mock<IRandom>();

            random.Setup(x => x.Next(It.IsAny<int>())).Returns(0);
            random.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            random.SetupSequence(x => x.NextDouble()).Returns(0.5);

            container.Rebind<IRandom>().ToConstant(random.Object);

            var regalia = GetMagicRegalia();
            Assert.AreEqual(0, regalia.Suffixes.Count);
            Assert.AreEqual(1, regalia.Prefixes.Count);
        }

        [TestMethod]
        public void TestAlterationOrbSuffixRoll()
        {
            Mock<IRandom> random = new Mock<IRandom>();

            random.Setup(x => x.Next(It.IsAny<int>())).Returns(1);
            random.Setup(x => x.Next(It.IsAny<int>(), It.IsAny<int>())).Returns(0);
            random.SetupSequence(x => x.NextDouble()).Returns(1);

            container.Rebind<IRandom>().ToConstant(random.Object);

            var regalia = GetMagicRegalia();

            Assert.AreEqual(1, regalia.Suffixes.Count);
            Assert.AreEqual(0, regalia.Prefixes.Count);
        }

        [TestMethod]
        public void CraftingSteps()
        {
            ICurrency trans = container.Get<ICurrency>("Transmutation");
            ICurrency aug = container.Get<ICurrency>("Augmentation");
            ICurrency alt = container.Get<ICurrency>("Alteration");
            ICurrency regal = container.Get<ICurrency>("Regal");
            ICurrency exalt = container.Get<ICurrency>("Exalted");
            ICurrency scour = container.Get<ICurrency>("Scouring");
            ICurrency chaos = container.Get<ICurrency>("Chaos");
            ICurrency alch = container.Get<ICurrency>("Alchemy");

            ItemStatus status = new ItemStatus();

            status.Initialized = true;

            status = trans.GetNextStatus(status);
            Assert.IsTrue(status.Validate());
            Assert.IsTrue(aug.IsWarning(status));
            Assert.IsFalse(aug.IsError(status));

            status = aug.GetNextStatus(status);
            Assert.IsTrue(aug.IsError(status));

            Assert.IsTrue(status.Validate());
            Assert.IsTrue(chaos.IsError(status));
            status = regal.GetNextStatus(status);
            Assert.IsTrue(status.Validate());
            status = chaos.GetNextStatus(status);
            Assert.IsTrue(status.Validate());

            for (int i = 0; i < 2; i++)
            {
                Assert.IsTrue(exalt.IsWarning(status));
                Assert.IsFalse(exalt.IsError(status));
                status = exalt.GetNextStatus(status);
                Assert.IsTrue(status.Validate());
            }
            Assert.IsTrue(exalt.IsError(status));

            status = scour.GetNextStatus(status);
            status = alch.GetNextStatus(status);

            Assert.IsTrue(trans.IsError(status));
            Assert.IsTrue(aug.IsError(status));
            Assert.IsTrue(alt.IsError(status));
        }

        [TestMethod]
        public void CurrencySpam()
        {
            ICurrency trans = container.Get<ICurrency>("Transmutation");
            ICurrency aug = container.Get<ICurrency>("Augmentation");
            ICurrency alt = container.Get<ICurrency>("Alteration");
            ICurrency regal = container.Get<ICurrency>("Regal");
            ICurrency exalt = container.Get<ICurrency>("Exalted");
            ICurrency scour = container.Get<ICurrency>("Scouring");
            ICurrency chaos = container.Get<ICurrency>("Chaos");
            ICurrency alch = container.Get<ICurrency>("Alchemy");

            var regaliaFactory = container.Get<EquipmentFactory>();
            regaliaFactory.Initialize(testBase);
            var regalia = regaliaFactory.CreateEquipment();

            Assert.AreEqual(EquipmentRarity.Normal, regalia.Rarity);

            trans.Execute(regalia);
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count <= 2);

            alt.Execute(regalia);
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count <= 2);

            aug.Execute(regalia);
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count == 2);

            Assert.AreEqual(EquipmentRarity.Magic, regalia.Rarity);

            regal.Execute(regalia);
            Assert.AreEqual(EquipmentRarity.Rare, regalia.Rarity);
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count == 3);

            for (int i = 4; i <= 6; i++)
            {
                exalt.Execute(regalia);
                Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count == i);
            }

            Assert.AreEqual(false, exalt.Execute(regalia));
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count == 6);

            scour.Execute(regalia);

            Assert.AreEqual(EquipmentRarity.Normal, regalia.Rarity);

            Assert.AreEqual(false, chaos.Execute(regalia));

            alch.Execute(regalia);
            Assert.IsTrue(regalia.Prefixes.Count + regalia.Suffixes.Count >= 2);
        }

        private Equipment GetMagicRegalia()
        {
            var regaliaFactory = container.Get<EquipmentFactory>();
            regaliaFactory.Initialize(testBase);

            var regalia = regaliaFactory.CreateEquipment();

            ICurrency trans = container.Get<ICurrency>("Transmutation");

            trans.Execute(regalia);
            return regalia;
        }
    }
}
