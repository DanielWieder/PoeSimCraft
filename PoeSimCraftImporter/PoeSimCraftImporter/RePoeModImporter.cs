using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoeSimCraftImporter.Entities;

namespace PoeSimCraftImporter
{
    // Import mod data from https://github.com/brather1ng/RePoE

    public class RePoeModImporter
    {
        public void Execute()
        {
            var mods = GetMods();

            mods = mods.Where(
                    x => x.GenerationType != "unique" &&
                         x.GenerationType != "enchantment" &&
                         x.Domain != "jewel" &&
                         x.Domain != "leaguestone" &&
                         x.Domain != "flask")
                .ToList();

            var generationTypes = mods.Select(x => x.GenerationType).Distinct().ToList();
            var domains = mods.Select(x => x.Domain).Distinct().ToList();
            var spawnTags = mods.SelectMany(x => x.SpawnTags).Select(x => x.Name).Distinct().ToList();
            var tags = mods.SelectMany(x => x.Tags).Distinct().ToList();



            foreach (var spawnTag in spawnTags)
            {
                var command = String.Format(@"INSERT INTO SpawnTag (Name)
                                VALUES ('{0}');", spawnTag);
                //    DatabaseUpdate(command);
            }

            foreach (var tag in tags)
            {
                var command = $@"INSERT INTO Tag (Name) VALUES ('{tag}');";
                //           DatabaseUpdate(command);
            }

            foreach (var mod in mods)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("INSERT INTO Affix");
                builder.AppendLine("(Domain, GenerationType, [Group], [Name], ModName, ILvl");
                for (var i = 1; i <= mod.Stats.Count; i++)
                {
                    builder.Append($", StatName{i}, StatMin{i}, StatMax{i}");
                }
                builder.Append(")");

                builder.AppendLine("Values (");

                builder.AppendLine(
                    $"'{mod.Domain}', '{mod.GenerationType}', '{mod.Group}', '{mod.Name}', '{mod.ModName}', {mod.LevelReq}");

                foreach (Stat stat in mod.Stats)
                {
                    builder.Append(string.Format(", '{0}', {1}, {2}", stat.Id, stat.Min, stat.Max));
                }

                builder.Append(")");



                //          DatabaseUpdate(builder.ToString());
            }

            foreach (var mod in mods)
            {
                foreach (var tag in mod.SpawnTags)
                {
                    var val = tag.Value ? 1 : 0;
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("INSERT INTO [dbo].[AffixSpawnTagMap]");
                    builder.AppendLine("(AffixId, SpawnTagId, [Value])");
                    builder.Append("Values (");
                    builder.Append($"(SELECT AffixId FROM Affix WHERE ModName = '{mod.ModName}')");
                    builder.Append($", (SELECT SpawnTagId FROM SpawnTag WHERE Name = '{tag.Name}')");
                    builder.Append($", '{val}'");
                    builder.Append(")");

                    //  DatabaseUpdate(builder.ToString());
                }
            }

            foreach (var mod in mods)
            {
                foreach (var tag in mod.Tags)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("INSERT INTO [dbo].[AffixTagMap]");
                    builder.AppendLine("(AffixId, TagId)");
                    builder.Append("Values (");
                    builder.Append($"(SELECT AffixId FROM Affix WHERE ModName = '{mod.ModName}')");
                    builder.Append($", (SELECT TagId FROM Tag WHERE Name = '{tag}')");
                    builder.Append(")");

                    DbCommand command = new DbCommand();
                    command.Execute(builder.ToString());
                }
            }
        }

        private static List<ModEntity> GetMods()
        {
            var mods = new List<ModEntity>();
            string text = File.ReadAllText("Json\\mods.json");

            using (var sr = new StringReader(text))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    var js = new JsonSerializer();
                    var u = js.Deserialize<JObject>(jr);

                    foreach (var mod in u)
                    {
                        var modName = mod.Key;

                        var domain = mod.Value["domain"].Value<string>();
                        var generation_type = mod.Value["generation_type"].Value<string>();
                        //  var grants_buff = mod.Value["grants_buff"];
                        //  var grants_effect = mod.Value["grants_effect"];
                        var group = mod.Value["group"].Value<string>();
                        var is_essence_only = mod.Value["is_essence_only"].Value<string>();
                        var name = mod.Value["name"].Value<string>().Replace("'", "''");
                        var required_level = mod.Value["required_level"].Value<string>();

                        var spawnTags = mod.Value["spawn_tags"]
                            .Select(x => x.First.Value<JProperty>())
                            .Select(tag => new SpawnTag
                            {
                                Value = tag.First().Value<bool>(),
                                Name = tag.Name
                            })
                            .ToList();

                        var stats = mod.Value["stats"]
                            .Select(stat => new Stat
                            {
                                Id = stat["id"].Value<string>(),
                                Min = stat["min"].Value<int>(),
                                Max = stat["max"].Value<int>()
                            })
                            .ToList();

                        var tagList = mod.Value["adds_tags"]
                            .Select(tag => tag.Value<string>())
                            .ToList();

                        var modEntity = new ModEntity
                        {
                            ModName = modName,
                            GenerationType = generation_type,
                            Group = group,
                            Name = name,
                            LevelReq = required_level,
                            SpawnTags = spawnTags,
                            Tags = tagList,
                            Stats = stats,
                            Domain = domain
                        };

                        mods.Add(modEntity);
                    }
                }
            }
            return mods;
        }

        private static List<string> GetValues(JObject u, string val)
        {
            List<string> mods = new List<string>();
            foreach (var mod in u)
            {
                var generation_type = mod.Value[val];
                var generation_type_value = generation_type.Value<string>();
                mods.Add(generation_type_value);
            }
            return mods.Distinct().ToList();
        }

    }
}
