using System;

using Ninject.Modules;
using PoeCrafting.UI.Pages;

namespace PoeCrafting.UI
{
    public class IocUiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<SimulationWindow>().ToSelf().InTransientScope();
            Bind<HomeWindow>().ToSelf().InTransientScope();
            Bind<TestbedWindow>().ToSelf().InTransientScope();
            Bind<CraftingTestbedModel>().ToSelf().InTransientScope();
        }
    }
}
