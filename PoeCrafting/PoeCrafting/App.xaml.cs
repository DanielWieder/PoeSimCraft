using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
using Ninject.Modules;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Currency;

using PoeCrafting.UI;
using Unity;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = new UnityContainer();

            container.AddNewExtension<IocUiModule>();
            container.AddNewExtension<IocDomainModule>();
            container.AddNewExtension<IocDataModule>();

            var window = container.Resolve<SimulationWindow>();
            window.Show();
        }


    }
}
