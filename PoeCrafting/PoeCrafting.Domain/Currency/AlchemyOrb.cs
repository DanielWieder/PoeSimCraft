using PoeCrafting.Data;
using PoeCrafting.Domain.Crafting;
using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Currency
{
    public class AlchemyOrb : ICurrency
    {
        private ICurrency Chaos { get; }

        public string Name => "Orb of Alchemy";
        public double Value { get; set; }


        public AlchemyOrb(IRandom random)
        {
            Chaos = new ChaosOrb(random);
        }

        public bool Execute(Equipment item)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;
            return Chaos.Execute(item);
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Normal) != EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.Rarity = status.Rarity & ~EquipmentRarity.Normal | EquipmentRarity.Rare;
            }
            else
            {
                status.Rarity = EquipmentRarity.Rare;
            }

            return Chaos.GetNextStatus(status);
        }
    }
}
