using System.Collections.Generic;
using System.Linq;

namespace HandshakeGame.GeoJson
{
    public class Converter
    {
        public static GeoJSON<T> GetGeoJSON<T>(List<T> Coordinates, Dictionary<string, string> Properties)
        {
            return new GeoJSON<T>(new List<Feature<T>> { new Feature<T>(Coordinates, Properties) });
        }

        public static GeoJSON<T> GetGeoJSON<T>(List<List<T>> Coordinates, List<Dictionary<string, string>> Properties)
        {
            var modal = new List<Feature<T>>();

            var CoordinatesAndProperties = Coordinates.Zip(Properties, (cord, prop) => new { Coordinates = cord, Properties = prop });

            foreach (var value in CoordinatesAndProperties)
            {
                modal.Add(new Feature<T>(value.Coordinates, value.Properties));
            }

            return new GeoJSON<T>(modal);
        }
    }

    public class GeoJSON<T>
    {
        public string type { get; set; }

        public List<Feature<T>> features { get; set; }

        public GeoJSON(List<Feature<T>> features)
        {
            type = "FeatureCollection";
            this.features = features;
        }

    }

    public class Feature<T>
    {
        public string type { get; set; }

        public Geometry<T> geometry { get; set; }

        public Dictionary<string, string> properties { get; set; }

        public Feature(List<T> coordinates, Dictionary<string, string> properties)
        {
            geometry = new Geometry<T>("Point", coordinates);
            type = "Feature";
            this.properties = properties;
        }
    }

    public class Geometry<T>
    {
        public string type { get; set; }
        public List<T> coordinates { get; set; }

        public Geometry(string type, List<T> coordinates)
        {
            this.type = type;
            this.coordinates = coordinates;
        }
    }
}
