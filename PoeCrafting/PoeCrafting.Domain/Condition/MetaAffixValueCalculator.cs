﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Domain.Condition
{
    public class MetaAffixValueCalculator
    {
        public int GetMetaConditionValue(string modType, ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.Type == AffixType.Suffix).ToList();

            if (modType.Contains(AffixNames.OpenPrefix))
            {
                return prefixes.Count > 3 ? 3 : 3 - prefixes.Count;
            }
            if (modType.Contains(AffixNames.OpenSuffix))
            {
                return suffixes.Count > 3 ? 3 : 3 - suffixes.Count;
            }
            if (modType == AffixNames.TotalEnergyShield)
            {
                return GetDefenseConditionValue(a, ItemProperties.EnergyShield, AffixNames.LocalEnergyShield, AffixGroupings.EnergyShieldPercent, AffixGroupings.HybridEnergyShieldPercent);
            }
            if (modType == AffixNames.TotalArmor)
            {
                return GetDefenseConditionValue(a, ItemProperties.Armour, AffixNames.LocalArmour, AffixGroupings.ArmourPercent, AffixGroupings.HybridArmourPercent);
            }
            if (modType == AffixNames.TotalEvasion)
            {
                return GetDefenseConditionValue(a, ItemProperties.Evasion, AffixNames.LocalEvasion, AffixGroupings.EvasionPercent, AffixGroupings.HybridEvasionPercent);
            }
            if (modType == AffixNames.TotalResistances)
            {
                var resList = new List<int>
                {
                    GetMaxPropertyValue(suffixes, AffixNames.ColdResist),
                    GetMaxPropertyValue(suffixes, AffixNames.FireResist),
                    GetMaxPropertyValue(suffixes, AffixNames.LightningResist),
                    GetMaxPropertyValue(suffixes, AffixNames.ChaosResist),
                    GetMaxPropertyValue(suffixes, AffixNames.AllResist) * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }
            if (modType == AffixNames.TotalElementalResistances)
            {
                var resList = new List<int>
                {
                    GetMaxPropertyValue(suffixes, AffixNames.ColdResist),
                    GetMaxPropertyValue(suffixes, AffixNames.FireResist),
                    GetMaxPropertyValue(suffixes, AffixNames.LightningResist),
                    GetMaxPropertyValue(suffixes, AffixNames.AllResist) * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }
            if (modType == AffixNames.TotalDps)
            {
                return GetDps(GetTotalDamage(a), a);
            }
            if (modType == AffixNames.TotalElementalDps)
            {
                double baseDamage;
                GetPhysicalDamage(a, out baseDamage);
                return GetDps(GetEleDamage(a) + baseDamage, a);
            }
            if (modType == AffixNames.TotalPhysicalDps)
            {
                double baseDamage;
                var physicalDamage = GetPhysicalDamage(a, out baseDamage);
                return GetDps(physicalDamage, a);
            }

            throw new NotImplementedException($"The meta affix {modType} is not recognized");
        }

        private int GetDps(double flatDamage, ConditionContainer a)
        {
            return (int)(flatDamage * GetAttackSpeed(a));
        }

        private double GetAttackSpeed(ConditionContainer a)
        {
            var addedAttackSpeed = 1 + GetMaxPropertyValue(a.Affixes, AffixNames.LocalAttackSpeed) / 100f;
            var baseAttackSpeed = a.ItemBase.Properties.ContainsKey(ItemProperties.APS) ? a.ItemBase.Properties[ItemProperties.APS] : 0;
            return baseAttackSpeed * addedAttackSpeed;
        }

        private int GetTotalDamage(ConditionContainer a)
        {
            double baseDamage;
            var totalPhysDamage = GetPhysicalDamage(a, out baseDamage);

            var eleDamage = GetEleDamage(a);

            // Calculating the best 3 out of the 6 damage affixes can be kind of tricky so I'm basically cheating
            // When we are analyzing a single item it doesn't matter, and when we are looking at the min/max possiblities
            // I expect that either full physical or full elemental will be superior
            if (a.Affixes.Count > 6)
            {
                return (int)Math.Max(eleDamage + baseDamage, totalPhysDamage);
            }
            else
            {
                return (int)(totalPhysDamage + eleDamage);
            }
        }

        private double GetPhysicalDamage(ConditionContainer a, out double baseDamage)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();

            var percentDamage = GetMaxPropertyValue(prefixes, AffixNames.LocalPhysicalPercent) + GetMaxPropertyValue(prefixes, AffixNames.LocalPhysicalHybrid);

            var physical = (GetMaxPropertyValue(prefixes, AffixGroupings.FlatPhysicalDamage) +
                             GetMaxPropertyValue(prefixes, AffixGroupings.FlatPhysicalDamage, 1)) / 2;

            baseDamage = BaseDamage(a);

            return (baseDamage + physical) * (120 + percentDamage) / 100;
        }

        private double BaseDamage(ConditionContainer a)
        {
            var minPhysDamage = a.ItemBase.Properties.ContainsKey(ItemProperties.MinDamage) ? a.ItemBase.Properties[ItemProperties.MinDamage] : 0;
            var maxPhysDamage = a.ItemBase.Properties.ContainsKey(ItemProperties.MaxDamage) ? a.ItemBase.Properties[ItemProperties.MaxDamage] : 0;
            var baseDamage = (minPhysDamage + maxPhysDamage) / 2;
            return baseDamage;
        }

        private double GetEleDamage(ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.Type == AffixType.Prefix).ToList();

            var chaos = (GetMaxPropertyValue(prefixes, AffixGroupings.FlatChaosDamage) +
                         GetMaxPropertyValue(prefixes, AffixGroupings.FlatChaosDamage, 1)) / 2;

            var cold = (GetMaxPropertyValue(prefixes, AffixGroupings.FlatColdDamage) +
                         GetMaxPropertyValue(prefixes, AffixGroupings.FlatColdDamage, 1)) / 2;

            var fire = (GetMaxPropertyValue(prefixes, AffixGroupings.FlatFireDamage) +
                         GetMaxPropertyValue(prefixes, AffixGroupings.FlatFireDamage, 1)) / 2;

            var lightning = (GetMaxPropertyValue(prefixes, AffixGroupings.FlatLightningDamage) +
                         GetMaxPropertyValue(prefixes, AffixGroupings.FlatLightningDamage, 1)) / 2;

            var eleList = new List<int>
            {
                chaos,
                cold,
                fire,
                lightning
            };

            eleList = eleList.OrderByDescending(x => x).ToList();
            eleList = eleList.Take(3).ToList();
            return eleList.Sum();
        }

        private int GetDefenseConditionValue(ConditionContainer a, string propertyName, string flatDefenseName, List<string> percentDefensesNames, List<string> hybridDefenseNames)
        {
            var baseDefense = a.ItemBase.Properties.ContainsKey(propertyName) ? a.ItemBase.Properties[propertyName] : 0;
            var flatDefense = GetMaxPropertyValue(a.Affixes, flatDefenseName);
            var percentDefense = GetMaxPropertyValue(a.Affixes, percentDefensesNames);
            var hybridDefense = GetMaxPropertyValue(a.Affixes, hybridDefenseNames);

            return (int)((baseDefense + flatDefense) * (120 + percentDefense + hybridDefense) / 100);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, List<string> propertyNames, int index = 0)
        {
            var properties = items.Where(x => propertyNames.Contains(x.ModType)).ToList();
            return GetMaxPropertyValue(properties, index);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, string property, int index = 0)
        {
            var properties = items.Where(x => x.ModType == property).ToList();
            return GetMaxPropertyValue(properties, index);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, int index = 0)
        {
            if (!items.Any())
            {
                return 0;
            }
            return items.Max(x => x.Values[index]);
        }
    }
}
