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
        public SubconditionAffixesControl PrefixConditions { get; set; }
        public SubconditionAffixesControl SuffixConditions { get; set; }
        public SubconditionAffixesControl MetaConditions { get; set; }

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

        public Visibility MetaConditionsVisibility => SelectedValueType != "Tier" ? Visibility.Visible : Visibility.Collapsed;

        private string _selectedAggregateType;

        public string SelectedAggregateType
        {
            get { return _selectedAggregateType; }
            set
            {
                _selectedAggregateType = value;
                AggregateTypeDescription = GetAggregateTypeDescription();
                OnPropertyChanged(nameof(AggregateTypeHasMinMax));
                OnPropertyChanged(nameof(AggregateTypeDescription));
            }
        }

        public List<string> ValueTypes => Enum.GetNames(typeof(SubconditionValueType)).ToList();

        private string _selectedValueType;

        public string SelectedValueType
        {
            get { return _selectedValueType; }
            set
            {
                _selectedValueType = value;
                var type = (SubconditionValueType)Enum.Parse(typeof(SubconditionValueType), value);

                PrefixConditions.ValueType = type;
                SuffixConditions.ValueType = type;
                MetaConditions.ValueType = type;

                OnPropertyChanged(nameof(MetaConditionsVisibility));
            }
        }

        private string GetAggregateTypeDescription()
        {
            switch (_selectedAggregateType)
            {
                case "And":
                    return "All specified mods must exist and match their values.";
                case "Not":
                    return "None of specified mods must exist.";
                case "Count":
                    return "Specify the number of mods that should be matched.";
                case "Sum":
                    return "The mods' values must add to the specified total value.";
                case "If":
                    return "If a mod is present, its value must match the specified min/max.";
                default:
                    return string.Empty;
            }
        }

        public string AggregateTypeDescription { get; set; }

        public List<string> AggregateTypes { get; } = Enum.GetNames(typeof(SubconditionAggregateType)).ToList();
        public int? AggregateTypeMin { get; set; }
        public int? AggregateTypeMax { get; set; }

        public bool AggregateTypeHasMinMax => SelectedAggregateType == "Sum" || SelectedAggregateType == "Count";

        public SubconditionControl(CraftingSubcondition subCondition, List<Affix> affixes, int index, ItemBase itemBase)
        {
            SubconditionName = string.IsNullOrEmpty(subCondition.Name) ? "Subcondition " + index : subCondition.Name;

            Index = index;
            SubCondition = subCondition;
            AggregateTypeMin = subCondition.AggregateMin;
            AggregateTypeMax = subCondition.AggregateMax;
            PrefixConditions = new SubconditionAffixesControl(subCondition.PrefixConditions, affixes, AffixType.Prefix, itemBase, subCondition.ValueType);
            SuffixConditions = new SubconditionAffixesControl(subCondition.SuffixConditions, affixes, AffixType.Suffix, itemBase, subCondition.ValueType);
            MetaConditions = new SubconditionAffixesControl(subCondition.MetaConditions, affixes, AffixType.Meta, itemBase, subCondition.ValueType);
            SelectedAggregateType = subCondition.AggregateType.ToString();
            SelectedValueType = subCondition.ValueType.ToString();

            DataContext = this;
            InitializeComponent();
        }

        public CraftingSubcondition Save()
        {
            SubCondition.Name = _subconditionName;
            SubCondition.ValueType =
                (SubconditionValueType)Enum.Parse(typeof(SubconditionValueType), SelectedValueType);
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
