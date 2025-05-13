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
        private static bool _boatsLoaded = false;
        private static Game _game = new Game
        {
            Id = 1,
            PlayerName = "Player1",
            StartedPlayingOn = DateTime.Now,
            Boats = new List<Boat>
            {
                new Boat { Location = new Location { Letter = "A", Number = "2" }, Status = BoatStatus.Active },
                new Boat { Location = new Location { Letter = "B", Number = "4" }, Status = BoatStatus.Active },
                new Boat { Location = new Location { Letter = "C", Number = "6" }, Status = BoatStatus.Active }
            }
        };

        // Hoort dit zo? controller wordt elke keer aangeroepen. In een static constructor creëer ik een deadlock ofzoiets
        public SeaBattleController(SeaBattleDbContext context)
        {
            _context = context;
            if (!_boatsLoaded)
            {
                LoadBoats().Wait();
                _boatsLoaded = true;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gooit een bom via de url en zal de nodige spelinformatie doorgeven aan de view
        /// </summary>
        /// <param name="letter">Bereik van A t.e.m. G</param>
        /// <param name="number">Bereik van 1 t.e.m. 6param>
        /// <returns>ThrowBomb view</returns>
        public async Task<IActionResult> ThrowBomb(string letter, string number)
        {

            // GameLog object om later te op te slaan in database indien er een geldige poging is
            GameLog gameLog = new GameLog()
            {
                GameId = _game.Id,
                PlayerName = _game.PlayerName,
                LocationLetter = letter,
                LocationNumber = number,
                
                CreatedOn = DateTime.Now
            };
            
            // Algemene info over het spel
            ViewBag.SeaBattleInfo = new List<string>
            {
                "Game id: " + _game.Id,
                "Speler: " + _game.PlayerName,
                "Gestart met spelen: " + _game.StartedPlayingOn
            };

            //Geeft ongeldige poging terug bij verkeerde url
            if (letter == null || number == null)
            {
                var routeData = RouteData.Values;
                ViewBag.BombInfo = new List<string>
                {
                    "Locatie: "  + " / ",
                    "Resultaat: Ongeldige poging",
                    "Aantal gezonken boten: " + GetNumberOfSunkenBoats()
                };
                return View();
            }

            // Check als inkomende parameters binnen het bereik zijn
            if (letters.Contains(letter.ToUpper()) && numbers.Contains(number))
            {
                // Loop door de lijst van boten, indien een boot geraakt word, wordt de ThrowBomb view aangeroepen
                foreach (Boat boat in _game.Boats)
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
                                "Locatie: " + RouteData.Values["letter"] + " / " + RouteData.Values["number"],
                                "Resultaat: HIT!!!",
                                "Aantal gezonken boten: " + GetNumberOfSunkenBoats()
                            };
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
                    "Locatie: " + RouteData.Values["letter"] + " / " + RouteData.Values["number"],
                    "Resultaat: MIS!!",
                    "Aantal gezonken boten: " + GetNumberOfSunkenBoats()
                };
                gameLog.Result = "MIS!!";
                _context.Add(gameLog);
                await _context.SaveChangesAsync();
                return View();
            }

            // Indien ongeldige poging maar beide parameters zijn wel meegegeven
            ViewBag.BombInfo = new List<string>
            {
                "Locatie: " + RouteData.Values["letter"] + " / " + RouteData.Values["number"],
                "Resultaat: Ongeldige poging",
                "Aantal gezonken boten: " + GetNumberOfSunkenBoats()
            };
            return View();
        }

        /// <summary>
        /// Voegt 3 random boten toe aan het spel
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBoats() 
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"https://mgp32-api.azurewebsites.net/");

            try
            {
                int i = 3;
                do
                {
                    Task<HttpResponseMessage> responseTask1 = httpClient.GetAsync("randomlocation/get/6");
                    HttpResponseMessage response1 = await responseTask1;

                    if (response1.IsSuccessStatusCode)
                    {
                        Location location = await response1.Content.ReadFromJsonAsync<Location>();
                        if (AddBoat(location))
                        {
                            i--;
                        }
                    }
                } while (i > 0);
            }
            // catch anders implementeren?
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
        }

        /*
            https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any?view=net-9.0
            https://stackoverflow.com/questions/879391/linq-any-vs-exists-whats-the-difference
            Check indien een er al een boot op de gegeven locatie staat (Any checked voorwaarde, Exist checked al
        */
        // TODO async maken?
        /// <summary>
        /// Voegt een boot toe indien de meegegeven locatie nog niet bezet is
        /// </summary>
        /// <param name="location">locatie van de boot</param>
        /// <returns>true als boot is toegevoegd</returns>
        private static bool AddBoat(Location location)
        {

            bool occupied = _game.Boats.Any(b => b.Location.Letter == location.Letter && b.Location.Number == location.Number);
            if (!occupied)
            {
                _game.Boats.Add(new Boat { Location = location, Status = BoatStatus.Active });
                return true;
            }
            return false;
        }

        /// <summary>
        /// Geeft terug hoeveel boten gezonken zijn
        /// </summary>
        /// <returns>Aantal gezonken boten</returns>
        private int GetNumberOfSunkenBoats()
        {
            int NumberOfSunkenBoats = 0;
            foreach(Boat boat in _game.Boats)
            {
                if (boat.Status == BoatStatus.Sunk)
                {
                    NumberOfSunkenBoats++;
                }
            }
            return NumberOfSunkenBoats;
        }

    }
}
