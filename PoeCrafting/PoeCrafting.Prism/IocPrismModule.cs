using Microsoft.Practices.Unity;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.Prism
{
    public class IocPrismModule : UnityContainerExtension
    {
        protected override void Initialize()
        {
        }
    }

    public class IocDomainModule : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType(typeof(IItemConfigRepository), typeof(DataRepository), new ContainerControlledLifetimeManager());

            Container.RegisterType(typeof(CraftingCondition));
            Container.RegisterType(typeof(CraftingSubcondition));
            Container.RegisterType(typeof(EquipmentFetch));

            Container.RegisterType(typeof(ICurrency), typeof(AlchemyOrb), "Alchemy");
            Container.RegisterType(typeof(ICurrency), typeof(AlterationOrb), "Alteration");
            Container.RegisterType(typeof(ICurrency), typeof(AugmentationOrb), "Augmentation");
            Container.RegisterType(typeof(ICurrency), typeof(BlessedOrb), "Blessed");

            Container.RegisterType(typeof(ICurrency), typeof(ChaosOrb), "Chaos");
            Container.RegisterType(typeof(ICurrency), typeof(ChanceOrb), "Chance");
            Container.RegisterType(typeof(ICurrency), typeof(DivineOrb), "Divine");
            Container.RegisterType(typeof(ICurrency), typeof(ExaltedOrb), "Exalted");
            Container.RegisterType(typeof(ICurrency), typeof(MasterCraft), "MasterCraft");

            Container.RegisterType(typeof(ICurrency), typeof(RegalOrb), "Regal");
            Container.RegisterType(typeof(ICurrency), typeof(ScouringOrb), "Scouring");
            Container.RegisterType(typeof(ICurrency), typeof(TransmutationOrb), "Transmutation");
            Container.RegisterType(typeof(ICurrency), typeof(VaalOrb), "Vaal");
            Container.RegisterType(typeof(ICurrency), typeof(AnullmentOrb), "Anullment");
        }
    }

    public class IocDataModule : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType(typeof(IRandom), typeof(PoeRandom), new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IFetchCurrencyValues), typeof(FetchCurrencyValues));
            Container.RegisterType(typeof(IFetchAffixesByItemName), typeof(FetchAffixesByItemName));
            Container.RegisterType(typeof(IFetchArmourByItemName), typeof(FetchArmourByItemName));
            Container.RegisterType(typeof(IFetchAccessoriesByItemName), typeof(FetchAccessoriesByItemName));
            Container.RegisterType(typeof(IFetchWeaponsByItemName), typeof(FetchWeaponsByItemName));
            Container.RegisterType(typeof(IFetchItemNamesBySubtype), typeof(FetchItemNamesBySubtype));
            Container.RegisterType(typeof(IFetchTypeByItemName), typeof(FetchTypeByItemName));
            Container.RegisterType(typeof(IFetchSubtypes), typeof(FetchSubtypes));

        }
    }
}
