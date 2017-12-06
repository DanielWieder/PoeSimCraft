using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PoeCrafting.Entities;
using PoeCrafting.Infrastructure;

namespace WorkspacesModule.Condition
{
    public class SubconditionAffixesControlViewModel : ViewModelBase
    {
        private AffixType _affixType { get; set; }

        public string AffixTypeName => _affixType.ToString();

        private StatValueType _valueType;
        public StatValueType ValueType
        {
            get { return _valueType; }
            set
            {
                if (_valueType == value) return;
                _valueType = value;
                FirstAffixModel.StatValueType = value;
                SecondAffixModel.StatValueType = value;
                ThirdAffixModel.StatValueType = value;
            }
        }

        public List<ConditionAffix> Conditions => GetConditions();

        public SubconditionAffixControlView FirstAffix { get; set; }
        public SubconditionAffixControlView SecondAffix { get; set; }
        public SubconditionAffixControlView ThirdAffix { get; set; }

        private SubconditionAffixControlViewModel FirstAffixModel { get; set; }
        private SubconditionAffixControlViewModel SecondAffixModel { get; set; }
        private SubconditionAffixControlViewModel ThirdAffixModel { get; set; }

        public SubconditionAffixesControlViewModel(List<ConditionAffix> conditions, List<Affix> affixes, AffixType affixType, PoeCrafting.Entities.ItemBase itemBase, StatValueType valueType)
        {
            _affixType = affixType;
            Initialize(conditions, affixes, affixType, itemBase, valueType);
            _valueType = valueType;

            OnPropertyChanged(nameof(FirstAffix));
            OnPropertyChanged(nameof(SecondAffix));
            OnPropertyChanged(nameof(ThirdAffix));
        }

        private void Initialize(List<ConditionAffix> conditions, List<Affix> affixes, AffixType affixType, PoeCrafting.Entities.ItemBase itemBase, StatValueType valueType)
        {
            var firstCondition = conditions.Count >= 1 ? conditions[0] : new ConditionAffix();
            var secondCondition = conditions.Count >= 2 ? conditions[1] : new ConditionAffix();
            var thirdCondition = conditions.Count >= 3 ? conditions[2] : new ConditionAffix();

            FirstAffixModel = new SubconditionAffixControlViewModel(firstCondition, affixes, valueType, affixType, itemBase);
            SecondAffixModel = new SubconditionAffixControlViewModel(secondCondition, affixes, valueType, affixType, itemBase);
            ThirdAffixModel = new SubconditionAffixControlViewModel(thirdCondition, affixes, valueType, affixType, itemBase);

            FirstAffix = new SubconditionAffixControlView(FirstAffixModel);
            SecondAffix = new SubconditionAffixControlView(SecondAffixModel);
            ThirdAffix = new SubconditionAffixControlView(ThirdAffixModel);
        }

        private List<ConditionAffix> GetConditions()
        {
            var list = new List<ConditionAffix>();

            var firstCondition = FirstAffixModel.GetCondition();
            if (firstCondition != null)
            {
                list.Add(firstCondition);
            }

            var secondCondition = SecondAffixModel.GetCondition();
            if (secondCondition != null)
            {
                list.Add(secondCondition);
            }

            var thirdCondition = ThirdAffixModel.GetCondition();
            if (thirdCondition != null)
            {
                list.Add(thirdCondition);
            }

            return list;
        }
    }
}
