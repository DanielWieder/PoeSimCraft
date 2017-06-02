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

            var kernel = new StandardKernel();
            kernel.Load(new INinjectModule[]
                {
                    new IocDataModule(),
                    new IocDomainModule(),
                    new IocUiModule()
                }
            );

            var window = kernel.Get<HomeWindow>();
            window.Show();
        }


    }
}
