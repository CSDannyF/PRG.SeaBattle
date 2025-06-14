using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.SeaBattle;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PRG.EVA01.SeaBattle.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize]
    public class SeaBattleController : Controller
    {
        private readonly ISeaBattleService _seaBattleService;

        public SeaBattleController(ISeaBattleService seaBattleService)
        {
            _seaBattleService = seaBattleService;
        }

        
        //GET: SeaBattle/ThrowBomb
        [HttpGet("SeaBattle/ThrowBomb/{id}")]
        public async Task<IActionResult> ThrowBomb(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = _seaBattleService.GetGameAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            var vm = new ThrowBombViewModel
            {
                GameId = game.Result.Id,
                PlayerName = game.Result.PlayerName
            };
            return View(vm);
        }


        /// <summary>
        /// Gooit een bom ingave in velden en zal de nodige spelinformatie doorgeven aan de view
        /// </summary>
        /// <param name="id">Game id</param>
        /// <param name="letter">Bereik van A t.e.m. F</param>
        /// <param name="number">Bereik van 1 t.e.m. 6param>
        /// <returns>ThrowBomb view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThrowBomb(ThrowBombViewModel vm)
        {
            ModelState.Remove("Result");
            ModelState.Remove("PlayerName");
            ModelState.Remove("TotalSunkenBoats");

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var game = await _seaBattleService.GetGameAsync(vm.GameId);

            if (game == null)
            {
                return NotFound();
            }

            string result = await _seaBattleService.ThrowBombAsync(vm.GameId, vm.LocationLetter, vm.LocationNumber);
            vm.Result = result;
            vm.TotalSunkenBoats = "Total sunken boats: " + _seaBattleService.GetTotalSunkenBoats(game.Boats);
            return View(vm);
        }
    }
}
