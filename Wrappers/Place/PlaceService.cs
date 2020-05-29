using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Handshake.Wrappers.Place
{

    
    public class PlaceService
    {
        static ILogger _logger;

        public PlaceService(ILogger<PlaceService> logger) {
            _logger = logger;
        }

        private static T GetResource<T>(string description, Tuple<string, string>[] parameters = null) where T : new()
        {
            var client = new RestClient { BaseUrl = new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json") };

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
        public static PlaceStructure GetPlaces(string type, string radius, string latitude, string longitude)
        {

            return GetResource<PlaceStructure>($"Nearby {type}",
                new[] { 

                    Tuple.Create("location", $"{latitude},{longitude}"), 
                    Tuple.Create("radius", radius), 
                    Tuple.Create("type", type), 
                    Tuple.Create("key", "AIzaSyCixAM65iGRvG2fOn3IQ4mjIC0qv3zuLOU") 

                });

        }


        public partial class PlaceStructure
        {
            [JsonProperty("html_attributions")]
            public List<object> HtmlAttributions { get; set; }

            [JsonProperty("next_page_token")]
            public string NextPageToken { get; set; }

            [JsonProperty("results")]
            public List<Result> Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public partial class Result
        {
            [JsonProperty("business_status")]
            public string BusinessStatus { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("icon")]
            public Uri Icon { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("opening_hours", NullValueHandling = NullValueHandling.Ignore)]
            public OpeningHours OpeningHours { get; set; }

            [JsonProperty("photos", NullValueHandling = NullValueHandling.Ignore)]
            public List<Photo> Photos { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("plus_code")]
            public PlusCode PlusCode { get; set; }

            [JsonProperty("rating", NullValueHandling = NullValueHandling.Ignore)]
            public double? Rating { get; set; }

            [JsonProperty("reference")]
            public string Reference { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("types")]
            public List<string> Types { get; set; }

            [JsonProperty("user_ratings_total", NullValueHandling = NullValueHandling.Ignore)]
            public long? UserRatingsTotal { get; set; }

            [JsonProperty("vicinity")]
            public string Vicinity { get; set; }
        }

        public partial class Geometry
        {
            [JsonProperty("location")]
            public Location Location { get; set; }

            [JsonProperty("viewport")]
            public Viewport Viewport { get; set; }
        }

        public partial class Location
        {
            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lng")]
            public double Lng { get; set; }
        }

        public partial class Viewport
        {
            [JsonProperty("northeast")]
            public Location Northeast { get; set; }

            [JsonProperty("southwest")]
            public Location Southwest { get; set; }
        }

        public partial class OpeningHours
        {
            [JsonProperty("open_now")]
            public bool OpenNow { get; set; }
        }

        public partial class Photo
        {
            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("html_attributions")]
            public List<string> HtmlAttributions { get; set; }

            [JsonProperty("photo_reference")]
            public string PhotoReference { get; set; }

            [JsonProperty("width")]
            public long Width { get; set; }
        }

        public partial class PlusCode
        {
            [JsonProperty("compound_code")]
            public string CompoundCode { get; set; }

            [JsonProperty("global_code")]
            public string GlobalCode { get; set; }
        }

        public enum BusinessStatus { Operational };

        public enum Scope { Google };

        public enum TypeElement { Dentist, Doctor, Establishment, Health, Hospital, LocalGovernmentOffice, Physiotherapist, PointOfInterest };

        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
            {
                BusinessStatusConverter.Singleton,
                ScopeConverter.Singleton,
                TypeElementConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
            };
        }

        internal class BusinessStatusConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(BusinessStatus) || t == typeof(BusinessStatus?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                if (value == "OPERATIONAL")
                {
                    return BusinessStatus.Operational;
                }
                throw new Exception("Cannot unmarshal type BusinessStatus");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (BusinessStatus)untypedValue;
                if (value == BusinessStatus.Operational)
                {
                    serializer.Serialize(writer, "OPERATIONAL");
                    return;
                }
                throw new Exception("Cannot marshal type BusinessStatus");
            }

            public static readonly BusinessStatusConverter Singleton = new BusinessStatusConverter();
        }

        internal class ScopeConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(Scope) || t == typeof(Scope?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                if (value == "GOOGLE")
                {
                    return Scope.Google;
                }
                throw new Exception("Cannot unmarshal type Scope");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (Scope)untypedValue;
                if (value == Scope.Google)
                {
                    serializer.Serialize(writer, "GOOGLE");
                    return;
                }
                throw new Exception("Cannot marshal type Scope");
            }

            public static readonly ScopeConverter Singleton = new ScopeConverter();
        }

        internal class TypeElementConverter : JsonConverter
        {
            public override bool CanConvert(Type t) => t == typeof(TypeElement) || t == typeof(TypeElement?);

            public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                var value = serializer.Deserialize<string>(reader);
                switch (value)
                {
                    case "dentist":
                        return TypeElement.Dentist;
                    case "doctor":
                        return TypeElement.Doctor;
                    case "establishment":
                        return TypeElement.Establishment;
                    case "health":
                        return TypeElement.Health;
                    case "hospital":
                        return TypeElement.Hospital;
                    case "local_government_office":
                        return TypeElement.LocalGovernmentOffice;
                    case "physiotherapist":
                        return TypeElement.Physiotherapist;
                    case "point_of_interest":
                        return TypeElement.PointOfInterest;
                }
                throw new Exception("Cannot unmarshal type TypeElement");
            }

            public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
            {
                if (untypedValue == null)
                {
                    serializer.Serialize(writer, null);
                    return;
                }
                var value = (TypeElement)untypedValue;
                switch (value)
                {
                    case TypeElement.Dentist:
                        serializer.Serialize(writer, "dentist");
                        return;
                    case TypeElement.Doctor:
                        serializer.Serialize(writer, "doctor");
                        return;
                    case TypeElement.Establishment:
                        serializer.Serialize(writer, "establishment");
                        return;
                    case TypeElement.Health:
                        serializer.Serialize(writer, "health");
                        return;
                    case TypeElement.Hospital:
                        serializer.Serialize(writer, "hospital");
                        return;
                    case TypeElement.LocalGovernmentOffice:
                        serializer.Serialize(writer, "local_government_office");
                        return;
                    case TypeElement.Physiotherapist:
                        serializer.Serialize(writer, "physiotherapist");
                        return;
                    case TypeElement.PointOfInterest:
                        serializer.Serialize(writer, "point_of_interest");
                        return;
                }
                throw new Exception("Cannot marshal type TypeElement");
            }

            public static readonly TypeElementConverter Singleton = new TypeElementConverter();
        }
    }
}
