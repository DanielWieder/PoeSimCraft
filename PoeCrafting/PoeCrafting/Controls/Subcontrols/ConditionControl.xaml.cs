using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class ConditionControl : UserControl, INotifyPropertyChanged
    {
        private SubconditionControl _selectedSubcondition;
        private readonly ItemBase _itemBase;
        private readonly List<Affix> _affixes;
        private readonly CraftingCondition _craftingCondition;

        public ObservableCollection<SubconditionControl> SubconditionControls { get; set; }

        public SubconditionControl SelectedSubcondition
        {
            get { return _selectedSubcondition; }
            set
            {
                _selectedSubcondition?.Save();
                _selectedSubcondition = value;
                OnPropertyChanged(nameof(SelectedSubcondition));
            }
        }

        public ConditionControl(CraftingCondition condition, ItemBase itemBase, List<Affix> affixes )
        {
            _itemBase = itemBase;
            _affixes = affixes;
            _craftingCondition = condition;

            SubconditionControls = new ObservableCollection<SubconditionControl>();

            for (int i = condition.CraftingSubConditions.Count - 1; i >= 0; i--)
            {
                var subconditionControl = new SubconditionControl(condition.CraftingSubConditions[i], affixes, GetNextIndex(), _itemBase);
                subconditionControl.OnDeleteEvent += (x, y) => RemoveSubcondition(y.Control);
                SubconditionControls.Add(subconditionControl);
            }

            if (SubconditionControls.Count == 0)
            {
                AddSubcondition();
            }

            SelectedSubcondition = SubconditionControls[0];

            OnPropertyChanged(nameof(SelectedSubcondition));
            OnPropertyChanged(nameof(SubconditionControls));

            DataContext = this;
            InitializeComponent();
        }

        public void AddSubcondition()
        {
            var subcondition = new CraftingSubcondition();
            var subconditionControl = new SubconditionControl(subcondition, _affixes, GetNextIndex(), _itemBase);
            subconditionControl.OnDeleteEvent += (x, y) => RemoveSubcondition(y.Control);

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

            return SubconditionControls.Max(x => x.Index) + 1;
        }

        public void RemoveSubcondition(SubconditionControl subcondition)
        {
                SubconditionControls.Remove(subcondition);
                _craftingCondition.CraftingSubConditions.Remove(subcondition.SubCondition);
                SelectedSubcondition = null;

                if (SubconditionControls.Count == 0)
                {
                    AddSubcondition();
                }

                SelectedSubcondition = SubconditionControls[0];

                OnPropertyChanged(nameof(SelectedSubcondition));
                OnPropertyChanged(nameof(SubconditionControls));
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            AddSubcondition();
        }

        public void Save()
        {
            _craftingCondition.CraftingSubConditions.Clear();
            foreach (var subcondition in SubconditionControls.Select(x => x.Save()))
            {
                _craftingCondition.CraftingSubConditions.Add(subcondition);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
