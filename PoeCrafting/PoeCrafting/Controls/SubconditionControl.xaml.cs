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
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;
using PoeCrafting.UI.Models;
using PoeCrafting.Domain.Crafting;

namespace PoeCrafting.UI.Controls
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class SubconditionControl : UserControl
    {
        public SubconditionSelectionControl PrefixConditions { get; set; }
        public SubconditionSelectionControl SuffixConditions { get; set; }
        public SubconditionSelectionControl MetaConditions { get; set; }

        public CraftingSubcondition SubCondition;

        public string SubconditionName { get; set; } = "Test";

        public SubconditionControl(CraftingSubcondition subCondition, ItemBase itemBase, List<Affix> affixes)
        {
            SubCondition = subCondition;
            PrefixConditions = new SubconditionSelectionControl(itemBase, affixes, AffixType.Prefix);
            SuffixConditions = new SubconditionSelectionControl(itemBase, affixes, AffixType.Suffix);
            MetaConditions = new SubconditionSelectionControl(itemBase, affixes, AffixType.Meta);
            DataContext = this;
            InitializeComponent();
        }

        public void Save()
        {
            SubCondition.PrefixConditions = PrefixConditions.Conditions;
            SubCondition.SuffixConditions = SuffixConditions.Conditions;
            SubCondition.MetaConditions = MetaConditions.Conditions;
        }
    }
}
