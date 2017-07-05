using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Condition;
using PoeCrafting.Entities;

namespace PoeCrafting.UI.Models
{
    public class ConditionAffixControlModel
    {
        // The drop downs will show valid, unselected affixes
        // Invalid Min/SelectedMax values will be outlined in red and changed to their closest valid values
        // If the SubconditionValueType is changed then all min/max values will automatically be transformed into the associated new value (If going from Tier -> Flat the MinValue will be used)
        // If the ItemLevel is changed then invalid values will be outlined in red

        private readonly List<Affix> _affixes;
        private readonly ItemBase _itemBase;
        public readonly AffixType AffixType;

        public SubconditionValueType ValueType { get; set; }

        public string FirstAffix { get; set; }
        public int? FirstAffixMin { get; set; }
        public int? FirstAffixMax { get; set; }

        public string SecondAffix { get; set; }
        public int? SecondAffixMin { get; set; }
        public int? SecondAffixMax { get; set; }

        public string ThirdAffix { get; set; }
        public int? ThirdAffixMin { get; set; }
        public int? ThirdAffixMax { get; set; }

        public List<string> ValidAffixes
        {
            get
            {
                var affixes = _affixes.Where(x => (FirstAffix == null || x.Group != FirstAffix) &&
                                                  (SecondAffix == null || x.Group != SecondAffix) &&
                                                  (ThirdAffix == null || x.Group != ThirdAffix))
                    .Where(x => x.Type == AffixType.ToString().ToLower())
                    .Select(x => x.Group)
                    .Distinct()
                    .ToList();

                affixes.Insert(0, string.Empty);
                return affixes.ToList();
            }
        }

        public ConditionAffixControlModel(ItemBase itemBase, SubconditionValueType subconditionValueType, List<Affix> affixes, AffixType affixType)
        {
            _itemBase = itemBase;

            ValueType = subconditionValueType;
            _affixes = affixes;
            AffixType = affixType;
        }
    }
}
