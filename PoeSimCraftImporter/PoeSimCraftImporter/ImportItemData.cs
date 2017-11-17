using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PoeSimCraftImporter
{
    public class ImportItemData
    {
        string Path = "Input/Items.xml";

        readonly DbCommand _command = new DbCommand();

        public void Execute()
        {
            var serializer = new XmlSerializer(typeof(ItemList));

            StreamReader reader = new StreamReader(Path);
            var items = (ItemList)serializer.Deserialize(reader);

            ImportItemTypes();
            ImportItemSubtypes(items);
            ImportSpawnTags(items);
            ImportItemList(items);

            _command.Cleanup();
        }

        private void ImportSpawnTags(ItemList items)
        {
            var tags = items.ItemBase.SelectMany(x => x.Tags.Split(' ')).Distinct().ToList();

            // add missing tags
            tags.Add("Default");
            tags.Add("StrIntShield");
            tags.Add("StrDexShield");
            tags.Add("StrShield");
            tags.Add("NoAttackMods");
            tags.Add("Focus");
            tags.Add("DexIntShield");
            tags.Add("DexShield");
            tags.Add("NoCasterMods");

            if (tags.GroupBy(x => x).Any(x => x.Count() > 1))
            {
                throw new InvalidOperationException("One of the missing tags was present in the item descriptions");
            }

            foreach (var tag in tags)
            {
                WriteToDb("SpawnTag", "Name", tag);
            }
        }

        private void ImportItemTypes()
        {
            List<string> itemTypes = new List<string>
            {
                "Weapon",
                "Armour",
                "Jewelry",
                "Jewel"
            };

            foreach (var type in itemTypes)
            {
                WriteToDb("ItemType", "Name", type);
            }
        }

        private void ImportItemSubtypes(ItemList items)
        {
            var itemSubtypes = items.ItemBase.Select(x => new {Subtype = x.ItemClass, Type = x.Tags.Split(' ')[0]}).Distinct().ToDictionary(x => x.Subtype, x => x.Type);


            foreach (var item in itemSubtypes)
            {
                var subtype = item.Key;
                var type = item.Value;
                if (type == "Belt" || type == "Ring" || type == "Amulet")
                {
                    type = "Jewelry";
                }

                if (type == "Quiver")
                {
                    type = "Armour";
                }

                _command.Execute($"INSERT INTO ItemSubtype (Name, ItemTypeId) Values ({Util.Format(subtype)}, (SELECT ItemTypeId FROM ItemType WHERE Name={Util.Format(type)}))");
            }
        }

        private void ImportItemList(ItemList items)
        {
            var grouped = items.ItemBase.GroupBy(x => x.Tags.Split(' ')[0]).ToDictionary(gdc => gdc.Key, gdc => gdc.ToList());

            foreach (var itemType in grouped)
            {
                switch (itemType.Key)
                {
                    case "Weapon":
                        ParseWeapons(itemType.Value);
                        break;
                    case "Armour":
                        ParseArmour(itemType.Value);
                        break;
                    case "Belt":
                    case "Ring":
                    case "Amulet":
                    case "Quiver":
                    case "Jewel":
                        ParseGeneric(itemType.Value);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected value");
                }
            }
        }

        private void ParseGeneric(List<ItemBase> items)
        {
            var unDupedList = items.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            for (var index = 0; index < unDupedList.ToList().Count; index++)
            {
                var item = unDupedList[index];
                Console.WriteLine($"{index}/{unDupedList.Count} Adding accessory {item.Name}");
                WriteItemToDb(item);
                WriteSpawnTagsToDb(item);
            }
        }

        private void ParseArmour(List<ItemBase> items)
        {
            var unDupedList = items.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            for (var index = 0; index < unDupedList.Count; index++)
            {
                var item = unDupedList[index];
                WriteItemToDb(item);

                Dictionary<string, string> armourProperties = new Dictionary<string, string>();
                armourProperties.Add("ItemId", $"(SELECT ItemId FROM Item WHERE Name={Util.Format(item.Name)})");

                if (item.Properties.String.Any(x => x.Contains("Armour")))
                {
                    var armour = FetchProperty(item, "Armour");
                    armourProperties.Add("Armour", armour);
                }
                if (item.Properties.String.Any(x => x.Contains("Chance to Block")))
                {
                    var armour = FetchProperty(item, "Chance to Block").Replace("%", "");
                    armourProperties.Add("Block", armour);
                }
                if (item.Properties.String.Any(x => x.Contains("Evasion Rating")))
                {
                    var armour = FetchProperty(item, "Evasion Rating");
                    armourProperties.Add("Evasion", armour);
                }
                if (item.Properties.String.Any(x => x.Contains("Energy Shield")))
                {
                    var armour = FetchProperty(item, "Energy Shield");
                    armourProperties.Add("EnergyShield", armour);
                }

                armourProperties.Add("Str", item.Strength);
                armourProperties.Add("Dex", item.Dexterity);
                armourProperties.Add("Int", item.Intelligence);

                Console.WriteLine($"{index}/{items.Count} Adding armour {item.Name}");
                WriteToDb("Armour", armourProperties);

                WriteSpawnTagsToDb(item);
            }
        }

        private static string FetchProperty(ItemBase item, string propertyName)
        {
            return item.Properties.String.First(x => x.Contains(propertyName)).Replace(propertyName + ": ", "");
        }

        private void ParseWeapons(List<ItemBase> items)
        {
            var unDupedList = items.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            for (var index = 0; index < unDupedList.Count; index++)
            {
                var item = unDupedList[index];
                WriteItemToDb(item);

                Dictionary<string, string> weaponProperties = new Dictionary<string, string>();
                weaponProperties.Add("ItemId", $"(SELECT ItemId FROM Item WHERE Name={Util.Format(item.Name)})");

                var damage = FetchProperty(item, "Physical Damage").Split('-');
                var crit = FetchProperty(item, "Critical Strike Chance").Replace("%", "");
                var aps = FetchProperty(item, "Attacks per Second");

                var dps = (int.Parse(damage[0]) + int.Parse(damage[1])) / 2.0f * float.Parse(aps);

                weaponProperties.Add("MinDamage", damage[0]);
                weaponProperties.Add("MaxDamage", damage[1]);
                weaponProperties.Add("APS", aps);
                weaponProperties.Add("DPS", dps.ToString("n2"));
                weaponProperties.Add("Str", item.Strength);
                weaponProperties.Add("Dex", item.Dexterity);
                weaponProperties.Add("Int", item.Intelligence);
                weaponProperties.Add("Crit", crit);

                Console.WriteLine($"{index}/{items.Count} Adding weapon {item.Name}");
                WriteToDb("Weapon", weaponProperties);

                WriteSpawnTagsToDb(item);
            }
        }

        private void WriteSpawnTagsToDb(ItemBase item)
        {
            foreach (var tag in item.Tags.Split(' '))
            {
                Dictionary<string, string> spawnTagDic = new Dictionary<string, string>
                {
                    { "ItemId", $"(SELECT ItemId from Item WHERE Name={Util.Format(item.Name)})" },
                    { "SpawnTagId", $"(SELECT SpawnTagId from SpawnTag WHERE Name={Util.Format(tag)})"}
                };

                WriteToDb("ItemSpawnTagMap", spawnTagDic);
            }
        }

        private void WriteItemToDb(ItemBase item)
        {
            Dictionary<string, string> itemProperties = new Dictionary<string, string>
            {
                {"ItemSubtypeId", $"(SELECT ItemSubtypeId FROM ItemSubtype WHERE Name={Util.Format(item.ItemClass)})"},
                {"Name", Util.Format(item.Name)},
                {"Level", item.Level}
            };
            WriteToDb("Item", itemProperties);
        }


        private void WriteToDb(string tableName, Dictionary<string, string> dic)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"INSERT INTO [dbo].[{tableName}] (");

            builder.AppendLine(string.Join(", ", dic.Keys));

            builder.Append(") Values (");

            builder.AppendLine(string.Join(", ", dic.Values));
            builder.Append(")");

            _command.Execute(builder.ToString());
        }

        private void WriteToDb(string table, string fieldName, string value)
        {
            _command.Execute($"INSERT INTO {table} ({fieldName}) Values ({Util.Format(value)})");
        }
    }
}
