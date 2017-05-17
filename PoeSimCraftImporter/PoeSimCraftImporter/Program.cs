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
            PoeModWeightImporter importer = new PoeModWeightImporter();
            importer.Execute();
        }

    }

}
