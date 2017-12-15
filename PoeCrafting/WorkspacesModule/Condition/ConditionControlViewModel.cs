using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;
using Prism.Commands;

namespace WorkspacesModule.Condition
{
    public class ConditionControlViewModel : ViewModelBase
    {
        private SubconditionControlView _selectedSubcondition;
        private readonly PoeCrafting.Entities.ItemBase _itemBase;
        private readonly List<Affix> _affixes;
        private readonly CraftingCondition _craftingCondition;

        public ICommand AddCommand => new DelegateCommand(AddSubcondition);

        public ObservableCollection<SubconditionControlView> SubconditionControls { get; set; }

        public SubconditionControlView SelectedSubcondition
        {
            get { return _selectedSubcondition; }
            set
            {
                var model = (SubconditionControlViewModel) value.ViewModel;
                model?.Save();
                _selectedSubcondition = value;
                OnPropertyChanged(nameof(SelectedSubcondition));
            }
        }

        public ConditionControlViewModel(CraftingCondition condition, PoeCrafting.Entities.ItemBase itemBase, List<Affix> affixes)
        {
            _itemBase = itemBase;
            _affixes = affixes;
            _craftingCondition = condition;

            SubconditionControls = new ObservableCollection<SubconditionControlView>();

            for (int i = condition.CraftingSubConditions.Count - 1; i >= 0; i--)
            {
                var model = new SubconditionControlViewModel(condition.CraftingSubConditions[i], affixes, GetNextIndex(), _itemBase);
                var subconditionControl = new SubconditionControlView(model);
                model.OnDeleteEvent += (x, y) => RemoveSubcondition(y.Control);
                SubconditionControls.Add(subconditionControl);
            }

            if (SubconditionControls.Count == 0)
            {
                AddSubcondition();
            }

            SelectedSubcondition = SubconditionControls[0];

            OnPropertyChanged(nameof(SelectedSubcondition));
            OnPropertyChanged(nameof(SubconditionControls));
        }

        public void AddSubcondition()
        {
            var subcondition = new CraftingSubcondition();

            var model = new SubconditionControlViewModel(subcondition, _affixes, GetNextIndex(), _itemBase);
            var subconditionControl = new SubconditionControlView(model);
            model.OnDeleteEvent += (x, y) => RemoveSubcondition(y.Control);

            _craftingCondition.CraftingSubConditions.Add(subcondition);
            SubconditionControls.Add(subconditionControl);
            SelectedSubcondition = subconditionControl;
            OnPropertyChanged(nameof(SelectedSubcondition));
            OnPropertyChanged(nameof(SubconditionControls));
        }

        private int GetNextIndex()
        {
            if (!SubconditionControls.Any())
            {
                return 1;
            }

            return SubconditionControls.Select(x => (SubconditionControlViewModel)x.ViewModel).Max(x => x.Index) + 1;
        }

        public void RemoveSubcondition(SubconditionControlViewModel model)
        {
            SubconditionControls.Remove(SubconditionControls.First(x => x.ViewModel == model));
            _craftingCondition.CraftingSubConditions.Remove(model.SubCondition);
            SelectedSubcondition = null;

            if (SubconditionControls.Count == 0)
            {
                AddSubcondition();
            }

            SelectedSubcondition = SubconditionControls[0];

            OnPropertyChanged(nameof(SelectedSubcondition));
            OnPropertyChanged(nameof(SubconditionControls));
        }

        public void Save()
        {
            _craftingCondition.CraftingSubConditions.Clear();
            foreach (var subcondition in SubconditionControls.Select(x => (SubconditionControlViewModel)x.ViewModel).Select(x => x.Save()))
            {
                _craftingCondition.CraftingSubConditions.Add(subcondition);
            }
        }
    }
}
