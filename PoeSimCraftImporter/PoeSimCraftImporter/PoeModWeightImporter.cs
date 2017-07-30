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
    // I added these to the JSON after the fact, otherwise I'd import them in the mod importer
    public class PoeModWeightImporter
    {
        public void Execute()
        {
            var mods = GetMods();
        }

        private static List<ModEntity> GetMods()
        {
            var mods = new List<ModEntity>();
            string text = File.ReadAllText("C:\\Users\\danie\\Documents\\Visual Studio 2015\\Projects\\PoeSimCraftImporter\\PoeSimCraftImporter\\Json\\mods.json");

            using (var sr = new StringReader(text))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    var js = new JsonSerializer();
                    IDictionary<string, object> u = js.Deserialize<ExpandoObject>(jr);

                    StringBuilder builder = new StringBuilder();

                    foreach (string mod in u.Keys)
                    {

                        var modValue = u[mod] as IDictionary<string, object>;
                        var modName = mod;
                        var name = modValue["name"].ToString().Replace("'", "''");
                        var spawnTagKeys = ((ICollection)modValue["spawn_weight_tags_keys"]).Cast<string>().ToList();
                        var spawnTagWeights = ((ICollection)modValue["spawn_weight_tags_values"]).Cast<System.Int64>().ToList();

                        for ( int i = 0; i < spawnTagKeys.Count; i++)
                        {
                            var key = spawnTagKeys[i];
                            var weight = spawnTagWeights[i];
                            builder.AppendLine("UPDATE astm");
                            builder.AppendLine("SET Priority = " + i);
                            builder.AppendLine("FROM [dbo].[AffixSpawnTagMap] astm");
                            builder.AppendLine($"JOIN Affix a ON a.AffixId = astm.AffixId");
                            builder.AppendLine($"JOIN SpawnTag t ON t.SpawnTagId = astm.SpawnTagId");
                            builder.Append("WHERE");
                            builder.Append($" a.Name = '{name}'");
                            builder.Append($" AND a.ModName = '{modName}'");
                            builder.Append($" AND t.Name = '{key}'");
                        }

                        builder.Append(Environment.NewLine);
                    }

                    DbCommand command = new DbCommand();
                    command.Execute(builder.ToString());
                }
            }

            return mods;
        }
    }
}
