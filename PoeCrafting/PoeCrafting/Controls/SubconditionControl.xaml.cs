using System;
using System.Collections.Generic;
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
using PoeCrafting.Entities;
using PoeCrafting.UI.Models;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI.Controls
{
    public class OnDeleteEventArgs
    {
        public OnDeleteEventArgs(SubconditionControl c) { Control = c; }
        public SubconditionControl Control { get; private set; } // readonly
    }

    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class SubconditionControl : UserControl, INotifyPropertyChanged
    {
        public SubconditionSelectionControl PrefixConditions { get; set; }
        public SubconditionSelectionControl SuffixConditions { get; set; }
        public SubconditionSelectionControl MetaConditions { get; set; }

        public CraftingSubcondition SubCondition;

        // Declare the delegate (if using non-generic pattern).
        public delegate void OnDeleteEventHandler(object sender, OnDeleteEventArgs e);

        // Declare the event.
        public event OnDeleteEventHandler OnDeleteEvent;

        private string _subconditionName;

        public int Index { get; set; }

        public string SubconditionName
        {
            get { return _subconditionName; }
            set
            {
                _subconditionName = value;
                OnPropertyChanged();
            }
        }


        private string _selectedAggregateType = "And";

        public string SelectedAggregateType
        {
            get { return _selectedAggregateType; }
            set
            {
                _selectedAggregateType = value;
                OnPropertyChanged(nameof(AggregateTypeHasMinMax));
            }
        }

        public List<string> AggregateTypes { get; } = Enum.GetNames(typeof(SubconditionAggregateType)).ToList();
        public int? AggregateTypeMin { get; set; }
        public int? AggregateTypeMax { get; set; }

        public bool AggregateTypeHasMinMax => SelectedAggregateType == "Sum" || SelectedAggregateType == "Count";

        public SubconditionControl(CraftingSubcondition subCondition, List<Affix> affixes, int index)
        {
            SubconditionName = string.IsNullOrEmpty(subCondition.Name) ? "Subcondition " + index : subCondition.Name;

            Index = index;
            SubCondition = subCondition;
            _selectedAggregateType =  subCondition.AggregateType.ToString();
            PrefixConditions = new SubconditionSelectionControl(subCondition.PrefixConditions, affixes, AffixType.Prefix);
            SuffixConditions = new SubconditionSelectionControl(subCondition.SuffixConditions, affixes, AffixType.Suffix);
            MetaConditions = new SubconditionSelectionControl(subCondition.MetaConditions, affixes, AffixType.Meta);
            DataContext = this;
            InitializeComponent();
        }

        public CraftingSubcondition Save()
        {

            SubCondition.Name = _subconditionName;
            SubCondition.ValueType = SubconditionValueType.Flat;
            SubCondition.AggregateType =
                (SubconditionAggregateType) Enum.Parse(typeof(SubconditionAggregateType), SelectedAggregateType);
            SubCondition.AggregateMin = AggregateTypeMin;
            SubCondition.AggregateMax = AggregateTypeMax;
            SubCondition.PrefixConditions = PrefixConditions.Conditions;
            SubCondition.SuffixConditions = SuffixConditions.Conditions;
            SubCondition.MetaConditions = MetaConditions.Conditions;
            return SubCondition;
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            OnDeleteEvent?.Invoke(this, new OnDeleteEventArgs(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
