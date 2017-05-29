using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Ninject;
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
        private IKernel container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var kernel = new StandardKernel();
            kernel.Load("*.dll");
            var window = kernel.Get<MainWindow>();
            window.Show();
        }


    }
}
