using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Handshake.Wrappers.CovidStats
{
    public class CovidStatsService
    {
        public static Stats GetStats()
        {
            // This is the new API with my key, it has a daily request limit of 24
            //var client = new RestClient { BaseUrl = new Uri("https://corona-stats.mobi/api/json.2.0.php?key=3BVeoNyhI6QCd1xRl0JK") };

            //please use this api for tesing . it has unlimited request but has a data thats abit out of date 
            var client = new RestClient { BaseUrl = new Uri("https://bozzaapi.azurewebsites.net/api/stats") };

            var deserial = new JsonDeserializer();
            var request = new RestRequest(Method.GET);
            var response = client.Execute(request);
            var stats = deserial.Deserialize<List<Stats>>(response);
            var data = stats[0];
            return data;
        }
    }
    public class Stats
    {
        public RSA RSA { get; set; }
    }

    public class RSA
    {
        public List<ProvenceCases> GP {get; set; }
        public List<ProvenceCases> EC { get; set; }
        public List<ProvenceCases> FS { get; set; }
        public List<ProvenceCases> KZN { get; set; }
        public List<ProvenceCases> MP { get; set; }
        public List<ProvenceCases> NC { get; set; }
        public List<ProvenceCases> NW { get; set; }
        public List<ProvenceCases> WC { get; set; }
        public List<ProvenceCases> LP { get; set; }
        public List<ProvenceCases> National { get; set; }
    }

    public class ProvenceCases
    {
        public List<int> Cases { get; set; }
    }


}
