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
    //System.Diagnostics.Debug.WriteLine(_gameService.player.Score);
    public class GameController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private GameService _gameService;

        public GameController( GameService gS, UserManager<ApplicationUser> userManager)
        {
            _gameService = gS;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await _gameService.LoadPlayerDataAsync(user.Id);

            await _gameService.SavePlayerData();

            //_gameService.player = await _gameService.GetPlayerAsync($"{user.Id}");

            return View();
        }
        public IActionResult GetPlayerData()
        {
            return new JsonResult(_gameService.player);
        }

        public IActionResult GetNPCData()
        {
            return new JsonResult(_gameService.NPCs);
        }

        public IActionResult InitialiseNPCs(double latitude, double longitude)
        {
            _gameService.RandomiseNPCLocations(latitude, longitude);
            return new JsonResult(_gameService.NPCs);
        }

        public bool IsNPCReady(int npcId)
        {
            return _gameService.IsNPCReady(npcId);
        }

        public IActionResult ShakeHand(int npcId)
        {
            _gameService.ShakeHand(npcId);
            return new JsonResult(_gameService.player);
        }

        public IActionResult Sanitise()
        {
            _gameService.UseSanitiser();
            return new JsonResult(new Dictionary<string, string> 
            {
                {"sanitiserCount", _gameService.player.SanitiserCount.ToString() },
                {"isInfected", _gameService.player.IsInfected.ToString() }
            }); 
        }

        public IActionResult UseMask()
        {
            _gameService.UseMask();
           /* return new JsonResult(new Dictionary<string, string>
            {
                {"sanitiserCount", _gameService.player.MaskCount.ToString() }
            });*/
            return new JsonResult(_gameService.player.MaskCount);
        }

        public IActionResult GetShopInventory(int shopId)
        {
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
            return new JsonResult(data);
        }

        public IActionResult BuyMask(int shopId)
        {
            var shop = _gameService.BuyMask(shopId);
            var data = new Dictionary<string, string>
            {
                {"shopSanitiserCount",  shop.MaskCount.ToString()},
                {"playerSanitiserCount",  _gameService.player.MaskCount.ToString()},
                {"playerGold",  _gameService.player.Gold.ToString()}
            };
            return new JsonResult(data);
        }

        public IActionResult InitialiseShops(double latitude, double longitude)
        {
            //Get a list of hospitals in a radius of 1,5km at the center -26.1796856,28.0509079
            // The hospital is the type of place. It is case sensitive 
            // Vaild types : https://developers.google.com/places/supported_types#table1

            //var lat = "-26.1796856";
            // var lon = "28.0509079";
            var lat = latitude.ToString();
            var lon = longitude.ToString();

            var shops = PlaceService.GetPlaces("store", "1500", lat, lon);
            var properties = new List<Dictionary<string, string>>();
            var coordinates = new List<List<double>>();

            foreach (var item in shops.Results)
            {
                var open = item.OpeningHours?.OpenNow;

                var currentProperties = new Dictionary<string, string>
                {
                    {"name", item.Name }
                    //{"open", open.HasValue ? (open.Value ? "Open" : "Closed") : "Unknow" },
                    //{"rating", item.Rating.HasValue ? $"{item.Rating.Value}" : "None" }
                };
                properties.Add(currentProperties);
                coordinates.Add(new List<double> { item.Geometry.Location.Lng, item.Geometry.Location.Lat });
            }
            var geolocation = Converter.GetGeoJSON(coordinates, properties);

            return new JsonResult(geolocation);
        }
    }
}
