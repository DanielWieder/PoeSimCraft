using System;
using System.ComponentModel;
using PoeCrafting.Entities;


namespace PoeCrafting.Data
{
    public class FetchWeaponsByItemName : IFetchWeaponsByItemName
    {
        public string Name { get; set; }

        public ItemBase Execute()
        {
            Database db = new Database();

            string command = $@"SELECT
                                    i.Name,
                                    i.Level,
                                    a.MinDamage,
                                    a.MaxDamage,
                                    a.APS,
                                    a.DPS,
                                    a.Crit,
                                    a.Str,
                                    a.Dex,
                                    a.Int,
                                    ist.Name as ItemType,
                                    it.Name as Type
                                FROM Item i
                                JOIN Weapon a ON i.ItemId = a.ItemId
                                JOIN ItemSubType ist ON ist.ItemSubtypeId = i.ItemSubtypeId
                                JOIN ItemType it ON ist.ItemTypeId = it.ItemTypeId
                                WHERE i.Name = '{Name.Replace("'", "''")}'";

            var entity = db.Fetch<WeaponEntity>(command);
            var item = new ItemBase()
            {
                Name = entity.Name,
                Subtype = entity.Subtype,
                Type = entity.Type
            };
            item.Properties.Add("MinDamage", entity.MinDamage);
            item.Properties.Add("MaxDamage", entity.MaxDamage);
            item.Properties.Add("APS", entity.APS);
            item.Properties.Add("DPS", entity.DPS);
            item.Properties.Add("Str", entity.Str);
            item.Properties.Add("Dex", entity.Dex);
            item.Properties.Add("Int", entity.Int);
            item.Properties.Add("Crit", entity.Crit);

            return item;
        }
    }

    public interface IFetchWeaponsByItemName : IQueryObject<ItemBase>
    {
        string Name { get; set; }
    }
}
