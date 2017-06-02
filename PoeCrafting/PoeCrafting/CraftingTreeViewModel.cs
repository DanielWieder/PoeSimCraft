using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI
{
    public class CraftingTreeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CraftingStepViewModel> Tree { get; set; }

        public CraftingTreeViewModel(CraftingTree craftingTree)
        {
            var craftingSteps = craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x));

            Tree = new ObservableCollection<CraftingStepViewModel>(craftingSteps.ToArray());
        }

        public void UpdateTree(CraftingTree craftingTree)
        {
            Tree.Clear();

            foreach (var item in craftingTree.CraftingSteps.Select(x => new CraftingStepViewModel(x)))
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
        private ObservableCollection<CraftingStepViewModel> _children;
        private readonly ICraftingStep _value;
        public bool IsSelected { get; set; }
        public ICraftingStep Value => _value;
        public bool IsVisible => _value.Options.Any();
        public string Name => _value.Name;
        public ObservableCollection<string> Options => new ObservableCollection<string>(_value.Options);
        public ObservableCollection<CraftingStepViewModel> Children => _children;

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

        public CraftingStepViewModel(ICraftingStep craftingStep)
        {
            _value = craftingStep;
            var children = craftingStep.Children.Select(x => new CraftingStepViewModel(x)).ToArray();
            _children = new ObservableCollection<CraftingStepViewModel>(children);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
