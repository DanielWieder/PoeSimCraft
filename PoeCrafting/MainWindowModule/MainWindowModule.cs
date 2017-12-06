using MainWindowModule.MainWindow;
using Microsoft.Practices.Unity;
using PoeCrafting.Infrastructure;
using Prism.Modularity;
using Prism.Regions;

namespace MainWindowModule
{
    [Module(ModuleName= "MainWindowModule")]
    [ModuleDependency("WorkspacesModule")]
    public class MainWindowModule : IModule
    {
        private IUnityContainer _container;
        private IRegionManager _regionManager;

        public MainWindowModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<ToolbarView>();
            _container.RegisterType<WorkspaceManagerView>();
            _container.RegisterType<WorkspaceManagerViewModel>();

            _regionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(ToolbarView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainWindowRegion, typeof(WorkspaceManagerView));
        }
    }
}
