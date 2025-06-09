using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.SeaBattle;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class SeaBattleController : Controller
    {
        private readonly SeaBattleDbContext _context;

        private string letters = "ABCDEF";
        private string numbers = "123456";

        public SeaBattleController(SeaBattleDbContext context)
        {
            _context = context;
        }

        
        //GET: SeaBattle/ThrowBomb
        [HttpGet("SeaBattle/ThrowBomb/{id}")]
        public async Task<IActionResult> ThrowBomb(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include Boats voor genest object van Game anders is Boats null
            var game = await _context.Games
                .Include(g => g.Boats)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var vm = new ThrowBombViewModel
            {
                GameId = game.Id,
                PlayerName = game.PlayerName,
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

            //ophalen van Game
            // Eager loading, voor geneste objecten
            // Als ik enkel de Game ophaal is Boats null en zo ook Location. Dus verplicht ik het Boats en daarna ook Locatie mee op te halen
            // https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager
            var game = await _context.Games
                .Include(g => g.Boats)
                .ThenInclude(b => b.Location)
                .FirstOrDefaultAsync(m => m.Id == vm.GameId);

            if (game == null)
            {
                return NotFound();
            }

            // GameLog object om later te op te slaan in database indien er een geldige poging is
            GameLog gameLog = new GameLog()
            {
                GameId = vm.GameId,
                PlayerName = vm.PlayerName,
                LocationLetter = vm.LocationLetter.ToUpper(),
                LocationNumber = vm.LocationNumber,
                CreatedOn = vm.CreatedOn
            };

            // Check als inkomende parameters binnen het bereik zijn
            if (letters.Contains(vm.LocationLetter.ToUpper()) && numbers.Contains(vm.LocationNumber))
            {
                // Loop door de lijst van boten, indien een boot geraakt word, wordt de ThrowBomb view aangeroepen
                foreach (Boat boat in game.Boats)
                {
                    // Indien een boot nog niet gezonken is kan hij geraakt worden
                    if (boat.Status == BoatStatus.Active)
                    {
                        // Check als de bomlocatie overeen komt met de bootlocatie indien true dan hit
                        if (boat.Location.Letter.Equals(vm.LocationLetter.ToUpper()) && boat.Location.Number.Equals(vm.LocationNumber))
                        {
                            boat.Status = BoatStatus.Sunk;
             
                            vm.Result = "HIT";
                            vm.TotalSunkenBoats = "Nr of boats sunk: " + GetNumberOfSunkenBoats(game);
                            gameLog.Result = vm.Result;
                            _context.Add(gameLog);
                            await _context.SaveChangesAsync();
                            return View(vm);
                        }
                    }
                }
                // Indien geen hit               
                vm.Result = "MISS";
                vm.TotalSunkenBoats = "Nr of boats sunk: " + GetNumberOfSunkenBoats(game);
                gameLog.Result = vm.Result;

                _context.Add(gameLog);
                await _context.SaveChangesAsync();
                return View(vm);
            }

            // Indien ongeldige poging maar beide parameters zijn wel meegegeven
            vm.Result = "OUT_OF_BOUNDS";
            vm.TotalSunkenBoats = "Nr of boats sunk: " + GetNumberOfSunkenBoats(game);
            return View(vm);
        }

        /// <summary>
        /// Geeft terug hoeveel boten gezonken zijn via Linq
        /// </summary>
        /// <returns>Aantal gezonken boten</returns>
        private int GetNumberOfSunkenBoats(Game game)
        {
            return game.Boats.Count(b => b.Status == BoatStatus.Sunk);
        }
    }
}
