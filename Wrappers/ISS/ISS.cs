using Handshake.Wrappers;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandshakeGame.Wrappers.ISS
{
    public class ISS
    {
        static ILogger _logger;

        public ISS(ILogger<ISS> logger) {
            _logger = logger;
        }

        private static T GetResource<T>(string description, string resource, Tuple<string, string>[] parameters = null) where T : new()
        {
            var client = new RestClient { BaseUrl = new Uri("http://api.open-notify.org") };

            var request = new RestRequest(resource, Method.GET);

            if (parameters != null)
                foreach (var param in parameters)
                    request.AddParameter(param.Item1, param.Item2);

            var response = client.Execute<T>(request);
            if (!response.IsSuccessful) {
                _logger.LogError($"Request {request.Method} {request.Resource} was unsuccessful");
            }
            var content = response.Content;

            if (response.ErrorException != null)
                throw new ApplicationException($"Unable to retrieve {description}.", response.ErrorException);

            return response.Data;
        }

        public static void ShowRoster()
        {
            var roster = GetResource<Roster>("roster", "astros.json");
            var astronautNames = String.Join(", ", roster.People.Select(x => x.Name));

            Console.WriteLine($"There are {roster.Number} people in space: {astronautNames}");
        }

        public static Passes ShowUpcomingPasses(string latitude, string longitude)
        {

            return GetResource<Passes>("next pass", "iss-pass.json",
                new[] { Tuple.Create("lat", latitude), Tuple.Create("lon", longitude) });

            //Console.WriteLine($"The next ISS pass for {latitude} {longitude} is " +
            //                  $"{DateTimeOffset.FromUnixTimeSeconds(nextPass.Risetime)} " +
            //                  $"for {nextPass.Duration} seconds.");
        }

        public static Position ShowCurrentLocation()
        {
            return GetResource<Position>("next pass", "iss-now.json");

        }
    }

    public class Roster
    {
        public int Number { get; set; }
        public List<Astronaut> People { get; set; }
    }
    public class Astronaut
    {
        public string Name { get; set; }
        public string Craft { get; set; }
    }

    public class Passes
    {
        public string Message { get; set; }
        public Request Request { get; set; }
        public List<Pass> Response { get; set; }
    }

    public class Request
    {
        public int Altitude { get; set; }
        public int Datetime { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Passes { get; set; }
    }

    public class Pass
    {
        public int Duration { get; set; }
        public int Risetime { get; set; }
    }

    public class Position
    {
        public Coordinate IssPosition { get; set; }
        public int Timestamp { get; set; }
        public string Message { get; set; }
    }

    public class Coordinate
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
