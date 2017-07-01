using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.Modules;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.UI;

namespace PoeCrafting.Test
{
    [TestClass]
    public class CraftingTests
    {
        private StandardKernel _container;
        string testBase = "Vaal Regalia";

        [TestInitialize]
        public void Initialize()
        {
            this._container = new StandardKernel();
            _container.Load(new INinjectModule[]
                {
                    new IocDataModule(),
                    new IocDomainModule()
                }
            );
        }

        [TestMethod]
        public void CraftingTreeCreationTest()
        {
            var tree = _container.Get<CraftingTree>();
            Assert.AreEqual(2, tree.CraftingSteps.Count);
            Assert.AreEqual("Start", tree.CraftingSteps[0].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[1].Name);
        }

        [TestMethod]
        public void CraftingTreeInsertTest()
        {
            var tree = _container.Get<CraftingTree>();
            var selectedOption = tree.AfterSelected.Options[0];
            tree.Replace(tree.AfterSelected, selectedOption);

            Assert.AreEqual("Start", tree.CraftingSteps[0].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[1].Name);
            Assert.AreEqual(selectedOption, tree.CraftingSteps[2].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[3].Name);

            Assert.AreEqual(4, tree.CraftingSteps.Count);
        }

        [TestMethod]
        public void CraftingTreeChildrenTest()
        {
            var tree = _container.Get<CraftingTree>();
            var selectedOption = "If";
            tree.Replace(tree.AfterSelected, selectedOption);

            Assert.AreEqual("Start", tree.CraftingSteps[0].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[1].Name);
            Assert.AreEqual(selectedOption, tree.CraftingSteps[2].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[2].Children[0].Name);
            Assert.AreEqual("Insert", tree.CraftingSteps[3].Name);

            Assert.AreEqual(1, tree.CraftingSteps[2].Children.Count);
            Assert.AreEqual(4, tree.CraftingSteps.Count);
        }

        [TestMethod]
        public void CraftingTreeCraft()
        {
            var equipment = CreateRegalia();

            Mock<ICraftingSubcondition> mockCraftingCondition = new Mock<ICraftingSubcondition>();
            mockCraftingCondition
                .Setup(x => x.IsValid(It.IsAny<Equipment>()))
                .Returns<Equipment>(x => x.Prefixes.Count + x.Suffixes.Count != 6);

            var tree = _container.Get<CraftingTree>();
            tree.Replace(tree.AfterSelected, "Orb of Transmutation");
            tree.Replace(tree.AfterSelected, "Orb of Augmentation");
            tree.Replace(tree.AfterSelected, "Regal Orb");
            tree.Replace(tree.AfterSelected, "While");

     //       var craftingStep = tree.CraftingSteps[tree.CraftingSteps.Count - 2] as WhileCraftingStep;
     //       craftingStep.Condition.CraftingSubConditions.Add(mockCraftingCondition.Object);

            tree.Replace(tree.InsideSelected, "Exalted Orb");

            tree.Craft(equipment);

            Assert.AreEqual(3, equipment.Suffixes.Count);
            Assert.AreEqual(3, equipment.Prefixes.Count);
            mockCraftingCondition.Verify(x => x.IsValid(It.IsAny<Equipment>()), Times.Exactly(4));
        }

        [TestMethod]
        public void CraftingTreeCollapseRarityStatus()
        {
            var tree = _container.Get<CraftingTree>();
            tree.Replace(tree.AfterSelected, "Orb of Chance");
            tree.Replace(tree.AfterSelected, "Regal Orb");
            tree.Replace(tree.AfterSelected, "Chaos Orb");
        
            Assert.AreEqual(CraftingStepStatus.Ok, tree.CraftingSteps[4].Status);
        }

        [TestMethod]
        public void CraftingTreeCraftEndEarly()
        {
            var equipment = CreateRegalia();

            var tree = _container.Get<CraftingTree>();
            tree.Replace(tree.AfterSelected, "End");
            tree.Replace(tree.AfterSelected, "Vaal Orb");
            tree.Craft(equipment);

            Assert.IsFalse(equipment.Corrupted);
        }

        private Equipment CreateRegalia()
        {
            var regaliaFactory = _container.Get<EquipmentFactory>();
            regaliaFactory.Initialize(testBase);
            var equipment = regaliaFactory.CreateEquipment();
            return equipment;
        }
    }
}
