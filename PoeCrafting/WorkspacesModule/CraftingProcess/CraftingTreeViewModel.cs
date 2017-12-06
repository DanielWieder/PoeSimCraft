using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.CraftingProcess
{
    public class CraftingTreeViewModel : ViewModelBase
    {
        public ObservableCollection<CraftingStepViewModel> Tree { get; set; }

        public CraftingTreeViewModel(CraftingTree craftingTree, ICraftingStep selected)
        {
            var craftingSteps = craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x, selected));

            Tree = new ObservableCollection<CraftingStepViewModel>(craftingSteps.ToArray());
            OnPropertyChanged(nameof(Tree));
        }

        public void UpdateTree(CraftingTree craftingTree, ICraftingStep selected)
        {
            
            Tree.Clear();
            var steps = craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x, selected)).ToList();
            for (int i = 0; i < steps.Count(); i++)
            {
                steps[i].UpdateStatus();
                Tree.Add(steps[i]);
            }

            OnPropertyChanged(nameof(Tree));
        }
    }

    public class CraftingStepViewModel : ViewModelBase
    {
        private readonly ICraftingStep _value;
        private bool _selected = false;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public ObservableCollection<string> Options { get; }
        public bool HasOptions => Options.Any();

        public CraftingCondition Condition => _value.Condition;
        public bool HasCondition => _value.Condition != null;

        public ICraftingStep Value => _value;
        public string Name => _value.Name;

        public ObservableCollection<CraftingStepViewModel> Children { get; }

        public SolidColorBrush BorderBrush
        {
            get
            {
                switch (_value.Status)
                {
                    case CraftingStepStatus.Unreachable:
                        return new SolidColorBrush(Colors.Black);
                    case CraftingStepStatus.Unusable:
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            }
        }

        public void UpdateStatus()
        {
            OnPropertyChanged(nameof(Options));

            foreach (var child in Children)
            {
                child.UpdateStatus();
            }
        }

        public CraftingStepViewModel(ICraftingStep craftingStep, ICraftingStep selected)
        {
            _value = craftingStep;

            Options = craftingStep.Options == null ? new ObservableCollection<string>() : new ObservableCollection<string>(craftingStep.Options);

            if (craftingStep == selected)
            {
                _selected = true;
            }

            if (craftingStep.Children == null)
            {
                Children = new ObservableCollection<CraftingStepViewModel>();
            }
            else
            {
                var children = craftingStep.Children.Select(x => new CraftingStepViewModel(x, selected)).ToArray();
                Children = new ObservableCollection<CraftingStepViewModel>(children);
            }
        }
    }
}
