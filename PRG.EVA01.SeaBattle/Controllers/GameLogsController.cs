using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.GameLogs;

namespace PRG.EVA01.SeaBattle.Controllers
{
    public class GameLogsController : Controller
    {
        private readonly SeaBattleDbContext _context;

        public GameLogsController(SeaBattleDbContext context)
        {
            _context = context;
        }

        // GET: GameLogs
        public async Task<IActionResult> Index()
        {
            //var gameLogs = from ga in _context.Games
            //               join gl in _context.GameLogs on ga.Id equals gl.GameId
            //               join b in _context.Boats on ga.Id equals b.GameId
            //               join l in _context.Locations on b.LocationId equals l.Id
            //               //where b.Location.Letter == gl.LocationLetter && b.Location.Number == gl.LocationNumber
            //               select new IndexViewModel
            //               {
            //                   GameId = ga.Id,
            //                   LocationId = l.Id,
            //                   LocationLetter = l.Letter,
            //                   LocationNumber = l.Number,
            //                   Status = b.Status
            //               };
            //return View(await gameLogs.ToListAsync());
            return View(await _context.GameLogs.ToListAsync());
        }

        // GET: GameLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLog = await _context.GameLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameLog == null)
            {
                return NotFound();
            }

            return View(gameLog);
        }

        // GET: GameLogs/GameId
        public async Task<IActionResult> GameDetails(int? id)
        {
            var seaBattleDbContext = _context.GameLogs
                .Where(w => w.GameId == id);
            return View("Index", await seaBattleDbContext.ToListAsync());
        }

        // GET: GameLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLog = await _context.GameLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameLog == null)
            {
                return NotFound();
            }
            return View(gameLog);
        }

        // POST: GameLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameLog = await _context.GameLogs.FindAsync(id);
            if (gameLog != null)
            {
                _context.GameLogs.Remove(gameLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameLogExists(int id)
        {
            return _context.GameLogs.Any(e => e.Id == id);
        }
    }
}
