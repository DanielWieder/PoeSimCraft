using System;
using System.Collections.Generic;
using System.Linq;
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
using PoeCrafting.Entities;
using PoeCrafting.UI.Models;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for ConditionAffixControl.xaml
    /// </summary>
    public partial class SubconditionSelectionControl : UserControl
    {
        public readonly ConditionAffixControlModel Model;

        public SubconditionValueType ValueType
        {
            set { Model.ValueType = value; }
        }

        public string AffixType => Model.AffixType.ToString();

        public List<string> ValidAffxes => Model.ValidAffixes;

        public List<ConditionAffix> Conditions
        {
            get
            {
                var list = new List<ConditionAffix>();
                if (!string.IsNullOrEmpty(Model.FirstAffix))
                {
                    list.Add(new ConditionAffix
                    {
                        Group = Model.FirstAffix,
                        Min = Model.FirstAffixMin,
                        Max = Model.FirstAffixMax
                    });
                }
                if (!string.IsNullOrEmpty(Model.SecondAffix))
                {
                    list.Add(new ConditionAffix
                    {
                        Group = Model.SecondAffix,
                        Min = Model.SecondAffixMin,
                        Max = Model.SecondAffixMax
                    });
                }
                if (!string.IsNullOrEmpty(Model.ThirdAffix))
                {
                    list.Add(new ConditionAffix
                    {
                        Group = Model.ThirdAffix,
                        Min = Model.ThirdAffixMin,
                        Max = Model.ThirdAffixMax
                    });
                }
                return list;
            }
        }

        public SubconditionSelectionControl(ItemBase itemBase, List<Affix> affixes, AffixType affixType)
        {
            Model = new ConditionAffixControlModel(itemBase, SubconditionValueType.Flat, affixes, affixType);
            DataContext = this;
            InitializeComponent();
        }
    }
}
