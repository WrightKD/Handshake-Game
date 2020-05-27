using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc; 
using System.Data.SqlClient;
using HandshakeGame.Database;
using HandshakeGame.Database.Models;
using HandshakeGame.GeoJson;
using HandshakeGame.Models;
using HandshakeGame.Wrappers.ISS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HandshakeGame.Controllers
{
    public class HomeController : Controller
    {
        Users users;
        ILogger logger;
        public HomeController(ILogger<HomeController> logger, Users users)
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

       [HttpPost]
        public IActionResult AdminLogin(IFormCollection form)
        {
            
            string user = form["email"].ToString();
            string pass = form["psw"].ToString();

            Console.WriteLine(user + " --------------" + pass);
            
            
            try{
                if(users.GetOneByUsername(user).IsAdmin==true){
                     Console.WriteLine(" users :"+users.GetOneByUsername(user).Email);
                     users.DeleteByUsername(user);
                      return View("Admin");
                }
                else
                    Console.WriteLine("user is not admin");
            } catch(Exception e)
            {
                Console.WriteLine(e);

            }
            return View("Index");
        }
    }
}

//commit . get data from form