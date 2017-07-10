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
    public partial class SubconditionSelectionControl : UserControl, INotifyPropertyChanged
    {
        private readonly List<Affix> _affixes;
        private AffixType _affixType { get; set; }

        public string FirstAffix { get; set; }
        public int? FirstAffixMin { get; set; }
        public int? FirstAffixMax { get; set; }

        public string SecondAffix { get; set; }
        public int? SecondAffixMin { get; set; }
        public int? SecondAffixMax { get; set; }

        public string ThirdAffix { get; set; }
        public int? ThirdAffixMin { get; set; }
        public int? ThirdAffixMax { get; set; }

        public string AffixTypeName => _affixType.ToString();

        private readonly List<ConditionAffix> _initialConditions;

        public SubconditionValueType ValueType { get; set; }

        public List<string> ValidAffxes => GetValidAffixes();
        public List<ConditionAffix> Conditions => GetConditions();

        private List<ConditionAffix> GetConditions()
        {
            var list = new List<ConditionAffix>();
            if (!string.IsNullOrEmpty(FirstAffix))
            {
                list.Add(new ConditionAffix
                {
                    Group = FirstAffix,
                    Min = FirstAffixMin,
                    Max = FirstAffixMax
                });
            }
            if (!string.IsNullOrEmpty(SecondAffix))
            {
                list.Add(new ConditionAffix
                {
                    Group = SecondAffix,
                    Min = SecondAffixMin,
                    Max = SecondAffixMax
                });
            }
            if (!string.IsNullOrEmpty(ThirdAffix))
            {
                list.Add(new ConditionAffix
                {
                    Group = ThirdAffix,
                    Min = ThirdAffixMin,
                    Max = ThirdAffixMax
                });
            }
            return list;
        }

        public SubconditionSelectionControl(List<ConditionAffix> conditions, List<Affix> affixes, AffixType affixType)
        {
            _affixType = affixType;
            _affixes = affixes;
            _initialConditions = conditions;
            OnPropertyChanged(nameof(ValidAffxes));
            Dispatcher.Invoke(Initialize, DispatcherPriority.SystemIdle);
            Initialize();
            DataContext = this;
            InitializeComponent();
        }

        private void Initialize()
        {

            if (_initialConditions != null)
            {
                if (_initialConditions.Count >= 1)
                {
                    FirstAffix = _initialConditions[0].Group;
                    FirstAffixMin = _initialConditions[0].Min;
                    FirstAffixMax = _initialConditions[0].Max;

                    OnPropertyChanged(nameof(FirstAffix));
                    OnPropertyChanged(nameof(FirstAffixMin));
                    OnPropertyChanged(nameof(FirstAffixMax));
                }
                if (_initialConditions.Count >= 2)
                {
                    SecondAffix = _initialConditions[1].Group;
                    SecondAffixMin = _initialConditions[1].Min;
                    SecondAffixMax = _initialConditions[1].Max;

                    OnPropertyChanged(nameof(SecondAffix));
                    OnPropertyChanged(nameof(SecondAffixMin));
                    OnPropertyChanged(nameof(SecondAffixMax));
                }
                if (_initialConditions.Count >= 3)
                {
                    ThirdAffix = _initialConditions[2].Group;
                    ThirdAffixMin = _initialConditions[2].Min;
                    ThirdAffixMax = _initialConditions[2].Max;

                    OnPropertyChanged(nameof(ThirdAffix));
                    OnPropertyChanged(nameof(ThirdAffixMin));
                    OnPropertyChanged(nameof(ThirdAffixMax));
                }
            }
        }

        public List<string> GetValidAffixes()
        {
     //       var unique = _affixes.Where(x => (FirstAffix == null || x.Group != FirstAffix) &&
   //                                             (SecondAffix == null || x.Group != SecondAffix) &&
     //                                           (ThirdAffix == null || x.Group != ThirdAffix)).ToList();

            var matching = _affixes.Where(x => x.Type == _affixType.ToString().ToLower()).ToList();

            var group = matching.Select(x => x.Group).ToList();

            var distinct = group.Distinct().ToList();

            distinct.Insert(0, string.Empty);
            return distinct.ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
