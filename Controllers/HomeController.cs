﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BasketballInfo.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BasketballInfo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BasketballController _basketballController;
        private readonly TeamsController _teamsController;

        public HomeController(ILogger<HomeController> logger, BasketballController basketballController, TeamsController teamsController)
        {
            _logger = logger;
            _basketballController = basketballController;
            _teamsController = teamsController;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Teams()
        {
            try
            {
                var teams = await _teamsController.GetTeams();
                return View(teams);
            }
            catch (Exception ex)
            {
                // Lida com a exceção de maneira apropriada para sua aplicação
                return View("Error"); // Crie uma visão de erro ou redirecione para outra página
            }
        }

        public async Task<IActionResult> Discover(DateTime? selectDate)
        {
            var game = await _basketballController.GetNbaGames(selectDate);

            return View(game);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
