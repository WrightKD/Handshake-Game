using System.Collections.Generic;
using HandshakeGame.GeoJson;
using HandshakeGame.Wrappers.ISS;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HandshakeGame.Controllers
{
    public class HomeController : Controller
    {

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

   
        public IActionResult Location()
        {
            var currentlocation = ISS.ShowCurrentLocation();

            var currentProperties = new Dictionary<string, string>
            {
                {"message", currentlocation.Message},
                {"timestamp", currentlocation.Timestamp.ToString()},
                {"icon", "ISS" }
            };

            //var upcomingLocation = ISS.ShowUpcomingPasses();

            var geolocation = Converter.GetGeoJSON(
                new List<string> { currentlocation.IssPosition.Latitude, currentlocation.IssPosition.Longitude },
                currentProperties
            );

            return new JsonResult(geolocation);
        }
    }
}
