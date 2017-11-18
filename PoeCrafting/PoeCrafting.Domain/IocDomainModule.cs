using Ninject.Modules;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.UI
{
    public class IocDomainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CraftingCondition>().ToSelf().InTransientScope();
            Bind<CraftingSubcondition>().ToSelf().InTransientScope();

            Bind<EquipmentFetch>().ToSelf().InTransientScope();

            Bind<ICurrency>().To<AlchemyOrb>().Named("Alchemy");
            Bind<ICurrency>().To<AlterationOrb>().Named("Alteration");
            Bind<ICurrency>().To<AugmentationOrb>().Named("Augmentation");
            Bind<ICurrency>().To<BlessedOrb>().Named("Blessed");

            Bind<ICurrency>().To<ChaosOrb>().Named("Chaos");
            Bind<ICurrency>().To<ChanceOrb>().Named("Chance");
            Bind<ICurrency>().To<DivineOrb>().Named("Divine");
            Bind<ICurrency>().To<ExaltedOrb>().Named("Exalted");
            Bind<ICurrency>().To<MasterCraft>().Named("MasterCraft");

            Bind<ICurrency>().To<RegalOrb>().Named("Regal");
            Bind<ICurrency>().To<ScouringOrb>().Named("Scouring");
            Bind<ICurrency>().To<TransmutationOrb>().Named("Transmutation");
            Bind<ICurrency>().To<VaalOrb>().Named("Vaal");
            Bind<ICurrency>().To<AnullmentOrb>().Named("Anullment");
        }
    }
}
