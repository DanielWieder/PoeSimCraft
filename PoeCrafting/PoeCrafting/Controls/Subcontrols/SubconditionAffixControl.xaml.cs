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

        private int? _statOneMin;
        private int? _statOneMax;
        private int? _statTwoMin;
        private int? _statTwoMax;
        private int? _statThreeMin;
        private int? _statThreeMax;

        private string _affixName;
        private StatValueType _statValueType;

        public string FirstStatName { get; set; }
        public int? FirstStatMin { get; set; }
        public int? FirstStatMax { get; set; }

        public string SecondStatName { get; set; }
        public int? SecondStatMin { get; set; }
        public int? SecondStatMax { get; set; }

        public string ThirdStatName { get; set; }
        public int? ThirdStatMin { get; set; }
        public int? ThirdStatMax { get; set; }

        // There are no relevant third stats. All of them have their min/max values as equal

        public bool HasFirstStat => AffixName != null && !string.IsNullOrEmpty(FirstStatName) && _statOneMin != _statOneMax;
        public bool HasSecondStat => AffixName != null && !IsTier && !string.IsNullOrEmpty(SecondStatName) && _statTwoMin != _statThreeMax;

        public bool HasOneStat => (HasFirstStat && !HasSecondStat) || (!HasFirstStat && HasSecondStat);
        private bool IsMetaAffix => AffixName != null && _affixType == AffixType.Meta;
        public bool IsTier => StatValueType == StatValueType.Tier;
        
        public Visibility DoubleStatSelectionVisibility => BoolToVisibility(!IsMetaAffix && (HasFirstStat && HasSecondStat));
        public Visibility FirstStatSelectionVisibility => BoolToVisibility(IsMetaAffix || (HasOneStat && HasFirstStat));
        public Visibility SecondStatSelectionVisibility => BoolToVisibility(!IsMetaAffix && (HasOneStat && HasSecondStat));

        public List<string> ValidAffixes => GetValidAffixes();

        public StatValueType StatValueType {
            get { return _statValueType; }
            set
            {
                if (_statValueType == value)
                {
                    return;
                }

                _statValueType = value;
                OnPropertyChanged(nameof(StatValueType));

                if (_affixType == AffixType.Meta && _statValueType == StatValueType.Tier)
                {
                    ClearStats();
                    return;
                }

                if (!string.IsNullOrEmpty(AffixName))
                {
                    UpdateModMinMax();
                }
            }
        }

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
                    ClearStats();
                    return;
                }

                _affixName = value;

                OnPropertyChanged(nameof(Affix));

                var affix = _affixes.First(x => x.ModType == RemoveSpaces(AffixName));

                FirstStatName = CapitalizeUnderscoreDelimited(affix.StatName1);
                SecondStatName = CapitalizeUnderscoreDelimited(affix.StatName2);
                ThirdStatName = CapitalizeUnderscoreDelimited(affix.StatName3);

                OnPropertyChanged(nameof(FirstStatName));
                OnPropertyChanged(nameof(SecondStatName));
                OnPropertyChanged(nameof(ThirdStatName));

                UpdateModMinMax();
            }
        }

        private void ClearStats()
        {
            _affixName = string.Empty;
            FirstStatName = string.Empty;
            SecondStatName = string.Empty;
            ThirdStatName = string.Empty;

            _statOneMin = null;
            _statOneMax = null;
            _statTwoMin = null;
            _statTwoMax = null;
            _statThreeMin = null;
            _statThreeMax = null;

            FirstStatMin = _statOneMin;
            SecondStatMin = _statTwoMin;
            ThirdStatMin = _statThreeMin;
            FirstStatMax = _statOneMax;
            SecondStatMax = _statTwoMax;
            ThirdStatMax = _statThreeMax;

            OnPropertyChanged(nameof(DoubleStatSelectionVisibility));
            OnPropertyChanged(nameof(FirstStatSelectionVisibility));
            OnPropertyChanged(nameof(SecondStatSelectionVisibility));
            OnPropertyChanged(nameof(FirstStatName));
            OnPropertyChanged(nameof(SecondStatName));
            OnPropertyChanged(nameof(ThirdStatName));
            OnPropertyChanged(nameof(FirstStatMin));
            OnPropertyChanged(nameof(SecondStatMin));
            OnPropertyChanged(nameof(SecondStatMin));
            OnPropertyChanged(nameof(SecondStatMax));
            OnPropertyChanged(nameof(ThirdStatMin));
            OnPropertyChanged(nameof(ThirdStatMax));
        }

        public SubconditionAffixControl(ConditionAffix condition, List<Affix> affixes, StatValueType statValueType, AffixType affixType, ItemBase itemBase)
        {
            _statValueType = statValueType;
            _affixes = affixes;
            _affixType = affixType;
            _itemBase = itemBase;

            AffixName = condition.ModType;

            if (condition.Min1.HasValue)
            {
                FirstStatMin = condition.Min1.Value;
            }
            if (condition.Max1.HasValue)
            {
                FirstStatMax = condition.Max1.Value;
            }
            if (condition.Min2.HasValue)
            {
                SecondStatMin = condition.Min2.Value;
            }
            if (condition.Max2.HasValue)
            {
                SecondStatMax = condition.Max2.Value;
            }
            if (condition.Min3.HasValue)
            {
                ThirdStatMin = condition.Min3.Value;
            }
            if (condition.Max3.HasValue)
            {
                ThirdStatMax = condition.Max3.Value;
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

            var condition = new ConditionAffix();

            condition.ModType = RemoveSpaces(_affixName);

            condition.Min1 = FirstStatMin;
            condition.Max1 = FirstStatMax;
            condition.Min2 = SecondStatMin;
            condition.Max2 = SecondStatMax;
            condition.Min3 = ThirdStatMin;
            condition.Max3 = ThirdStatMax;

            return condition;
        }

        private void UpdateModMinMax()
        {
            var modAffixes = _affixes.Where(x => x.ModType == RemoveSpaces(AffixName)).OrderBy(x => x.Tier);

            var lastTier = modAffixes.Last();
            var firstTier = modAffixes.First();

            if (StatValueType == StatValueType.Tier)
            {
                _statOneMin = firstTier.Tier;
                _statTwoMin = null;
                _statThreeMin = null;

                _statOneMax = lastTier.Tier;
                _statTwoMax = null;
                _statThreeMax = null;
            }
            else if (_affixType == AffixType.Meta)
            {
                var max = AffixValueCalculator.GetModMax(RemoveSpaces(AffixName), _itemBase, _affixes, AffixType.Meta);

                _statOneMin = 0;
                _statTwoMin = null;
                _statThreeMin = null;

                _statOneMax = max.FirstOrDefault();
                _statTwoMax = null;
                _statThreeMax = null;
            }
            else
            {
                if (!string.IsNullOrEmpty(lastTier.StatName1))
                {
                    _statOneMin = lastTier.StatMin1;
                }
                if (!string.IsNullOrEmpty(lastTier.StatName2))
                {
                    _statTwoMin = lastTier.StatMin2;
                }
                if (!string.IsNullOrEmpty(lastTier.StatName3))
                {
                    _statThreeMin = lastTier.StatMin3;
                }

                if (!string.IsNullOrEmpty(firstTier.StatName1))
                {
                    _statOneMax = firstTier.StatMax1;
                }
                if (!string.IsNullOrEmpty(firstTier.StatName2))
                {
                    _statTwoMax = firstTier.StatMax2;
                }
                if (!string.IsNullOrEmpty(firstTier.StatName3))
                {
                    _statThreeMax = firstTier.StatMax3;
                }
            }

            FirstStatMin = _statOneMin;
            SecondStatMin = _statTwoMin;
            ThirdStatMin = _statThreeMin;

            FirstStatMax = _statOneMax;
            SecondStatMax = _statTwoMax;
            ThirdStatMax = _statThreeMax;

            OnPropertyChanged(nameof(FirstStatMin));
            OnPropertyChanged(nameof(SecondStatMin));
            OnPropertyChanged(nameof(ThirdStatMin));

            OnPropertyChanged(nameof(FirstStatMax));
            OnPropertyChanged(nameof(SecondStatMax));
            OnPropertyChanged(nameof(ThirdStatMax));

            OnPropertyChanged(nameof(FirstStatSelectionVisibility));
            OnPropertyChanged(nameof(SecondStatSelectionVisibility));
            OnPropertyChanged(nameof(DoubleStatSelectionVisibility));
        }

        public List<string> GetValidAffixes()
        {
            //       var unique = _affixes.Where(x => (FirstStatName == null || x.ModType != FirstStatName) &&
            //                                           (SecondStatName == null || x.ModType != SecondStatName) &&
            //                                           (ThirdStatName == null || x.ModType != ThirdStatName)).ToList();

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
