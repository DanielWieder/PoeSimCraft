using System;
using System.ComponentModel;
using PoeCrafting.Entities;


namespace PoeCrafting.Data
{
    public class FetchArmourByItemName : IFetchArmourByItemName
    {
        public string Name { get; set; }

        public ItemBase Execute()
        {
            Database db = new Database();

            string command = $@"SELECT
                                    i.Name,
                                    i.Level,
                                    a.Armour,
                                    a.Evasion,
                                    a.EnergyShield,
                                    a.Block,
                                    a.Str,
                                    a.Dex,
                                    a.Int,
                                    ist.Name as Subtype,
                                    it.Name as Type
                                FROM Item i
                                JOIN Armour a ON i.ItemId = a.ItemId
                                JOIN ItemSubType ist ON ist.ItemSubtypeId = i.ItemSubtypeId
                                JOIN ItemType it ON ist.ItemTypeId = it.ItemTypeId
                                WHERE i.Name = '{Name.Replace("'", "''")}'";

            var entity = db.Fetch<ArmourEntity>(command);
            var item = new ItemBase()
            {
                Name = entity.Name,
                Subtype = entity.Subtype,
                Type = entity.Type
            };
            item.Properties.Add("Evasion", entity.Evasion);
            item.Properties.Add("Armour", entity.Armour);
            item.Properties.Add("Level", entity.Level);
            item.Properties.Add("EnergyShield", entity.EnergyShield);
            item.Properties.Add("Str", entity.Str);
            item.Properties.Add("Dex", entity.Dex);
            item.Properties.Add("Int", entity.Int);
            item.Properties.Add("Block", entity.Block);

            return item;
        }
    }

    public interface IFetchArmourByItemName : IQueryObject<ItemBase>
    {
        string Name { get; set; }
    }
}
