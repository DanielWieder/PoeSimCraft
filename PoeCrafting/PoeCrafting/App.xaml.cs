using System.Windows;
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
