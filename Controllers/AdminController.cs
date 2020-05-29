using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Handshake.Database;
using Handshake.Models;
using HandshakeGame.Controllers;
using HandshakeGame.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Models.AccountViewModels;
using Handshake.Services;

namespace Handshake.Controllers
{
    
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;
        private AdminService  _adminService;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger,AdminService  adminService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Users = await _adminService.GetUsers();
            _logger.LogInformation(" Admin is Displaying All Users ");

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                 return NotFound();
            }

            
             ViewBag.Users = await _adminService.GetUserDetails(id);
              _logger.LogInformation(" Admin is Displaying Details for User by Id ");
             return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var inRole = await _userManager.IsInRoleAsync(user, "Admin");

            if (inRole)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            _logger.LogInformation(" Admin is Editing role for User :" + id);

            int userID = short.Parse(id);
            ViewBag.Users = await _adminService.GetUserDetails(userID);
            return RedirectToAction("Index");
        }

    }


}