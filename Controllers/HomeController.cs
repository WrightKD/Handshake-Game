using System.Collections.Generic;
using System.Threading.Tasks;
using Handshake.GeoJson;
using HandshakeGame.Wrappers.ISS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HandshakeGame.Controllers
{
    
    public class HomeController : Controller
    {

        // GET: /<controller>/
        [Authorize]
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

        public IActionResult SignOut()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // await HttpContext.SignOutAsync("Google");

            // return View("Login");

            //Need work out how to do sign out

            return Redirect("https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:44351/Login");
        }
    }
}
