using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PoeCrafting.Domain.Currency;
using PoeCrafting.UI.Pages;

namespace PoeCrafting.UI
{
    /// <summary>
    /// Interaction logic for TestbedWindow.xaml
    /// </summary>
    public partial class TestbedWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly CraftingTestbedModel ViewModel;

        public ObservableCollection<ICurrency> Currencies => ToObservableCollection(ViewModel.CurrencyFactory.GetValidCurrency(ViewModel.Status));

        public ObservableCollection<string> ItemTypes => ToObservableCollection(ViewModel.ItemSubtypeNames);

        public ObservableCollection<string> ItemBases => ViewModel.BaseItemNames != null ? ToObservableCollection(ViewModel.BaseItemNames) : new ObservableCollection<string>();

        public ObservableCollection<string> Prefixes => ViewModel.Equipment != null ? ToObservableCollection(ViewModel.Equipment.Prefixes.Select(x => x.Affix.ModType)) : new ObservableCollection<string>();

        public ObservableCollection<string> Suffixes => ViewModel.Equipment != null ? ToObservableCollection(ViewModel.Equipment.Suffixes.Select(x => x.Affix.ModType)) : new ObservableCollection<string>();

        public ICurrency SelectedCurrency { get; set; }

        private string selectedType;
        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                if (ViewModel != null && !string.IsNullOrEmpty(value))
                {
                    ViewModel.SelectSubtype(value);
                }
                NotifyPropertyChanged("ItemBases");
            }
        }

        public string selectedBase;
        public string SelectedBase
        {
            get { return selectedBase; }
            set
            {
                selectedBase = value;
                if (ViewModel != null && !string.IsNullOrEmpty(value))
                {
                    ViewModel.SelectBase(value);
                    ViewModel.CreateItem();

                    NotifyPropertyChanged("Prefixes");
                    NotifyPropertyChanged("Suffixes");
                    NotifyPropertyChanged("MinPrefix");
                    NotifyPropertyChanged("MaxPrefix");
                    NotifyPropertyChanged("MinSuffix");
                    NotifyPropertyChanged("MaxSuffix");
                    NotifyPropertyChanged("MinAffix");
                    NotifyPropertyChanged("MaxAffix");
                    NotifyPropertyChanged("Rarity");
                    NotifyPropertyChanged("Currencies");
                }
            }
        }

        public string MinPrefix => ViewModel.Status.MinPrefixes.ToString();
        public string MaxPrefix => ViewModel.Status.MaxPrefixes.ToString();
        public string MinSuffix => ViewModel.Status.MinSuffixes.ToString();
        public string MaxSuffix => ViewModel.Status.MaxSuffixes.ToString();
        public string MinAffix => ViewModel.Status.MinAffixes.ToString();
        public string MaxAffix => ViewModel.Status.MaxAffixes.ToString();
        public string Rarity => ViewModel.Status.Rarity.ToString();

        public TestbedWindow(CraftingTestbedModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }

        private void OnCraftSelection(object sender, RoutedEventArgs e)
        {
            if (SelectedCurrency != null)
            {
                ViewModel.Craft(SelectedCurrency);
            }

            NotifyPropertyChanged("Prefixes");
            NotifyPropertyChanged("Suffixes");
            NotifyPropertyChanged("MinPrefix");
            NotifyPropertyChanged("MaxPrefix");
            NotifyPropertyChanged("MinSuffix");
            NotifyPropertyChanged("MaxSuffix");
            NotifyPropertyChanged("MinAffix");
            NotifyPropertyChanged("MaxAffix");
            NotifyPropertyChanged("Rarity");
            NotifyPropertyChanged("Currencies");
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> collection)
        {
            var obsCollection = new ObservableCollection<T>();
            foreach (var item in collection)
            {
                obsCollection.Add(item);
            }
            return obsCollection;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
