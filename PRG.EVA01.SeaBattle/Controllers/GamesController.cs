using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly SeaBattleDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private int numberOfBoats = 6;
        private List<Boat> _boats = new List<Boat>();

        public GamesController(SeaBattleDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET: Games
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            // Als de ingelogde gebruiker niet de rol Administrator heeft krijgt hij enkel zijn eigen games te zien, anders alles games
            if (!userRoles.Contains("Administrator"))
            {
                return View(_context.Games.Where(a => a.UserId == _userManager.GetUserId(User)));
            }
            return View(await _context.Games.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // + Afscherming dat spelers via de url aan andere spelers hun games of data kunnen via where
            // Als de UserId niet gelinkt is met het GameId kan hij deze niet zien
            // (ook op andere plekken waar dit nodig is voorzien)
            var game = await _context.Games
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (userId == game.UserId || User.IsInRole("Administrator"))
            {
                return View(game);
            }
            return NotFound();
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
            var userId = _userManager.GetUserId(User);

            game.StartedPlayingOn = DateTime.Now;
            await LoadBoats(numberOfBoats);
            game.Boats = _boats;
            game.UserId = userId;

            // Ik krijg altijd ModelState invalid op deze omdat deze ModelState wordt gecontrolleerd bij de Post
            // Nu worden de objecten later bijgevoegd maar wordt de ModelState niet veranderd
            ModelState.Remove("Boats");
            ModelState.Remove("GameLogs");
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("ThrowBomb", "SeaBattle", new { id = game.Id });
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

            var userId = _userManager.GetUserId(User);
            if (userId == game.UserId || User.IsInRole("Administrator"))
            {
                return View(game);
            }
            return NotFound();
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
        [Authorize(Roles = "Administrator")]
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
            bool[] AreBoatsAdded = new bool[numberOfBoats];

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(@"https://mgp32-api.azurewebsites.net/");

            try
            {
                int i = numberOfBoats;
                do
                {
                    if (!AreBoatsAdded[0])
                    {
                        Task<HttpResponseMessage> responseTask1 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response1 = await responseTask1;
                        if (response1.IsSuccessStatusCode)
                        {
                            Location location = await response1.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[0] = AddBoat(location);
                            Console.WriteLine("boot 1 toegevoegd");
                        }
                    }
                    
                    if (!AreBoatsAdded[1])
                    {
                        Task<HttpResponseMessage> responseTask2 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response2 = await responseTask2;
                        if (response2.IsSuccessStatusCode)
                        {
                            Location location = await response2.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[1] = AddBoat(location);
                            Console.WriteLine("boot 2 toegevoegd");
                        }
                    }

                    if (!AreBoatsAdded[2])
                    {
                        Task<HttpResponseMessage> responseTask3 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response3 = await responseTask3;
                        if (response3.IsSuccessStatusCode)
                        {
                            Location location = await response3.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[2] = AddBoat(location);
                            Console.WriteLine("boot 3 toegevoegd");
                        }
                    }

                    if (!AreBoatsAdded[3])
                    {
                        Task<HttpResponseMessage> responseTask4 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response4 = await responseTask4;
                        if (response4.IsSuccessStatusCode)
                        {
                            Location location = await response4.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[3] = AddBoat(location);
                            Console.WriteLine("boot 4 toegevoegd");
                        }
                    }

                    if (!AreBoatsAdded[4])
                    {
                        Task<HttpResponseMessage> responseTask5 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response5 = await responseTask5;
                        if (response5.IsSuccessStatusCode)
                        {
                            Location location = await response5.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[4] = AddBoat(location);
                            Console.WriteLine("boot 5 toegevoegd");
                        }
                    }

                    if (!AreBoatsAdded[5])
                    {
                        Task<HttpResponseMessage> responseTask6 = httpClient.GetAsync("randomlocation/get/6");
                        HttpResponseMessage response6 = await responseTask6;
                        if (response6.IsSuccessStatusCode)
                        {
                            Location location = await response6.Content.ReadFromJsonAsync<Location>();
                            AreBoatsAdded[5] = AddBoat(location);
                            Console.WriteLine("boot 6 toegevoegd");
                        }
                    }
                } while (AreBoatsAdded.Contains(false));
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
