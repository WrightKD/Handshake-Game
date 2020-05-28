using System.Collections.Generic;
using System.Data.SqlClient;
using Handshake.Wrappers.CovidStats;
using Handshake.Wrappers.Place;
using Handshake.Wrappers.Weather;
using System.Diagnostics;
using HandshakeGame.Database;
using HandshakeGame.Database.Models;
using HandshakeGame.GeoJson;
using HandshakeGame.Models;
using HandshakeGame.Wrappers.ISS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HandshakeGame.Controllers
{
    public class HomeController : Controller
    {
        IDBModel<User, UserCreate> users;
        ILogger logger;
        public HomeController(ILogger<HomeController> logger, IDBModel<User, UserCreate> users)
        {
            this.users = users;
            this.logger = logger;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetCovidStats()
        {
            return Json(CovidStatsService.GetStats());
        }

        public IActionResult Location()
        {



            // Add the ISS as an easter egg?
            //var currentlocation = ISS.ShowCurrentLocation();

            //var currentProperties = new Dictionary<string, string>
            //{
            //    {"message", currentlocation.Message},
            //    {"timestamp", currentlocation.Timestamp.ToString()},
            //    {"icon", "ISS" }
            //};

            //var geolocation = Converter.GetGeoJSON(
            //    new List<string> { currentlocation.IssPosition.Latitude, currentlocation.IssPosition.Longitude },
            //    currentProperties
            //);




            //Get a list of hospitals in a radius of 1,5km at the center -26.1796856,28.0509079
            // The hospital is the type of place. It is case sensitive 
            // Vaild types : https://developers.google.com/places/supported_types#table1

            var lat = "-26.1796856";
            var lon = "28.0509079";

            var hospitals = PlaceService.GetPlaces("supermarket", "1500", lat, lon);

            var weather = WeatherService.GetWeatherDetails(lat, lon);

            var properties = new List<Dictionary<string, string>>();
            var coordinates = new List<List<double>>();

            foreach (var item in hospitals.Results)
            {
                var open = item.OpeningHours?.OpenNow;

                var currentProperties = new Dictionary<string, string>
                {
                    {"name", item.Name },
                    {"open", open.HasValue ? (open.Value ? "Open" : "Closed") : "Unknow" },
                    {"rating", item.Rating.HasValue ? $"{item.Rating.Value}" : "None" },
                    {"temp" ,  weather.Main.Temp.ToString()}
                };

                properties.Add(currentProperties);
                coordinates.Add(new List<double> { item.Geometry.Location.Lng, item.Geometry.Location.Lat });

            }


            var geolocation = Converter.GetGeoJSON(coordinates, properties);

            return new JsonResult(geolocation);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}