using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Wrappers.Weather
{
    public class WeatherService
    {
        static ILogger _logger;

        public WeatherService(ILogger logger)
        {
            _logger = logger;
        }

        //https://api.openweathermap.org/data/2.5/weather?lat=-26.1796856&lon=28.0509079&appid=1966bff07554d73e9faeb640c8d63bae
        private static T GetResource<T>(string description, Tuple<string, string>[] parameters = null) where T : new()
        {
            var client = new RestClient { BaseUrl = new Uri("https://api.openweathermap.org/data/2.5/weather") };

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

        public static WeatherStructure GetWeatherDetails(string latitude, string longitude)
        {

            return GetResource<WeatherStructure>($"{nameof(WeatherStructure)}",
                new[] {

                    Tuple.Create("lat", $"{latitude}"),
                    Tuple.Create("lon", $"{longitude}"),
                    Tuple.Create("units", "metric"),
                    Tuple.Create("appid", "")

                });
        }

        public partial class WeatherStructure
        {
            [JsonProperty("coord")]
            public Coord Coord { get; set; }

            [JsonProperty("weather")]
            public List<Weather> Weather { get; set; }

            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("main")]
            public Main Main { get; set; }

            [JsonProperty("wind")]
            public Wind Wind { get; set; }

            [JsonProperty("clouds")]
            public Clouds Clouds { get; set; }

            [JsonProperty("dt")]
            public long Dt { get; set; }

            [JsonProperty("sys")]
            public Sys Sys { get; set; }

            [JsonProperty("timezone")]
            public long Timezone { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("cod")]
            public long Cod { get; set; }
        }

        public partial class Clouds
        {
            [JsonProperty("all")]
            public long All { get; set; }
        }

        public partial class Coord
        {
            [JsonProperty("lon")]
            public double Lon { get; set; }

            [JsonProperty("lat")]
            public double Lat { get; set; }
        }

        public partial class Main
        {
            [JsonProperty("temp")]
            public double Temp { get; set; }

            [JsonProperty("feels_like")]
            public double FeelsLike { get; set; }

            [JsonProperty("temp_min")]
            public double TempMin { get; set; }

            [JsonProperty("temp_max")]
            public double TempMax { get; set; }

            [JsonProperty("pressure")]
            public long Pressure { get; set; }

            [JsonProperty("humidity")]
            public long Humidity { get; set; }
        }

        public partial class Sys
        {
            [JsonProperty("type")]
            public long Type { get; set; }

            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("sunrise")]
            public long Sunrise { get; set; }

            [JsonProperty("sunset")]
            public long Sunset { get; set; }
        }

        public partial class Weather
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("main")]
            public string Main { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("icon")]
            public string Icon { get; set; }
        }

        public partial class Wind
        {
            [JsonProperty("speed")]
            public double Speed { get; set; }

            [JsonProperty("deg")]
            public long Deg { get; set; }
        }
    }
}
