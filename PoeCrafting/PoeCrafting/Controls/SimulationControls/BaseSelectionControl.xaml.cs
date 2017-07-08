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
    /// <summary>
    /// Interaction logic for BaseSelectionControl.xaml
    /// </summary>
    public partial class BaseSelectionControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        private string _selectedSubtype;

        private EquipmentFetch _fetch;

        public List<string> Subtypes { get; }
        public List<string> Bases { get; set; }

        public string SelectedSubtype
        {
            get { return _selectedSubtype; }
            set
            {
                _selectedSubtype = value;
                Bases = _fetch.FetchBasesBySubtype(_selectedSubtype);
                OnPropertyChanged(nameof(Bases));
            }
        }

        public string SelectedBase { get; set; }

        public int ItemLevel { get; set; } = 84;

        public int Currency { get; set; } = 1000;

        public bool IsReady()
        {
            return !string.IsNullOrEmpty(SelectedSubtype) &&
                   !string.IsNullOrEmpty(SelectedBase);
        }

        public void Save()
        { }

        public BaseSelectionControl(EquipmentFetch fetch)
        {
            _fetch = fetch;
            Subtypes = fetch.FetchSubtypes();
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
