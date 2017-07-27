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
                var unique = _affixes.Where(x => (FirstAffix == null || x.ModType != FirstAffix) &&
                                                  (SecondAffix == null || x.ModType != SecondAffix) &&
                                                  (ThirdAffix == null || x.ModType != ThirdAffix)).ToList();

                var matching = unique.Where(x => x.Type == AffixType.ToString().ToLower()).ToList();

                var mods = matching.Select(x => x.ModType).ToList();

                var distinct = mods.Distinct().ToList();

                distinct.Insert(0, string.Empty);
                return distinct.ToList();
            }
        }

        public ConditionAffixControlModel(SubconditionValueType subconditionValueType, List<Affix> affixes, AffixType affixType)
        {
            ValueType = subconditionValueType;
            _affixes = affixes;
            AffixType = affixType;
        }
    }
}
