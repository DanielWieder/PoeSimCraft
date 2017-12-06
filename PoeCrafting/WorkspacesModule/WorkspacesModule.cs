using Microsoft.Practices.Unity;
using PoeCrafting.Infrastructure;
using Prism.Modularity;
using Prism.Regions;
using WorkspacesModule.ItemBase;

namespace WorkspacesModule
{
    [Module(ModuleName= "WorkspacesModule")]
    [ModuleDependency("")]
    public class WorkspacesModule : IModule
    {
        private IUnityContainer _container;
        private IRegionManager _regionManager;

        public WorkspacesModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.WorkspaceRegion, typeof(ItemBaseView));

            _container.RegisterType(typeof(IWorkspaceFactory), typeof(WorkspaceFactory));
            _container.RegisterType<object, ItemBaseView>(typeof(ItemBaseView).FullName);
            _container.RegisterType(typeof(ItemBaseViewModel));
        }
    }
}
