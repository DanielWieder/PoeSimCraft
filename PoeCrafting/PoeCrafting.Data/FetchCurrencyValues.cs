using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Data
{
    public class FetchCurrencyValues : IFetchCurrencyValues
    {
        private Dictionary<string, string> leagueLookup = new Dictionary<string, string>
        {
            {"Standard", "standard"},
            {"Hardcore", "hardcore"},
            {"Abyss", "tmpstandard"},
            {"HC Abyss", "tmphardcore"},
        };

        public string url = @"http://poe.ninja/api/Data/GetCurrencyOverview?league=";

        public string League { get; set; }

        public Dictionary<string, double> Execute()
        {
            Dictionary<string, double> values = new Dictionary<string, double>();

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url + leagueLookup[League]);
                using (var sr = new StringReader(json))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        var js = new JsonSerializer();
                        IDictionary<string, object> topLevel = js.Deserialize<ExpandoObject>(jr);

                        var currencies = topLevel["lines"] as IList<object>;

                        foreach (dynamic currency in currencies)
                        {
                            string type = currency.currencyTypeName;
                            double value = currency.chaosEquivalent;

                            values.Add(type, value);
                        }
                    }
                }
            }
            return values;
        }
    }

    public interface IFetchCurrencyValues : IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
