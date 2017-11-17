using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoeSimCraftImporter.Entities;

namespace PoeSimCraftImporter
{
    // Import mod data from https://github.com/brather1ng/RePoE

    public class ImportModData
    {
        readonly DbCommand _command = new DbCommand();

        public void Execute()
        {
            string text = File.ReadAllText("Input\\mods.json");

            var mods = ModsDeserializeJson(text);

            mods = mods.Where(
                    x => x.GenerationType != "unique" &&
                         x.GenerationType != "enchantment" &&
                         x.GenerationType != "tempest" &&
                         x.Domain != "jewel" &&
                         x.Domain != "leaguestone" &&
                         x.Domain != "flask" &&
                         x.Domain != "area" &&
                         x.Domain != "atlas")
                .ToList();

            for (var index = 0; index < mods.Count; index++)
            {
                var mod = mods[index];
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
                    $"{Util.Format(mod.Domain)}, {Util.Format(mod.GenerationType)}, {Util.Format(mod.Group)}, {Util.Format(mod.Name)}, {Util.Format(mod.ModName)}, {mod.LevelReq}");

                foreach (Stat stat in mod.Stats)
                {
                    builder.Append($", {Util.Format(stat.Id)}, {stat.Min}, {stat.Max}");
                }

                builder.Append(")");

                _command.Execute(builder.ToString());

                Console.WriteLine($"{index}/{mods.Count} Affixes");
            }

            for (var i = 0; i < mods.Count; i++)
            {
                var mod = mods[i];
                for (var index = 0; index < mod.SpawnTags.Count; index++)
                {
                    var tag = mod.SpawnTags[index];
                    var val = tag.Weight;
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("INSERT INTO [dbo].[AffixSpawnTagMap]");
                    builder.AppendLine("(AffixId, SpawnTagId, Weight, Priority, ST)");
                    builder.Append("Values (");
                    builder.Append($"(SELECT AffixId FROM Affix WHERE ModName = {Util.Format(mod.ModName)})");
                    builder.Append($", (SELECT SpawnTagId FROM SpawnTag WHERE Name = {Util.Format(tag.Name)})");
                    builder.Append($", {tag.Weight}, {tag.Priority}, {Util.Format(tag.Name)}");
                    builder.Append(")");
                    _command.Execute(builder.ToString());
                }
                Console.WriteLine($"{i}/{mods.Count} affix spawn tags");
            }

            _command.Cleanup();
        }

        private static List<ModEntity> ModsDeserializeJson(string text)
        {
            var mods = new List<ModEntity>();

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
                        var group = mod.Value["group"].Value<string>();
                        var name = mod.Value["name"].Value<string>().Replace("'", "''");
                        var required_level = mod.Value["required_level"].Value<string>();

                        var spawnTags = mod.Value["spawn_weights"]
                            .Select(x => x.First.Value<JProperty>())
                            .Select((tag, i) => new SpawnWeight
                            {
                                Name = tag.First.Value<string>(),
                                Weight = tag.Next.First.Value<int>(),
                                Priority = i
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
    }
}
