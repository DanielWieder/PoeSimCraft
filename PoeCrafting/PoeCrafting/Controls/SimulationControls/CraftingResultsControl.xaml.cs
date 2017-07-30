using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;
using PoeCrafting.UI.Annotations;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for CraftingResultsControl.xaml
    /// </summary>
    public partial class CraftingResultsControl : UserControl, INotifyPropertyChanged, ISimulationControl
    {
        public Dictionary<ItemPrototypeModel, List<Equipment>> ItemDictionary = new Dictionary<ItemPrototypeModel, List<Equipment>>();

        public ObservableCollection<ItemPrototypeModel> ItemPrototypes { get; set; } = new ObservableCollection<ItemPrototypeModel>();

        public ItemPrototypeModel SelectedItem { get; set; }

        public string ItemResults { get; set; }

        public CraftingResultsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(Dictionary<ItemPrototypeModel, List<Equipment>> items)
        {
            ItemResults = string.Empty;
            OnPropertyChanged(nameof(ItemResults));

            ItemDictionary = items;
            ItemPrototypes.Clear();
            foreach (var itemPrototype in ItemDictionary.Keys)
            {
                ItemPrototypes.Add(itemPrototype);
            }

            OnPropertyChanged(nameof(ItemPrototypes));
        }

        public bool CanComplete()
        {
            return false;
        }

        public void OnClose()
        {
        }

        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }

            var list = ItemDictionary[SelectedItem];

            StringBuilder builder = new StringBuilder();

            if (list.Any())
            {
                builder.Append(Environment.NewLine);
                builder.Append("Base Item Name: " + list[0].ItemBase.Name);
                builder.Append(Environment.NewLine);
                builder.Append("Base Item Type: " + list[0].ItemBase.Type);
                builder.Append(Environment.NewLine);
                builder.Append("Base Item Subtype: " + list[0].ItemBase.Subtype);
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
            }

            builder.Append("Count: " + list.Count);

            if (list.Count > 50)
            {
                builder.Append(Environment.NewLine);
                builder.Append("Showing the first 50 items");
            }

            foreach (var item in list.Take(50))
            {
                builder.Append(Environment.NewLine);
                builder.Append("---------------------");
                builder.Append(Environment.NewLine);

                builder.Append("Rarity: " + item.Rarity);
                builder.Append(Environment.NewLine);


                if (item.ItemBase.Properties.ContainsKey("EnergyShield") && item.ItemBase.Properties["EnergyShield"] > 0)
                {
                    var totalEs = AffixValueCalculator.GetAffixValues("TotalEnergyShield", item, AffixType.Meta, SubconditionValueType.Flat);
                    var maxEs = AffixValueCalculator.GetAffixValues("TotalEnergyShield", item, AffixType.Meta, SubconditionValueType.Max);
                    builder.Append("Total ES: " + totalEs[0]);
                    builder.Append(Environment.NewLine);
                    builder.Append("Max ES: " + maxEs[0]);
                    builder.Append(Environment.NewLine);
                }

                if (item.ItemBase.Properties.ContainsKey("Armour") && item.ItemBase.Properties["Armour"] > 0)
                {
                    var totalArmour = AffixValueCalculator.GetAffixValues("TotalArmour", item, AffixType.Meta, SubconditionValueType.Flat);
                    var maxArmour = AffixValueCalculator.GetAffixValues("TotalArmour", item, AffixType.Meta, SubconditionValueType.Max);
                    builder.Append("Total Armour: " + totalArmour[0]);
                    builder.Append(Environment.NewLine);
                    builder.Append("Max Armour: " + maxArmour[0]);
                    builder.Append(Environment.NewLine);
                }

                if (item.ItemBase.Properties.ContainsKey("Evasion") && item.ItemBase.Properties["Evasion"] > 0)
                {
                    var totalEvasion = AffixValueCalculator.GetAffixValues("TotalEvasion", item, AffixType.Meta, SubconditionValueType.Flat);
                    var maxEvasion = AffixValueCalculator.GetAffixValues("TotalEvasion", item, AffixType.Meta, SubconditionValueType.Max);
                    builder.Append("Total Evasion: " + totalEvasion[0]);
                    builder.Append(Environment.NewLine);
                    builder.Append("Max Evasion: " + maxEvasion[0]);
                    builder.Append(Environment.NewLine);
                }

                builder.Append("Prefixes");
                builder.Append(Environment.NewLine);
                foreach (var prefix in item.Prefixes)
                {
                    builder.Append("\t");
                    builder.Append(AddSpaces(prefix.Affix.ModType) + ": " + prefix.Value1);
                    builder.Append(Environment.NewLine);
                }

                builder.Append("Suffixes");
                builder.Append(Environment.NewLine);
                foreach (var suffix in item.Suffixes)
                {
                    builder.Append("\t");
                    builder.Append(AddSpaces(suffix.Affix.ModType) + ": " + suffix.Value1);
                    builder.Append(Environment.NewLine);
                }

                if (item.Corrupted)
                {
                    builder.Append("\t");
                    builder.Append("Corrupted");
                    builder.Append(Environment.NewLine);
                }
            }

            ItemResults = builder.ToString();

            OnPropertyChanged(nameof(ItemResults));
        }

        private static string AddSpaces(string x)
        {
            return string.Concat(x.Select(y => Char.IsUpper(y) ? " " + y : y.ToString())).TrimStart(' ');
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
