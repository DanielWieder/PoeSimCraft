using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    public class OnDeleteEventArgs
    {
        public OnDeleteEventArgs(SubconditionControlViewModel c) { Control = c; }
        public SubconditionControlViewModel Control { get; private set; } // readonly
    }

    public class SubconditionControlViewModel: ViewModelBase
    {
        public SubconditionAffixesControlView PrefixConditions { get; set; }
        public SubconditionAffixesControlView SuffixConditions { get; set; }
        public SubconditionAffixesControlView MetaConditions { get; set; }

        private SubconditionAffixesControlViewModel PrefixConditionsModel { get; set; }
        private SubconditionAffixesControlViewModel SuffixConditionsModel { get; set; }
        private SubconditionAffixesControlViewModel MetaConditionsModel { get; set; }

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
                OnPropertyChanged(nameof(SubconditionName));
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

        public List<string> ValueTypes => Enum.GetNames(typeof(StatValueType)).ToList();

        private string _selectedValueType;

        public string SelectedValueType
        {
            get { return _selectedValueType; }
            set
            {
                _selectedValueType = value;
                var type = (StatValueType)Enum.Parse(typeof(StatValueType), value);

                PrefixConditionsModel.ValueType = type;
                SuffixConditionsModel.ValueType = type;
                MetaConditionsModel.ValueType = type;

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

        public SubconditionControlViewModel(CraftingSubcondition subCondition, List<Affix> affixes, int index, PoeCrafting.Entities.ItemBase itemBase)
        {
            SubconditionName = string.IsNullOrEmpty(subCondition.Name) ? "JsonSubcondition " + index : subCondition.Name;

            Index = index;
            SubCondition = subCondition;
            AggregateTypeMin = subCondition.AggregateMin;
            AggregateTypeMax = subCondition.AggregateMax;

            PrefixConditionsModel = new SubconditionAffixesControlViewModel(subCondition.PrefixConditions, affixes, AffixType.Prefix, itemBase, subCondition.ValueType);
            SuffixConditionsModel = new SubconditionAffixesControlViewModel(subCondition.SuffixConditions, affixes, AffixType.Suffix, itemBase, subCondition.ValueType);
            MetaConditionsModel = new SubconditionAffixesControlViewModel(subCondition.MetaConditions, affixes, AffixType.Meta, itemBase, subCondition.ValueType);

            PrefixConditions = new SubconditionAffixesControlView(PrefixConditionsModel);
            SuffixConditions = new SubconditionAffixesControlView(SuffixConditionsModel);
            MetaConditions = new SubconditionAffixesControlView(MetaConditionsModel);

            SelectedAggregateType = subCondition.AggregateType.ToString();
            SelectedValueType = subCondition.ValueType.ToString();
        }

        public CraftingSubcondition Save()
        {
            SubCondition.Name = _subconditionName;
            SubCondition.ValueType =
                (StatValueType)Enum.Parse(typeof(StatValueType), SelectedValueType);
            SubCondition.AggregateType =
                (SubconditionAggregateType)Enum.Parse(typeof(SubconditionAggregateType), SelectedAggregateType);
            SubCondition.AggregateMin = AggregateTypeMin;
            SubCondition.AggregateMax = AggregateTypeMax;
            SubCondition.PrefixConditions = PrefixConditionsModel.Conditions;
            SubCondition.SuffixConditions = SuffixConditionsModel.Conditions;
            SubCondition.MetaConditions = MetaConditionsModel.Conditions;
            return SubCondition;
        }

        public void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            OnDeleteEvent?.Invoke(this, new OnDeleteEventArgs(this));
        }
    }
}
