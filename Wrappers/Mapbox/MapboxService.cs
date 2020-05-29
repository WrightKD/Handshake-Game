using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Wrappers.Mapbox
{
    public class MapboxService
    {
        static ILogger _logger;

        public MapboxService(ILogger<MapboxService> logger)
        {
            _logger = logger;
        }

        private static T GetResource<T>(string description, Tuple<string, string>[] parameters = null) where T : new()
        {

            var client = new RestClient { BaseUrl = new Uri($"https://api.mapbox.com/geocoding/v5/mapbox.places/{description}") };

            var request = new RestRequest(Method.GET);

            if (parameters != null)
                foreach (var param in parameters)
                    request.AddParameter(param.Item1, param.Item2, ParameterType.QueryStringWithoutEncode);

            var response = client.Execute<T>(request);
            var content = response.Content;
            if (!response.IsSuccessful)
            {
                _logger.LogError($"Request {request.Method} {request.Resource} was unsuccessful");
            }
            if (response.ErrorException != null)
                throw new ApplicationException($"Unable to retrieve {description}.", response.ErrorException);

            return response.Data;
        }
        //https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=-33.8670522,151.1957362&radius=1500&type=hospital&key=AIzaSyCixAM65iGRvG2fOn3IQ4mjIC0qv3zuLOU
        public static LocationStructure GetLocationDetails(string latitude, string longitude)
        {

            return GetResource<LocationStructure>($"{latitude},{longitude}.json",
              new[] {
                    Tuple.Create("access_token", "pk.eyJ1Ijoid3JpZ2h0a2QiLCJhIjoiY2thN3duNW42MDR5bjJ3c2JibGZjdTc5aSJ9.WfzsXOP8KI0_PVo9su3ubw")
              });
        }

        public static string GetProvince(LocationStructure location)
        {
            return location.Features.FirstOrDefault(l => l.Id.Contains("region")).Text;
        }
        public partial class LocationStructure
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("query")]
            public List<double> Query { get; set; }

            [JsonProperty("features")]
            public List<Feature> Features { get; set; }

            [JsonProperty("attribution")]
            public string Attribution { get; set; }
        }

        public partial class Feature
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("place_type")]
            public List<string> PlaceType { get; set; }

            [JsonProperty("relevance")]
            public long Relevance { get; set; }

            [JsonProperty("properties")]
            public Properties Properties { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("place_name")]
            public string PlaceName { get; set; }

            [JsonProperty("center")]
            public List<double> Center { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
            public long? Address { get; set; }

            [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
            public List<Context> Context { get; set; }

            [JsonProperty("bbox", NullValueHandling = NullValueHandling.Ignore)]
            public List<double> Bbox { get; set; }
        }

        public partial class Context
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("wikidata", NullValueHandling = NullValueHandling.Ignore)]
            public string Wikidata { get; set; }

            [JsonProperty("short_code", NullValueHandling = NullValueHandling.Ignore)]
            public string ShortCode { get; set; }
        }

        public partial class Geometry
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("coordinates")]
            public List<double> Coordinates { get; set; }
        }

        public partial class Properties
        {
            [JsonProperty("accuracy", NullValueHandling = NullValueHandling.Ignore)]
            public string Accuracy { get; set; }

            [JsonProperty("wikidata", NullValueHandling = NullValueHandling.Ignore)]
            public string Wikidata { get; set; }

            [JsonProperty("short_code", NullValueHandling = NullValueHandling.Ignore)]
            public string ShortCode { get; set; }
        }
    }
}
