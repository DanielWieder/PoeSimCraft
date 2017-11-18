using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoeSimCraftImporter.Entities;

namespace PoeSimCraftImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            WipeData();

            ImportItemData itemImport = new ImportItemData();
            itemImport.Execute();

            ImportModData modImport = new ImportModData();
            modImport.Execute();

            var file = File.ReadAllText("Input/CustomScripts.sql");
            DbCommand command = new DbCommand();
            command.Execute(file);
            command.Cleanup();
        }

        private static void WipeData()
        {
            var deleteCommand =
                "TRUNCATE TABLE [dbo].[Affix]\r\nTRUNCATE TABLE [dbo].[AffixSpawnTagMap]\r\nTRUNCATE TABLE [dbo].[Armour]\r\nTRUNCATE TABLE [dbo].[Item]\r\nTRUNCATE TABLE [dbo].[ItemSpawnTagMap]\r\nTRUNCATE TABLE [dbo].[ItemSubType]\r\nTRUNCATE TABLE [dbo].[ItemType]\r\nTRUNCATE TABLE [dbo].[SpawnTag]\r\nTRUNCATE TABLE [dbo].[Weapon]";

            DbCommand command = new DbCommand();
            command.Execute(deleteCommand);
            command.Cleanup();
        }
    }

}



