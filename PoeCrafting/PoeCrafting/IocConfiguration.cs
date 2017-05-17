using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ninject.Modules;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Currency;
using PoeCrafting.Entities.Currency;
using PoeCrafting.UI.Pages;

namespace PoeCrafting.UI
{
    class IocConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindow>().ToSelf().InTransientScope();
            Bind<CraftingTestbedModel>().ToSelf().InTransientScope();

            Bind<IRandom>().To<PoeRandom>().InSingletonScope();
            Bind<IFetchAffixesByItemName>().To<FetchAffixesByItemName>().InTransientScope();
            Bind<IFetchArmourByItemName>().To<FetchArmourByItemName>().InTransientScope();
            Bind<IFetchAccessoriesByItemName>().To<FetchAccessoriesByItemName>().InTransientScope();
            Bind<IFetchWeaponsByItemName>().To<FetchWeaponsByItemName>().InTransientScope();
            Bind<IFetchItemNamesBySubtype>().To<FetchItemNamesBySubtype>().InTransientScope();
            Bind<IFetchTypeByItemName>().To<FetchTypeByItemName>().InTransientScope();
            Bind<IFetchSubtypes>().To<FetchSubtypes>().InTransientScope();
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
        }
    }
}
