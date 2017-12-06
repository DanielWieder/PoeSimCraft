using System;
using System.Collections.Generic;
using System.Windows;
using PoeCrafting.Domain;
using PoeCrafting.Infrastructure;
using Prism.Commands;
using Prism.Regions;
using WorkspacesModule.ItemBase;

namespace WorkspacesModule
{
    public class WorkspaceFactory : IWorkspaceFactory
    {
        public List<CommandViewModel> Workspaces { get; }
        public Action<WorkspaceViewModel> ActivateWorkspace { get; set; }

        private readonly CommandViewModel _itemInput;
        private readonly CommandViewModel _craftingProcess;
        private readonly CommandViewModel _itemSelection;
        private readonly CommandViewModel _craftItems;
        private readonly CommandViewModel _viewResults;

        private readonly IItemConfigRepository _configRepository;
        private readonly EquipmentFetch _equipmentFetch;
        private readonly CurrencyFactory _currencyFactory;
        private readonly IRegionManager _regionManager;

        public WorkspaceFactory(
            IItemConfigRepository configRepository, 
            EquipmentFetch equipmentFetch, 
            CurrencyFactory currencyFactory,
            IRegionManager regionManager)
        {
            _configRepository = configRepository;
            _equipmentFetch = equipmentFetch;
            _currencyFactory = currencyFactory;
            _regionManager = regionManager;

            _itemInput = new CommandViewModel(
                "Item Info",
                new DelegateCommand(() => Navigate(typeof(ItemBaseView).FullName)),
                () => true);

            _craftingProcess = new CommandViewModel(
                "Crafting Process",
                new DelegateCommand(() => ActivateWorkspace(CreateCraftingProcess())),
                () => _configRepository.GetItemConfig().IsValid);

            _itemSelection = new CommandViewModel(
                "Item Selection",
                new DelegateCommand(() => ActivateWorkspace(CreateItemSelection())),
                () => _configRepository.GetItemConfig().IsValid);

            _craftItems = new CommandViewModel(
                "Craft Items",
                new DelegateCommand(() => ActivateWorkspace(CreateCraftItems())),
                () => false);

            _viewResults = new CommandViewModel(
                "View Results",
                new DelegateCommand(() => ActivateWorkspace(CreateViewResults())),
                () => false);

            Workspaces = new List<CommandViewModel>
            {
                _itemInput,
                _craftingProcess,
                _itemSelection,
                _craftItems,
                _viewResults
            };
        }

        private void Navigate(object navigatePath)
        {
            if (navigatePath != null)
            {
                _regionManager.RequestNavigate(RegionNames.WorkspaceRegion, navigatePath.ToString());
            }
        }

        public void SubscribeConfigChanged(EventHandler configChanged)
        {
            configChanged += (x, y) => _itemInput.UpdateIsEnabled();
            configChanged += (x, y) => _craftingProcess.UpdateIsEnabled();
        }

        WorkspaceViewModel CreateInputItemInfo()
        {
            return new ItemBaseViewModel(_configRepository, _equipmentFetch, _configRepository.GetItemConfig());
        }

        WorkspaceViewModel CreateCraftingProcess()
        {
            MessageBoxResult result = MessageBox.Show("Create the crafting process here");
            return null;
            //new CraftingProcessViewModel(_currencyFactory);
        }

        WorkspaceViewModel CreateItemSelection()
        {
            MessageBoxResult result = MessageBox.Show("Select items here");
            return null;
        }

        WorkspaceViewModel CreateCraftItems()
        {
            MessageBoxResult result = MessageBox.Show("Craft items here");
            return null;
        }

        WorkspaceViewModel CreateViewResults()
        {
            MessageBoxResult result = MessageBox.Show("View results here");
            return null;
        }
    }
}
