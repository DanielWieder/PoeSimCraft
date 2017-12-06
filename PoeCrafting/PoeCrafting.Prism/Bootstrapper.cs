
using System;
using System.Windows;
using PoeCrafting.Data;
using PoeCrafting.Prism;
using Prism.Unity;
using Microsoft.Practices.Unity;
using PoeCrafting.Domain;
using Prism.Modularity;

namespace BootstrapperShell
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.AddNewExtension<IocPrismModule>();
            Container.AddNewExtension<IocDomainModule>();
            Container.AddNewExtension<IocDataModule>();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window) Shell;
            Application.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            // Load through directory
            return Prism.Modularity.ModuleCatalog.CreateFromXaml(new Uri("/PoeCrafting.Prism;component/XamlCatalog.xaml", UriKind.Relative));
        }
    }
}