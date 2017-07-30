using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for SubconditionAffixControl.xaml
    /// </summary>
    public partial class SubconditionAffixControl : UserControl, INotifyPropertyChanged
    {
        private readonly List<Affix> _affixes;

        private readonly AffixType _affixType;

        private readonly ItemBase _itemBase;

        private int _statOneMin;
        private int _statOneMax;
        private int _statTwoMin;
        private int _statTwoMax;
        private int _statThreeMin;
        private int _statThreeMax;

        public string FirstAffix { get; set; }
        public int FirstAffixMin { get; set; }
        public int FirstAffixMax { get; set; }

        public string SecondAffix { get; set; }
        public int SecondAffixMin { get; set; }
        public int SecondAffixMax { get; set; }

        public string ThirdAffix { get; set; }
        public int ThirdAffixMin { get; set; }
        public int ThirdAffixMax { get; set; }

        // There are no relevant third affixes. All of them have their min/max values as equal

        public bool HasFirstAffix => AffixName != null && !string.IsNullOrEmpty(FirstAffix) && _statOneMin != _statOneMax;
        public bool HasSecondAffix => AffixName != null && !IsTier && !string.IsNullOrEmpty(SecondAffix) && _statTwoMin != _statThreeMax;

        public bool HasOneAffix => (HasFirstAffix && !HasSecondAffix) || (!HasFirstAffix && HasSecondAffix);
        private bool HasMetaAffix => (AffixName != null && _affixType == AffixType.Meta);
        public bool IsTier => ValueType == SubconditionValueType.Tier;

        public Visibility NoUserSelection => BoolToVisibility(!HasFirstAffix && !HasSecondAffix);
        public Visibility OneUserSelection => BoolToVisibility(HasMetaAffix || HasOneAffix);
        public Visibility TwoUserSelection => BoolToVisibility(!HasMetaAffix && (HasFirstAffix && HasSecondAffix));
        public Visibility FirstAffixOneUserSelection => BoolToVisibility(HasMetaAffix || (HasOneAffix && HasFirstAffix));
        public Visibility SecondAffixOneUserSelection => BoolToVisibility(!HasMetaAffix && (HasOneAffix && HasSecondAffix));

        public List<string> ValidAffixes => GetValidAffixes();

        private SubconditionValueType _valueType { get; set; }
        public SubconditionValueType ValueType {
            get { return _valueType; }
            set
            {
                if (_valueType == value)
                {
                    return;
                }

                _valueType = value;
                OnPropertyChanged(nameof(ValueType));
                if (!string.IsNullOrEmpty(AffixName))
                {
                    UpdateModMinMax();
                }
            }
        }

        private string _affixName { get; set; }
        public string AffixName
        {
            get { return _affixName; }
            set
            {
                if (value == _affixName)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    _affixName = string.Empty;
                    FirstAffix = string.Empty;
                    SecondAffix = string.Empty;
                    ThirdAffix = string.Empty;

                    OnPropertyChanged(nameof(NoUserSelection));
                    OnPropertyChanged(nameof(OneUserSelection));
                    OnPropertyChanged(nameof(TwoUserSelection));
                    OnPropertyChanged(nameof(FirstAffixOneUserSelection));
                    OnPropertyChanged(nameof(SecondAffixOneUserSelection));
                    OnPropertyChanged(nameof(FirstAffix));
                    OnPropertyChanged(nameof(SecondAffix));
                    OnPropertyChanged(nameof(ThirdAffix));

                    return;
                }

                _affixName = value;

                OnPropertyChanged(nameof(Affix));

                var affix = _affixes.First(x => x.ModType == RemoveSpaces(AffixName));

                FirstAffix = CapitalizeUnderscoreDelimited(affix.StatName1);
                SecondAffix = CapitalizeUnderscoreDelimited(affix.StatName2);
                ThirdAffix = CapitalizeUnderscoreDelimited(affix.StatName3);

                OnPropertyChanged(nameof(FirstAffix));
                OnPropertyChanged(nameof(SecondAffix));
                OnPropertyChanged(nameof(ThirdAffix));

                UpdateModMinMax();
            }
        }

        public SubconditionAffixControl(ConditionAffix condition, List<Affix> affixes, SubconditionValueType valueType, AffixType affixType, ItemBase itemBase)
        {
            _valueType = valueType;
            _affixes = affixes;
            _affixType = affixType;
            _itemBase = itemBase;

            AffixName = condition.ModType;
            if (condition.Min1.HasValue)
            {
                FirstAffixMin = condition.Min1.Value;
            }
            if (condition.Max1.HasValue)
            {
                FirstAffixMax = condition.Max1.Value;
            }
            if (condition.Min2.HasValue)
            {
                SecondAffixMin = condition.Min2.Value;
            }
            if (condition.Max2.HasValue)
            {
                SecondAffixMax = condition.Max2.Value;
            }
            if (condition.Min3.HasValue)
            {
                ThirdAffixMin = condition.Min3.Value;
            }
            if (condition.Max3.HasValue)
            {
                ThirdAffixMax = condition.Max3.Value;
            }

            DataContext = this;
            InitializeComponent();
        }

        public ConditionAffix GetCondition()
        {
            if (string.IsNullOrEmpty(_affixName))
            {
                return null;
            }

            return new ConditionAffix
            {
                ModType = RemoveSpaces(_affixName),
                Min1 = FirstAffixMin,
                Max1 = FirstAffixMax,
                Min2 = SecondAffixMin,
                Max2 = SecondAffixMax,
                Min3 = ThirdAffixMin,
                Max3 = ThirdAffixMax
            };
        }

        private void UpdateModMinMax()
        {
            var modAffixes = _affixes.Where(x => x.ModType == RemoveSpaces(AffixName)).OrderBy(x => x.Tier);

            var lastTier = modAffixes.Last();
            var firstTier = modAffixes.First();

            if (ValueType == SubconditionValueType.Tier)
            {
                _statOneMin = firstTier.Tier;
                _statTwoMin = 0;
                _statThreeMin = 0;

                _statOneMax = lastTier.Tier;
                _statTwoMax = 0;
                _statThreeMax = 0;
            }
            else if (_affixType == AffixType.Meta)
            {
                var max = AffixValueCalculator.GetModMax(RemoveSpaces(AffixName), _itemBase, _affixes, AffixType.Meta);

                _statOneMin = 0;
                _statTwoMin = 0;
                _statThreeMin = 0;

                _statOneMax = max.FirstOrDefault();
                _statTwoMax = 0;
                _statThreeMax = 0;
            }
            else
            {
                _statOneMin = lastTier.StatMin1;
                _statTwoMin = lastTier.StatMin2;
                _statThreeMin = lastTier.StatMin3;

                _statOneMax = firstTier.StatMax1;
                _statTwoMax = firstTier.StatMax2;
                _statThreeMax = firstTier.StatMax3;
            }

            FirstAffixMin = _statOneMin;
            SecondAffixMin = _statTwoMin;
            ThirdAffixMin = _statThreeMin;

            FirstAffixMax = _statOneMax;
            SecondAffixMax = _statTwoMax;
            ThirdAffixMax = _statThreeMax;

            OnPropertyChanged(nameof(FirstAffixMin));
            OnPropertyChanged(nameof(SecondAffixMin));
            OnPropertyChanged(nameof(ThirdAffixMin));

            OnPropertyChanged(nameof(FirstAffixMax));
            OnPropertyChanged(nameof(SecondAffixMax));
            OnPropertyChanged(nameof(ThirdAffixMax));

            OnPropertyChanged(nameof(FirstAffixOneUserSelection));
            OnPropertyChanged(nameof(SecondAffixOneUserSelection));
            OnPropertyChanged(nameof(NoUserSelection));
            OnPropertyChanged(nameof(OneUserSelection));
            OnPropertyChanged(nameof(TwoUserSelection));
        }

        public List<string> GetValidAffixes()
        {
            //       var unique = _affixes.Where(x => (FirstAffix == null || x.ModType != FirstAffix) &&
            //                                           (SecondAffix == null || x.ModType != SecondAffix) &&
            //                                           (ThirdAffix == null || x.ModType != ThirdAffix)).ToList();

            var matching = _affixes.Where(x => x.Type == _affixType.ToString().ToLower()).ToList();

            var mods = matching.Select(x => x.ModType).ToList();

            var distinct =
                mods.Distinct()
                    .Select(AddSpaces)
                    .ToList();

            distinct.Insert(0, string.Empty);
            return distinct.ToList();
        }

        private static string CapitalizeUnderscoreDelimited(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string pattern = "_\\w";
            Regex regex = new Regex(pattern);
            MatchEvaluator evaluator = x => x.Value.ToUpper().Replace("_", " ");
            text = regex.Replace(text, evaluator);
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        private static string AddSpaces(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return string.Empty;
            }

            return string.Concat(x.Select(y => Char.IsUpper(y) ? " " + y : y.ToString())).TrimStart(' ');
        }

        private static string RemoveSpaces(string x)
        {
            return x.Replace(" ", "");
        }

        private Visibility BoolToVisibility(bool boolean)
        {
            return boolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public Action OnSelection;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
