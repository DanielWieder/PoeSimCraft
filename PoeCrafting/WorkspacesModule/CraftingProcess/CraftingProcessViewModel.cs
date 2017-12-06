using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;
using Prism.Commands;
using WorkspacesModule.Condition;

namespace WorkspacesModule.CraftingProcess
{
    public class CraftingProcessViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        public CraftingTree CraftingTree { get; set; }
        private ICraftingStep _selected;
        private ObservableCollection<Affix> _affixes;
        private PoeCrafting.Entities.ItemBase _itemBase;
        public CraftingTreeViewModel Tree { get; set; }
        public ConditionControlView Condition { get; set; }
        public ConditionControlViewModel ConditionModel { get; set; }

        private ItemConfig _baseInfo = null;

        public CraftingProcessViewModel(CurrencyFactory factory)
        {
            CraftingTree = new CraftingTree(factory);
            Tree = new CraftingTreeViewModel(CraftingTree, _selected);
        }

        public void Initialize(List<Affix> affixes, PoeCrafting.Entities.ItemBase itemBase,
            ItemConfig baseInfo)
        {
            if (_baseInfo == null || !_baseInfo.Equals(baseInfo))
            {
                _itemBase = itemBase;
                _affixes = new ObservableCollection<Affix>(affixes);
                CraftingTree.ClearConditions();
                Condition = null;
                _selected = Tree.Tree[0].Value;
                Tree.UpdateTree(CraftingTree, _selected);
                OnPropertyChanged(nameof(Condition));
                OnPropertyChanged(nameof(Tree));
                _baseInfo = baseInfo;
                CraftingTree.Initialize();
            }
        }

        public void LoadTree(CraftingTree tree)
        {
            CraftingTree = tree;
            Tree = new CraftingTreeViewModel(CraftingTree, tree.CraftingSteps[0]);
            Condition = null;
            OnPropertyChanged(nameof(Condition));
            OnPropertyChanged(nameof(Tree));
        }

        DelegateCommand _selectionChangedCommand;
        public ICommand SelectionChangedCommand => _selectionChangedCommand ?? (_selectionChangedCommand = new DelegateCommand(() => OnInsertOptionSelected(null, null)));

        public void OnInsertOptionSelected(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox.DataContext == BindingOperations.DisconnectedSource)
            {
                return;
            }

            var model = comboBox.DataContext as CraftingStepViewModel;
            var craftingStep = model.Value;
            var selection = comboBox.SelectedItem as string;

            CraftingTree.Replace(craftingStep, selection);

            Tree.UpdateTree(CraftingTree, _selected);
        }

        public void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selected != null)
            {
                CraftingTree.Delete(_selected);

                Tree.UpdateTree(CraftingTree, _selected);
                _selected = null;
            }
        }

        public void OnSelectedStepChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            var model = treeView.SelectedItem as CraftingStepViewModel;

            if (model?.Value == null || _selected == model.Value)
            {
                return;
            }

            var craftingStep = model.Value;

            ConditionModel?.Save();

            if (model.HasCondition)
            {
                ConditionModel = new ConditionControlViewModel(model.Condition, _itemBase, _affixes.ToList());
                Condition = new ConditionControlView(ConditionModel);
                OnPropertyChanged(nameof(Condition));
            }
            else
            {
                Condition = null;
                OnPropertyChanged(nameof(Condition));
            }

            CraftingTree.Select(craftingStep);
            _selected = model.Value;
            Tree.UpdateTree(CraftingTree, _selected);
        }

        public bool CanComplete()
        {
            return true;
        }

        public void OnClose()
        {
            ConditionModel?.Save();
        }

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        public string Error { get; }
    }
}
