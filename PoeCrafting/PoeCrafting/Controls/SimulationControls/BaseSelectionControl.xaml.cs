﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
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
        public int Category { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var baseInfo = (BaseInfomation)obj;
            return string.Equals(SelectedBase, baseInfo.SelectedBase) && string.Equals(SelectedSubtype, baseInfo.SelectedSubtype) && ItemLevel == baseInfo.ItemLevel && ItemCost == baseInfo.ItemCost && League == baseInfo.League && Category == baseInfo.Category;
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
                hashCode = (hashCode * 397) ^ Category.GetHashCode();
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
            "Abyss",
            "HC Abyss",
            "Standard",
            "Hardcore"
        };

        public List<string> Leagues => _leagues;

        private string _selectedSubtype;
        private string _selectedBase;
        private string _selectedLeague;
        private Action _updateIsReady;
        private EquipmentFetch _fetch;
        private readonly EquipmentFactory _factory;
        private int _itemLevel;

        public List<string> Subtypes { get; }
        public List<string> Bases { get; set; }
        public List<string> Categories { get; } = new List<string>
        {
            "Normal",
            "Shaper",
            "Elder"
        };

        public string AffixDescriptions { get; set; }

        public bool HasSubtype => !string.IsNullOrEmpty(_selectedSubtype);

        public string SelectedSubtype
        {
            get { return _selectedSubtype; }
            set
            {
                _selectedSubtype = value;
                Bases = _fetch.FetchBasesBySubtype(_selectedSubtype).OrderBy(x => x).ToList();
                UpdateAffixDescriptions();
                OnPropertyChanged(nameof(Bases));
                OnPropertyChanged(nameof(HasSubtype));
            }
        }

        public string SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                UpdateAffixDescriptions();
            }
        }

        private static string _selectedCategory;


        public string SelectedBase
        {
            get { return _selectedBase; }
            set
            {
                _selectedBase = value;
                UpdateAffixDescriptions();
                _updateIsReady();
            }
        }

        private void UpdateAffixDescriptions()
        {
            if (SelectedBase == null)
            {
                AffixDescriptions = string.Empty;
                OnPropertyChanged(nameof(AffixDescriptions));
                return;
            }

            StringBuilder builder = new StringBuilder();

            _factory.Initialize(SelectedBase, Categories.IndexOf(_selectedCategory), ItemLevel);

            var affixes = _factory.GetPossibleAffixes();
            var types = affixes.GroupBy(x => x.Type);
            var totalWeight = affixes.Sum(x => x.Weight);

            foreach (var type in types)
            {
                if (type.Key == "Meta")
                {
                    continue;
                }
                builder.AppendLine(type.Key);

                var groups = type.GroupBy(x => x.Group);

                foreach (var group in groups)
                {
                    var items = group.ToList();

                    builder.AppendLine("\t" + group.Key);

                    if (items.Any(x => x.StatName2 != null && !x.StatName2.Contains("Dummy")))
                    {
                        builder.AppendLine("\tStat 1: " + items[0].StatName1);
                        builder.AppendLine("\tStat 2: " + items[0].StatName2);
                    }
                    if (items.Any(x => x.StatName3 != null && !x.StatName3.Contains("Dummy")))
                    {
                        builder.AppendLine("\tStat 3: " + items[0].StatName3);
                    }


                    foreach (var item in items)
                    {
                        var chance = item.Weight / (float)totalWeight * 100;
                        if (item.StatName2 != null && !item.StatName2.Contains("Dummy"))
                        {
                            builder.AppendLine("\t\tilvl: " + item.ILvl + "\tT" + item.Tier + "\t" + chance.ToString("0.###") + "%\t");

                            builder.AppendLine("\t\t\t\t\t" + item.StatMin1 + "-" + item.StatMax1);
                            builder.AppendLine("\t\t\t\t\t" + item.StatMin2 + "-" + item.StatMax2);

                            if (item.StatName3 != null && !item.StatName3.Contains("Dummy"))
                            {
                                builder.AppendLine("\t\t\t\t" + "ilvl: " + item.ILvl + "\t" + item.StatMin3 + " - " + item.StatMax3);
                            }
                        }
                        else
                        {
                            builder.AppendLine("\t\tilvl: " + item.ILvl + "\tT" + item.Tier + "\t" + chance.ToString("0.###") + "%\t" + item.StatMin1 + "-" + item.StatMax1);
                        }
                    }

                }
            }

            AffixDescriptions = builder.ToString();
            OnPropertyChanged(nameof(AffixDescriptions));
        }

        public string SelectedLeague
        {
            get { return _selectedLeague; }
            set
            {
                _selectedLeague = value;
            }
        }

        public int ItemLevel {
            get { return _itemLevel; }
            set
            {
                _itemLevel = value;
                UpdateAffixDescriptions();
            }
        }

        public int ItemCost { get; set; } = 0;

        public BaseInfomation BaseInformation => new BaseInfomation
        {
            ItemLevel = ItemLevel,
            SelectedBase = SelectedBase,
            SelectedSubtype = SelectedSubtype,
            ItemCost = ItemCost,
            League = SelectedLeague,
            Category = Categories.IndexOf(_selectedCategory)
        };

        public bool CanComplete()
        {
            return !string.IsNullOrEmpty(SelectedSubtype) &&
                   !string.IsNullOrEmpty(SelectedBase);
        }

        public void OnClose()
        { }

        public BaseSelectionControl(EquipmentFetch fetch, EquipmentFactory factory)
        {
            _itemLevel = 84;
            _fetch = fetch;
            _factory = factory;
            Subtypes = fetch.FetchSubtypes().OrderBy(x => x).ToList();
            SelectedLeague = Leagues[0];
            SelectedCategory = Categories[0];
            OnPropertyChanged(nameof(SelectedLeague));
            OnPropertyChanged(nameof(SelectedCategory));
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
