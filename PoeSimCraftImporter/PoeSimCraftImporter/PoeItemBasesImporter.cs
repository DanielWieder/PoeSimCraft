using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoeSimCraftImporter.Entities;

namespace PoeSimCraftImporter
{
    // Import item base data from https://github.com/DistilledCode/ScrapeItemBases
    // A modified form of that application is used

    public class PoeItemBasesImporter
    {
        public void Execute()
        {
            var mods = new List<ModEntity>();
            string text = File.ReadAllText("C:\\Users\\danie\\Documents\\Visual Studio 2015\\Projects\\PoeSimCraftImporter\\PoeSimCraftImporter\\Json\\Basetypes.json");

            using (var sr = new StringReader(text))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    var js = new JsonSerializer();
                    IDictionary<string, object> data = js.Deserialize<ExpandoObject>(jr);
                    var weaponSubtypes = data["Weapons"] as IDictionary<string, object>;
                    AddType("Weapon");
                    foreach (var weaponSubtype in weaponSubtypes)
                    {
                        var subtype = weaponSubtype.Key;

                        AddSubtype("Weapon", subtype);
                        var weapons = weaponSubtype.Value as List<dynamic>;

                        foreach (dynamic weapon in weapons)
                        {
                            var damage = weapon.Damage as string;
                            var split = damage.Split(' ');

                            AddWeapon(subtype, weapon.Name, weapon.Level, split[0], split[2], weapon.APS, weapon.DPS,
                                weapon.Str, weapon.Dex, weapon.Int);

                        }
                    }

                    AddType("Armour");
                    var armourSubtypes = data["Armour"] as IDictionary<string, object>;
                    foreach (var armourSubtype in armourSubtypes)
                    {
                        var subtype = armourSubtype.Key;
                        var armours = armourSubtype.Value as List<dynamic>;
                        AddSubtype("Armour", subtype);

                        foreach (dynamic armour in armours)
                        {
                            AddArmour(subtype, armour.Name, armour.Level, armour.Armour, armour.Evasion,
                                armour.EnergyShield, armour.Str, armour.Dex, armour.Int);
                        }
                    }

                    AddType("Jewelry");
                    var jewelrySubtypes = data["Jewelry"]as IDictionary<string, object>;
                    foreach (var jewelrySubtype in jewelrySubtypes)
                    {
                        var subtype = jewelrySubtype.Key;
                        var jewelries = jewelrySubtype.Value as List<dynamic>;
                        AddSubtype("Jewelry", subtype);

                        foreach (dynamic jewelry in jewelries.GroupBy(x => x.Name).Select(group => group.First()))
                        {
                        if (!((string)jewelry.Name).Contains("Talisman"))
                             AddJewelry(subtype, jewelry.Name, jewelry.Level);
                        }
                    }
                }
            }
        }

        public void AddType(string type)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[ItemType]");
            builder.AppendLine("(Name)");
            builder.Append($"Values ('{type}')");

            DbCommand command = new DbCommand();
            // command.Execute(builder.ToString());
        }

        public void AddSubtype(string type, string subtype)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[ItemSubType]");
            builder.AppendLine("(Name, ItemTypeId)");
            builder.Append($"Values (");
            builder.Append($"'{subtype}'");
            builder.Append($", (SELECT ItemTypeId FROM ItemType WHERE Name = '{type}')");
            builder.Append(")");

            DbCommand command = new DbCommand();
            // command.Execute(builder.ToString());
        }

        public void AddWeapon(
            string subtype, 
            string name,
            string level,
            string minDamage,
            string maxDamage,
            string aps,
            string dps,
            string str,
            string dex,
            string intelligence)
        {
            AddItem(subtype, name, level);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[Weapon]");
            builder.AppendLine("(MinDamage, MaxDamage, APS, DPS, Str, Dex, Int, ItemId)");
            builder.Append($"Values (");
            builder.Append($"'{minDamage}'");
            builder.Append($", '{maxDamage}'");
            builder.Append($", '{aps}'");
            builder.Append($", '{dps}'");
            builder.Append($", '{str}'");
            builder.Append($", '{dex}'");
            builder.Append($", '{intelligence}'");
            builder.Append($", (SELECT ItemId FROM Item WHERE Name = '{name.Replace("'", "''")}')");
            builder.Append(")");

            DbCommand command = new DbCommand();
             command.Execute(builder.ToString());
        }
        public void AddArmour(
            string subtype,
            string name,
            string level,
            string armour,
            string evasion,
            string energyShield,
            string str,
            string dex,
            string intelligence)
        {
            AddItem(subtype, name, level);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[Armour]");
            builder.AppendLine("(Armour, Evasion, EnergyShield, Str, Dex, Int, ItemId)");
            builder.Append($"Values (");
            builder.Append($"' {armour}'");
            builder.Append($", '{evasion}'");
            builder.Append($", '{energyShield}'");
            builder.Append($", '{str}'");
            builder.Append($", '{dex}'");
            builder.Append($", '{intelligence}'");
            builder.Append($", (SELECT ItemId FROM Item WHERE Name = '{name.Replace("'", "''")}')");
            builder.Append(")");

            DbCommand command = new DbCommand();
            command.Execute(builder.ToString());
        }

        public void AddJewelry(
            string subtype,
            string name,
            string level)
        {
            AddItem(subtype, name, level);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[Jewelry]");
            builder.AppendLine("(ItemId)");
            builder.Append($"Values (");
            builder.Append($"(SELECT ItemId FROM Item WHERE Name = '{name.Replace("'", "''")}')");
            builder.Append(")");

            DbCommand command = new DbCommand();
            command.Execute(builder.ToString());
        }

        public void AddItem(string subtype,
            string name,
            string level)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("INSERT INTO [dbo].[Item]");
            builder.AppendLine("(Name, Level, ItemSubtypeId)");
            builder.Append($"Values (");
            builder.Append($"'{name.Replace("'", "''")}'");
            builder.Append($", '{level}'");
            builder.Append($", (SELECT ItemSubtypeId FROM ItemSubtype WHERE Name = '{subtype}')");
            builder.Append(")");

            DbCommand command = new DbCommand();
            command.Execute(builder.ToString());
        }
    }
}
