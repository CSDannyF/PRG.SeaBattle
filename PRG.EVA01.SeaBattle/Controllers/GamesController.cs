using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class GamesController : Controller
    {
        private readonly SeaBattleDbContext _context;

        private int numberOfBoats = 6;
        private List<Boat> _boats = new List<Boat>();

        public GamesController(SeaBattleDbContext context)
        {
            _context = context;
        }

        //GET: Games
        public async Task<IActionResult> Index()
        {
            return View(await _context.Games.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlayerName,StartedPlayingOn")] Game game)
        {
            game.StartedPlayingOn = DateTime.Now;
            await LoadBoats(numberOfBoats);
            game.Boats = _boats;

            // Ik krijg altijd ModelState invalid op de boats
            ModelState.Remove("Boats");
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                ViewBag.SeaBattleInfo = new List<string>
                {
                    "Game id: " + game.Id,
                    "Speler: " + game.PlayerName,
                    "Gestart met spelen: " + game.StartedPlayingOn,
                    "Aantal gezonken botenbah: " + GetNumberOfSunkenBoats(game) + "/" + game.Boats.Count
                };
                return RedirectToAction(nameof(Index));
                //return RedirectToAction("ThrowBomb", new { id = game.Id });
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlayerName,StartedPlayingOn")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }

        /// <summary>
        /// Voegt x aantal random boten toe aan het spel
        /// </summary>
        /// <param name="numberOfBoats">Hoeveelheid boten</param>
        /// <returns></returns>
        public async Task LoadBoats(int numberOfBoats)
        {
            List<Boat> boats = new List<Boat>();

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"https://mgp32-api.azurewebsites.net/");

            try
            {
                int i = numberOfBoats;
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
        /// <summary>
        /// Voegt een boot toe indien de meegegeven locatie nog niet bezet is
        /// </summary>
        /// <param name="location">locatie van de boot</param>
        /// <returns>true als boot is toegevoegd</returns>
        private bool AddBoat(Location location)
        {
            bool occupied = _boats.Any(b => b.Location.Letter == location.Letter && b.Location.Number == location.Number);
            if (!occupied)
            {
                _boats.Add(new Boat { Location = location, Status = BoatStatus.Active });
                return true;
            }
            return false;
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
