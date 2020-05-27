using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Handshake.GameLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Handshake.Controllers
{
    public class GameController : Controller
    {
        GameService _gameService;

        public GameController(ILogger<GameController> logger, GameService gS)
        {
            _gameService = gS;
        }

        public IActionResult Index()
        {
            _gameService.GetPlayerData();
            return View();
        }

        public IActionResult ShakeHand()
        {
            _gameService.ShakeHand(0.5);
            System.Diagnostics.Debug.WriteLine(_gameService._player.Score);

             return new JsonResult(_gameService._player.Score);
        }

        public IActionResult GetNPCData()
        {
            return new JsonResult(_gameService.NPCs);
        }

        public IActionResult InitialiseNPCs(double latitude, double longitude)
        {
            System.Diagnostics.Debug.WriteLine("ran");

            _gameService.RandomiseNPCLocations(latitude, longitude);
            return new JsonResult(_gameService.NPCs);
        }

        public IActionResult test(double a)
        {
            return new JsonResult(a);
        }
    }
}
