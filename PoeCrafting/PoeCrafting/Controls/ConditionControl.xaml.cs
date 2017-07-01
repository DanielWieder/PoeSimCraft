using System;
using System.Collections.Generic;
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
        public List<SubconditionControl> SubconditionControls { get; set; }



        public SubconditionControl SelectedSubcondition
        {
            get { return _selectedSubcondition; }
            set
            {
                _selectedSubcondition = value;
                OnPropertyChanged(nameof(SelectedSubcondition));
            }
        }

        private SubconditionControl _selectedSubcondition;
        private ItemBase _itemBase;
        private List<Affix> _affixes;
        private CraftingCondition _craftingCondition;


        public ConditionControl(CraftingCondition condition, ItemBase itemBase, List<Affix> affixes )
        {
            _itemBase = itemBase;
            _affixes = affixes;
            _craftingCondition = condition;
            SubconditionControls = condition.CraftingSubConditions.Select(x => new SubconditionControl(x, itemBase, affixes)).ToList();

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
            var subconditionControl = new SubconditionControl(subcondition, _itemBase, _affixes);
            _craftingCondition.CraftingSubConditions.Add(subcondition);
            SubconditionControls.Add(subconditionControl);

            OnPropertyChanged(nameof(SubconditionControls));
        }

        public void RemoveSubcondition()
        {
            if (this.SelectedSubcondition != null)
            {
                SubconditionControls.Remove(SelectedSubcondition);
                _craftingCondition.CraftingSubConditions.Remove(SelectedSubcondition.SubCondition);
                SelectedSubcondition = null;

                if (SubconditionControls.Count == 0)
                {
                    AddSubcondition();
                }

                SelectedSubcondition = SubconditionControls[0];

                OnPropertyChanged(nameof(SelectedSubcondition));
                OnPropertyChanged(nameof(SubconditionControls));
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            RemoveSubcondition();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            AddSubcondition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
