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
using PoeCrafting.Domain;
using PoeCrafting.UI.Annotations;

namespace PoeCrafting.UI.Controls
{
    public class BaseInfomation
    {
        public string SelectedBase { get; set; }
        public string SelectedSubtype { get; set; }
        public int ItemLevel { get; set; }
        public int ItemCost { get; set; }
        public string League { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var baseInfo = (BaseInfomation)obj;
            return string.Equals(SelectedBase, baseInfo.SelectedBase) && string.Equals(SelectedSubtype, baseInfo.SelectedSubtype) && ItemLevel == baseInfo.ItemLevel && ItemCost == baseInfo.ItemCost && League == baseInfo.League;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SelectedBase?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (SelectedSubtype?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ ItemLevel;
                hashCode = (hashCode * 397) ^ ItemCost;
                hashCode = (hashCode * 397) ^ League.GetHashCode();
                return hashCode;
            }
        }
    }

    /// <summary>
    /// Interaction logic for BaseSelectionControl.xaml
    /// </summary>
    public partial class BaseSelectionControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        private readonly List<string> _leagues = new List<string>
        {
            "Harbinger",
            "HC Harbinger",
            "Standard",
            "Hardcore"
        };

        public List<string> Leagues => _leagues;

        private string _selectedSubtype;
        private string _selectedBase;
        private string _selectedLeague;
        private Action _updateIsReady;
        private EquipmentFetch _fetch;

        public List<string> Subtypes { get; }
        public List<string> Bases { get; set; }

        public bool HasSubtype => !string.IsNullOrEmpty(_selectedSubtype);

        public string SelectedSubtype
        {
            get { return _selectedSubtype; }
            set
            {
                _selectedSubtype = value;
                Bases = _fetch.FetchBasesBySubtype(_selectedSubtype).OrderBy(x => x).ToList();
                OnPropertyChanged(nameof(Bases));
                OnPropertyChanged(nameof(HasSubtype));
            }
        }

        public string SelectedBase
        {
            get { return _selectedBase; }
            set
            {
                _selectedBase = value;
                _updateIsReady();
            }
        }

        public string SelectedLeague
        {
            get { return _selectedLeague; }
            set
            {
                _selectedLeague = value;
            }
        }

        public int ItemLevel { get; set; } = 84;

        public int ItemCost { get; set; } = 0;

        public BaseInfomation BaseInformation => new BaseInfomation
        {
            ItemLevel = ItemLevel,
            SelectedBase = SelectedBase,
            SelectedSubtype = SelectedSubtype,
            ItemCost = ItemCost,
            League = SelectedLeague
        };

        public bool CanComplete()
        {
            return !string.IsNullOrEmpty(SelectedSubtype) &&
                   !string.IsNullOrEmpty(SelectedBase);
        }

        public void OnClose()
        { }

        public BaseSelectionControl(EquipmentFetch fetch)
        {
            _fetch = fetch;
            Subtypes = fetch.FetchSubtypes().OrderBy(x => x).ToList();
            SelectedLeague = Leagues[0];
            OnPropertyChanged(nameof(SelectedLeague));
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(Action updateIsReady)
        {
            _updateIsReady = updateIsReady;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
