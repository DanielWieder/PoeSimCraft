using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
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

        public string CurrencyResults { get; set; }

        public CraftingResultsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Initialize(Dictionary<ItemPrototypeModel, List<Equipment>> items, double currencySpent)
        {
            ItemResults = string.Empty;
            OnPropertyChanged(nameof(ItemResults));

            ItemDictionary = items;
            ItemPrototypes.Clear();

            var itemCount = items.Where(x => x.Key.Value > 0).Sum(x => x.Value.Count);
            var itemsValue = items.Sum(x => x.Key.Value * x.Value.Count);

            CurrencyResults = String.Format("You spent {0} chaos crafting {1} items worth a total of {2} chaos", currencySpent, itemCount, itemsValue);

            foreach (var itemPrototype in ItemDictionary)
            {
                ItemPrototypes.Add(itemPrototype.Key);
            }

            OnPropertyChanged(nameof(CurrencyResults));
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

            builder.Append("Condition: " + SelectedItem.ItemName);
            builder.Append(Environment.NewLine);
            builder.Append("Value Per Item (C): " + SelectedItem.Value);
            builder.Append(Environment.NewLine);
            builder.Append("Value (C): " + SelectedItem.Value * list.Count);
            builder.Append(Environment.NewLine);
            builder.Append("Count: " + list.Count);

            if (list.Count > 50)
            {
                builder.Append(Environment.NewLine);
                builder.Append("Showing the first 50 items");
            }
            AffixValueCalculator calculator = new AffixValueCalculator();
            foreach (var item in list.Take(50))
            {
                builder.Append(Environment.NewLine);
                builder.Append("---------------------");
                builder.Append(Environment.NewLine);

                builder.Append("Rarity: " + item.Rarity);
                builder.Append(Environment.NewLine);


                if (item.ItemBase.Properties.ContainsKey(ItemProperties.EnergyShield) && item.ItemBase.Properties[ItemProperties.EnergyShield] > 0)
                {
                    var totalEs = calculator.GetAffixValues(AffixNames.TotalEnergyShield, item, AffixType.Meta, StatValueType.Flat);
                    builder.Append("Total ES: " + totalEs[0]);
                    builder.Append(Environment.NewLine);
                }

                if (item.ItemBase.Properties.ContainsKey(ItemProperties.Armour) && item.ItemBase.Properties[ItemProperties.Armour] > 0)
                {
                    var totalArmour = calculator.GetAffixValues(AffixNames.TotalArmor, item, AffixType.Meta, StatValueType.Flat);
                    builder.Append("Total Armour: " + totalArmour[0]);
                    builder.Append(Environment.NewLine);
                }

                if (item.ItemBase.Properties.ContainsKey(ItemProperties.Evasion) && item.ItemBase.Properties[ItemProperties.Evasion] > 0)
                {
                    var totalEvasion = calculator.GetAffixValues(AffixNames.TotalEvasion, item, AffixType.Meta, StatValueType.Flat);
                    builder.Append("Total Evasion: " + totalEvasion[0]);
                    builder.Append(Environment.NewLine);
                }

                if (item.ItemBase.Properties.ContainsKey(ItemProperties.DPS))
                {
                    var totalEvasion = calculator.GetAffixValues(AffixNames.TotalDps, item, AffixType.Meta, StatValueType.Flat);
                    builder.Append("Total Damage: " + totalEvasion[0]);
                    builder.Append(Environment.NewLine);
                }

                builder.Append("Prefixes");
                builder.Append(Environment.NewLine);
                foreach (var prefix in item.Prefixes)
                {
                    WriteAffix(prefix, builder);
                }
                builder.Append(Environment.NewLine);

                builder.Append("Suffixes");
                builder.Append(Environment.NewLine);
                foreach (var suffix in item.Suffixes)
                {
                    WriteAffix(suffix, builder);
                }

                builder.Append(Environment.NewLine);
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

        private static void WriteAffix(Stat prefix, StringBuilder builder)
        {
            if (prefix.Affix.StatName3 != null)
            {
                builder.AppendLine($"\t {prefix.Affix.ModType} : {prefix.Value1}, {prefix.Value2}, {prefix.Value3}");
            }
            else if (prefix.Affix.StatName2 != null)
            {
                builder.AppendLine($"\t {prefix.Affix.ModType} : {prefix.Value1}, {prefix.Value2}");
            }
            else
            {
                builder.AppendLine("\t" + prefix.Affix.ModType + ": " + prefix.Value1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
