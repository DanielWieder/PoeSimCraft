using System;
using System.ComponentModel;
using PoeCrafting.Entities;


namespace PoeCrafting.Data
{
    public class FetchAccessoriesByItemName : IFetchAccessoriesByItemName
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

            var entity = db.Fetch<AccessoryEntity>(command);
            var item = new ItemBase()
            {
                Name = entity.Name,
                Subtype = entity.Subtype,
                Type = entity.Type
            };

            return item;
        }
    }

    public interface IFetchAccessoriesByItemName : IQueryObject<ItemBase>
    {
        string Name { get; set; }
    }
}
