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

namespace HandshakeGame.Controllers
{
    public class AdminController : Controller
    {
        Users users;
        // ILogger logger;
        // public AdminController(ILogger<HomeController> logger, Users users)
        // {
        //     this.users = users;
        //     this.logger = logger;
        // }
        
        [HttpPost]
        public IActionResult DeleteUser()
        {
        
            return  View("Admin");
        }

    }

}