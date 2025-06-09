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
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                    where gl.GameId == id
                    select new IndexViewModel
                    {
                        LogId = gl.Id,
                        GameId = gl.GameId,
                        LocationLetter = gl.LocationLetter,
                        LocationNumber = gl.LocationNumber,
                        Result = gl.Result,
                        CreatedOn = gl.CreatedOn
                    }).ToListAsync();
                return View(gameLogs);
            } 
            else
            {
                var gameLogs = await (from gl in _context.GameLogs
                    select new IndexViewModel
                    {
                        LogId = gl.Id,
                        GameId = gl.GameId,
                        LocationLetter = gl.LocationLetter,
                        LocationNumber = gl.LocationNumber,
                        Result = gl.Result,
                        CreatedOn = gl.CreatedOn
                    }).ToListAsync();
                return View(gameLogs);
            }
            
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

            var vm = new DetailsViewModel
            {
                GameId = gameLog.GameId,
                PlayerName = gameLog.PlayerName,
                LocationLetter = gameLog.LocationLetter,
                LocationNumber = gameLog.LocationNumber,
                Result = gameLog.Result,
                CreatedOn = gameLog.CreatedOn
            };

            return View(vm);
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
