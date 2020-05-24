using System.Collections.Generic;
using System.Data.SqlClient;
using HandshakeGame.Database;
using HandshakeGame.Database.Models;
using HandshakeGame.GeoJson;
using HandshakeGame.Models;
using HandshakeGame.Wrappers.ISS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
