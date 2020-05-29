using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handshake.Database;
using Handshake.GameLogic;
using Handshake.Models;
using Handshake.Wrappers.Place;
using Handshake.Wrappers.Weather;
using HandshakeGame.GeoJson;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace Handshake.Controllers
{
    public class GameController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GameService _gameService;
        private readonly ILogger _logger;


        public GameController( GameService gS, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
        {
            _gameService = gS;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await _gameService.LoadPlayerDataAsync(user.Id);

            await _gameService.SavePlayerData();

            //_gameService.player = await _gameService.GetPlayerAsync($"{user.Id}");

            return View();
        }
        public async Task<IActionResult> GetPlayerData()
        {
            var user = await _userManager.FindByIdAsync(_gameService.player.Id.ToString());
            _gameService.player.Name = user.UserName;
            _logger.LogInformation("GetPlayerData requested from backend.");
            return new JsonResult(_gameService.player);
        }

        public IActionResult GetNPCData()
        {
            _logger.LogInformation("GetPlayerData requested from backend.");
            return new JsonResult(_gameService.NPCs);
        }

        public IActionResult InitialiseNPCs(double latitude, double longitude)
        {
            _gameService.RandomiseNPCLocations(latitude, longitude);
            _logger.LogInformation("NPCs initialised.");
            return new JsonResult(_gameService.NPCs);
        }

        public bool IsNPCReady(int npcId)
        {
            _logger.LogInformation("IsNPCReady requested from backend.");
            return _gameService.IsNPCReady(npcId);
        }

        public IActionResult ShakeHand(int npcId)
        {
            _gameService.ShakeHand(npcId);
            _logger.LogInformation("Player shaken hand with NPC.");
            return new JsonResult(_gameService.player);
        }

        public IActionResult UseSanitiser()
        {
            _gameService.UseSanitiser();
            _logger.LogInformation("Player used sanitiser.");
            return new JsonResult(new Dictionary<string, string> 
            {
                {"sanitiserCount", _gameService.player.SanitiserCount.ToString() },
                {"isInfected", _gameService.player.IsInfected.ToString() }
            }); 
        }

        public IActionResult UseMask()//change name
        {
            _gameService.UseMask();
            _logger.LogInformation("Player used mask.");
            return new JsonResult(_gameService.maskDuration);
        }
        public bool UseTest()//change name
        {
            _gameService.UseTest();
            _logger.LogInformation("Player used test.");
            return _gameService.player.IsInfected;
        }

        public IActionResult GetShopInventory(int shopId)
        {
            _logger.LogInformation("GetShopInventory requested from backend.");
            return new JsonResult(_gameService.GetShopInventory(shopId));
        }

        public IActionResult BuySanitiser(int shopId)
        {
            var shop = _gameService.BuySanitiser(shopId);
            var data = new Dictionary<string, string>
            {
                {"shopSanitiserCount",  shop.SanitiserCount.ToString()},
                {"playerSanitiserCount",  _gameService.player.SanitiserCount.ToString()},
                {"playerGold",  _gameService.player.Gold.ToString()}
            };
            _logger.LogInformation("Sanitiser bought.");
            return new JsonResult(data);
        }

        public IActionResult BuyMask(int shopId)
        {
            var shop = _gameService.BuyMask(shopId);
            var data = new Dictionary<string, string>
            {
                {"shopMaskCount",  shop.MaskCount.ToString()},
                {"playerMaskCount",  _gameService.player.MaskCount.ToString()},
                {"playerGold",  _gameService.player.Gold.ToString()}
            };
            _logger.LogInformation("Mask bought.");
            return new JsonResult(data);
        }
        public IActionResult BuyTest(int shopId)
        {
            var shop = _gameService.BuyTest(shopId);
            var data = new Dictionary<string, string>
            {
                {"shopTestCount",  shop.TestCount.ToString()},
                {"playerTestCount",  _gameService.player.CovidTests.ToString()},
                {"playerGold",  _gameService.player.Gold.ToString()}
            };
            _logger.LogInformation("Test bought.");
            return new JsonResult(data);
        }

        public IActionResult InitialiseShops(double latitude, double longitude)
        {
            var lat = latitude.ToString();
            var lon = longitude.ToString();

            var shops = PlaceService.GetPlaces("store", "1500", lat, lon);
            var properties = new List<Dictionary<string, string>>();
            var coordinates = new List<List<double>>();

            int shopId = 0;
            foreach (var item in shops.Results)
            {
                _gameService.AddShop(shopId, item.Name);
                var currentProperties = new Dictionary<string, string>
                {
                    {"ID", shopId.ToString() },
                    {"name", item.Name }
                };
                properties.Add(currentProperties);
                coordinates.Add(new List<double> { item.Geometry.Location.Lng, item.Geometry.Location.Lat });
                shopId++;
            }
            var geolocation = Converter.GetGeoJSON(coordinates, properties);
            _logger.LogInformation("Shops intialised.");
            return new JsonResult(geolocation);
        }
    }
}
