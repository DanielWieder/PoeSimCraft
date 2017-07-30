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
using System.Windows.Threading;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for ConditionAffixControl.xaml
    /// </summary>
    public partial class SubconditionAffixesControl : UserControl, INotifyPropertyChanged
    {
        private AffixType _affixType { get; set; }

        public string AffixTypeName => _affixType.ToString();

        private StatValueType _valueType;
        public StatValueType ValueType {
            get { return _valueType; }
            set
            {
                if (_valueType == value) return;
                _valueType = value;
                FirstAffix.StatValueType = value;
                SecondAffix.StatValueType = value;
                ThirdAffix.StatValueType = value;
            } }

        public List<ConditionAffix> Conditions => GetConditions();

        public SubconditionAffixControl FirstAffix { get; set; }
        public SubconditionAffixControl SecondAffix { get; set; }
        public SubconditionAffixControl ThirdAffix { get; set; }

        public SubconditionAffixesControl(List<ConditionAffix> conditions, List<Affix> affixes, AffixType affixType, ItemBase itemBase, StatValueType valueType)
        {
            _affixType = affixType;
            Dispatcher.Invoke(() => Initialize(conditions, affixes, affixType, itemBase, valueType), DispatcherPriority.SystemIdle);
            _valueType = valueType;
            DataContext = this;
            InitializeComponent();

            OnPropertyChanged(nameof(FirstAffix));
            OnPropertyChanged(nameof(SecondAffix));
            OnPropertyChanged(nameof(ThirdAffix));
        }

        private void Initialize(List<ConditionAffix> conditions, List<Affix> affixes, AffixType affixType, ItemBase itemBase, StatValueType valueType)
        {
            var firstCondition = conditions.Count >= 1 ? conditions[0] : new ConditionAffix();
            var secondCondition = conditions.Count >= 2 ? conditions[1] : new ConditionAffix();
            var thirdCondition = conditions.Count >= 3 ? conditions[2] : new ConditionAffix();

            FirstAffix = new SubconditionAffixControl(firstCondition, affixes, valueType, affixType, itemBase);
            SecondAffix = new SubconditionAffixControl(secondCondition, affixes, valueType, affixType, itemBase);
            ThirdAffix = new SubconditionAffixControl(thirdCondition, affixes, valueType, affixType, itemBase);
        }

        private List<ConditionAffix> GetConditions()
        {
            var list = new List<ConditionAffix>();

            var firstCondition = FirstAffix.GetCondition();
            if (firstCondition != null)
            {
                list.Add(firstCondition);
            }

            var secondCondition = SecondAffix.GetCondition();
            if (secondCondition != null)
            {
                list.Add(secondCondition);
            }

            var thirdCondition = ThirdAffix.GetCondition();
            if (thirdCondition != null)
            {
                list.Add(thirdCondition);
            }

            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
