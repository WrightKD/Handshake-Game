using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handshake.GameLogic;
using Handshake.Models;
using Handshake.Wrappers.Place;
using Handshake.Wrappers.Weather;
using HandshakeGame.GeoJson;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Handshake.Controllers
{
    //System.Diagnostics.Debug.WriteLine(_gameService.player.Score);
    public class GameController : Controller
    {
        GameService _gameService;

        public GameController( GameService gS)
        {
            _gameService = gS;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult InitialiseShops()
        {
            //Get a list of hospitals in a radius of 1,5km at the center -26.1796856,28.0509079
            // The hospital is the type of place. It is case sensitive 
            // Vaild types : https://developers.google.com/places/supported_types#table1

            var lat = "-26.1796856";
            var lon = "28.0509079";

            var hospitals = PlaceService.GetPlaces("store", "1500", lat, lon);

           // var weather = WeatherService.GetWeatherDetails(lat, lon);

            var properties = new List<Dictionary<string, string>>();
            var coordinates = new List<List<double>>();

            foreach (var item in hospitals.Results)
            {
                var open = item.OpeningHours?.OpenNow;

                var currentProperties = new Dictionary<string, string>
                {
                    {"name", item.Name }
                    //{"open", open.HasValue ? (open.Value ? "Open" : "Closed") : "Unknow" },
                    //{"rating", item.Rating.HasValue ? $"{item.Rating.Value}" : "None" },
                    //{"temp" ,  weather.Main.Temp.ToString()}
                };

                properties.Add(currentProperties);
                coordinates.Add(new List<double> { item.Geometry.Location.Lng, item.Geometry.Location.Lat });

            }


            var geolocation = Converter.GetGeoJSON(coordinates, properties);

            return new JsonResult(geolocation);
        }
    }
}
