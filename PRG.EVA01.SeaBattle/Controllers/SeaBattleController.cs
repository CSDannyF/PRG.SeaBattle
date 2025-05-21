using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class SeaBattleController : Controller
    {
        private readonly SeaBattleDbContext _context;

        private string letters = "ABCDEFG";
        private string numbers = "123456";
        private int numberOfBoats = 6;
        private List<Boat> _boats = new List<Boat>();

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

            ViewBag.SeaBattleInfo = new List<string>
            {
                "Game id: " + game.Id,
                "Speler: " + game.PlayerName,
                "Gestart met spelen: " + game.StartedPlayingOn,
            };
            ViewBag.SunkenShips = "Aantal gezonken boten: " + GetNumberOfSunkenBoats(game) + "/" + game.Boats.Count;

            return View();
        }

        /// <summary>
        /// Gooit een bom ingave in velden en zal de nodige spelinformatie doorgeven aan de view
        /// </summary>
        /// <param name="id">Game id</param>
        /// <param name="letter">Bereik van A t.e.m. F</param>
        /// <param name="number">Bereik van 1 t.e.m. 6param>
        /// <returns>ThrowBomb view</returns>
        public async Task<IActionResult> ThrowBomb(string letter, string number, int Id)
        {
            //ophalen van Game
            if (Id == null)
            {
                return NotFound();
            }

            // Eager loading, voor geneste objecten
            // Als ik enkel de Game ophaal is Boats null en zo ook Location. Dus verplicht ik het Boats en daarna ook Locatie mee op te halen
            // https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager
            var game = await _context.Games
                .Include(g => g.Boats)
                .ThenInclude(b => b.Location)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (game == null)
            {
                return NotFound();
            }

            // GameLog object om later te op te slaan in database indien er een geldige poging is
            GameLog gameLog = new GameLog()
            {
                GameId = game.Id,
                PlayerName = game.PlayerName,
                LocationLetter = letter,
                LocationNumber = number,
                CreatedOn = DateTime.Now
            };

            // Algemene info over het spel
            ViewBag.SeaBattleInfo = new List<string>
            {
                "Game id: " + game.Id,
                "Speler: " + game.PlayerName,
                "Gestart met spelen: " + game.StartedPlayingOn,
            };

            // Check als inkomende parameters binnen het bereik zijn
            if (letters.Contains(letter.ToUpper()) && numbers.Contains(number))
            {
                // Loop door de lijst van boten, indien een boot geraakt word, wordt de ThrowBomb view aangeroepen
                foreach (Boat boat in game.Boats)
                {
                    // Indien een boot nog niet gezonken is kan hij geraakt worden
                    if (boat.Status == BoatStatus.Active)
                    {
                        // Check als de bomlocatie overeen komt met de bootlocatie indien true dan hit
                        if (boat.Location.Letter.Equals(letter.ToUpper()) && boat.Location.Number.Equals(number))
                        {
                            boat.Status = BoatStatus.Sunk;
                            ViewBag.BombInfo = new List<string>
                            {
                                "Locatie: " + letter + " / " + number,
                                "Resultaat: HIT!!!",
                            };
                            ViewBag.SunkenShips = "Aantal gezonken boten: " + GetNumberOfSunkenBoats(game) + "/" + game.Boats.Count;
                            gameLog.Result = "HIT!!!";
                            _context.Add(gameLog);
                            await _context.SaveChangesAsync();
                            return View();
                        }
                    }
                }
                // Indien geen hit
                ViewBag.BombInfo = new List<string>
                {
                    "Locatie: " + letter + " / " + number,
                    "Resultaat: MIS!!",
                };
                ViewBag.SunkenShips = "Aantal gezonken boten: " + GetNumberOfSunkenBoats(game) + "/" + game.Boats.Count;
                gameLog.Result = "MIS!!";

                _context.Add(gameLog);
                await _context.SaveChangesAsync();
                return View();
            }

            // Indien ongeldige poging maar beide parameters zijn wel meegegeven
            ViewBag.BombInfo = new List<string>
            {
                "Locatie: " + letter + " / " + number,
                "Resultaat: Ongeldige poging",
            };
            ViewBag.SunkenShips = "Aantal gezonken boten: " + GetNumberOfSunkenBoats(game) + "/" + game.Boats.Count;
            return View();
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
