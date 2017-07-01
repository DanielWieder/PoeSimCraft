using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI
{
    public class CraftingTreeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CraftingStepViewModel> Tree { get; set; }

        public CraftingTreeViewModel(CraftingTree craftingTree, ICraftingStep selected)
        {
            var craftingSteps = craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x, selected));

            Tree = new ObservableCollection<CraftingStepViewModel>(craftingSteps.ToArray());
        }

        public void UpdateTree(CraftingTree craftingTree, ICraftingStep selected)
        {
            Tree.Clear();

            foreach (var item in craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x, selected)))
            {
                item.UpdateStatus();
                Tree.Add(item);
            }

            OnPropertyChanged(nameof(Tree));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CraftingStepViewModel : INotifyPropertyChanged
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

            if (craftingStep.Children == null)
            {
                Children = new ObservableCollection<CraftingStepViewModel>();
            }
            else
            {
                var children = craftingStep.Children.Select(x => new CraftingStepViewModel(x, selected)).ToArray();

                if (craftingStep == selected)
                {
                    _selected = true;
                }

                Children = new ObservableCollection<CraftingStepViewModel>(children);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
