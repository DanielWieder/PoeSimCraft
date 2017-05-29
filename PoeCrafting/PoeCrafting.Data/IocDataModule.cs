using Ninject.Modules;

namespace PoeCrafting.Data
{
    public class IocDataModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRandom>().To<PoeRandom>().InSingletonScope();
            Bind<IFetchCurrencyValues>().To<FetchCurrencyValues>().InTransientScope();
            Bind<IFetchAffixesByItemName>().To<FetchAffixesByItemName>().InTransientScope();
            Bind<IFetchArmourByItemName>().To<FetchArmourByItemName>().InTransientScope();
            Bind<IFetchAccessoriesByItemName>().To<FetchAccessoriesByItemName>().InTransientScope();
            Bind<IFetchWeaponsByItemName>().To<FetchWeaponsByItemName>().InTransientScope();
            Bind<IFetchItemNamesBySubtype>().To<FetchItemNamesBySubtype>().InTransientScope();
            Bind<IFetchTypeByItemName>().To<FetchTypeByItemName>().InTransientScope();
            Bind<IFetchSubtypes>().To<FetchSubtypes>().InTransientScope();
        }
    }
}
