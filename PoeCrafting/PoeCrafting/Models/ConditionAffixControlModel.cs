using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;

namespace PoeCrafting.UI.Models
{
    public class ConditionAffixControlModel
    {
        // The drop downs will show valid, unselected affixes
        // Invalid Min/SelectedMax values will be outlined in red and changed to their closest valid values
        // If the SubconditionValueType is changed then all min/max values will automatically be transformed into the associated new value (If going from Tier -> Flat the MinValue will be used)
        // If the ItemLevel is changed then invalid values will be outlined in red

        private readonly List<Affix> _affixes;
        private readonly ItemBase _itemBase;
        private readonly AffixType _affixType;

        public SubconditionValueType ValueType { get; set; }

        public ConditionAffix FirstAffix { get; set; }
        public ConditionAffix SecondAffix { get; set; }
        public ConditionAffix ThirdAffix { get; set; }


        public List<string> ValidAffixes
        {
            get
            {
                return _affixes.Where(x => (FirstAffix == null || x.Group != FirstAffix.Group) && 
                                             (SecondAffix == null || x.Group != SecondAffix.Group) && 
                                             (ThirdAffix == null || x.Group != ThirdAffix.Group))
                                             .Select(x => x.Group)
                                             .ToList();
            }
        }
        public bool IsFirstAffixMinValid => IsRangeValid(FirstAffix.Group, FirstAffix.Min) && IsMinLessThanMax(FirstAffix.Min, FirstAffix.Max);
        public bool IsFirstAffixMaxValid => IsRangeValid(FirstAffix.Group, FirstAffix.Max) && IsMinLessThanMax(FirstAffix.Min, FirstAffix.Max);
        public bool IsSecondAffixMinValid => IsRangeValid(SecondAffix.Group, SecondAffix.Min) && IsMinLessThanMax(SecondAffix.Min, SecondAffix.Max);
        public bool IsSecondAffixMaxValid => IsRangeValid(SecondAffix.Group, SecondAffix.Max) && IsMinLessThanMax(SecondAffix.Min, SecondAffix.Max);
        public bool IsThirdAffixMinValid => IsRangeValid(ThirdAffix.Group, ThirdAffix.Min) && IsMinLessThanMax(ThirdAffix.Min, ThirdAffix.Max);
        public bool IsThirdAffixMaxValid => IsRangeValid(ThirdAffix.Group, ThirdAffix.Max) && IsMinLessThanMax(ThirdAffix.Min, ThirdAffix.Max);

        public ConditionAffixControlModel(ItemBase itemBase, SubconditionValueType subconditionValueType, List<Affix> affixes, AffixType affixType)
        {
            _itemBase = itemBase;

            ValueType = subconditionValueType;
            _affixes = affixes;
            _affixType = affixType;
        }

        public void SetAffix(string group, ConditionAffix affix)
        {
            if (string.IsNullOrEmpty(group))
            {
                affix.Group = string.Empty;
                affix.Min = null;
                affix.Max = null;
                return;
            }

            affix.Group = group;
            affix.Min = 0;
            affix.Max = ConditionValueCalculator.GetGroupMax(group, _itemBase, _affixes, _affixType);
        }

        private bool IsMinLessThanMax(int? min, int? max)
        {
            if (!min.HasValue || !max.HasValue)
            {
                return true;
            }

            return min.Value <= max.Value;
        }

        private bool IsRangeValid(string group, int? value)
        {
            if (!value.HasValue)
            {
                return true;
            }

            var minOption = 0;
            var maxOption = ConditionValueCalculator.GetGroupMax(group, _itemBase, _affixes, _affixType);

            return minOption <= value.Value && maxOption >= value.Value;
        }
    }
}
