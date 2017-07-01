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

        public List<string> ValidAffxes => Model.ValidAffixes;



        public List<ConditionAffix> Conditions
        {
            get
            {
                var list = new List<ConditionAffix>();
                if (Model.FirstAffix != null)
                {
                    list.Add(Model.FirstAffix);
                }
                if (Model.SecondAffix != null)
                {
                    list.Add(Model.SecondAffix);
                }
                if (Model.ThirdAffix != null)
                {
                    list.Add(Model.ThirdAffix);
                }
                return list;
            }
        }

        public SubconditionSelectionControl(ItemBase itemBase, List<Affix> affixes, AffixType affixType)
        {
            InitializeComponent();
            Model = new ConditionAffixControlModel(itemBase, SubconditionValueType.Flat, affixes, affixType);
        }
    }
}
