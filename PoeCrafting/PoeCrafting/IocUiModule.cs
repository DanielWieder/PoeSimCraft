﻿using System;

using Ninject.Modules;
using PoeCrafting.UI.Controls;
using PoeCrafting.UI.Pages;

namespace PoeCrafting.UI
{
    public class IocUiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ConditionControl>().ToSelf().InTransientScope();
            Bind<SubconditionControl>().ToSelf().InTransientScope();
            Bind<SubconditionSelectionControl>().ToSelf().InTransientScope();
            Bind<BaseSelectionControl>().ToSelf().InTransientScope();
            Bind<CraftingTreeControl>().ToSelf().InTransientScope();
            Bind<ConditionControl>().ToSelf().InTransientScope();
            Bind<SimulationWindow>().ToSelf().InTransientScope();
            Bind<HomeWindow>().ToSelf().InTransientScope();
            Bind<TestbedWindow>().ToSelf().InTransientScope();
            Bind<CraftingTestbedModel>().ToSelf().InTransientScope();
        }
    }
}
