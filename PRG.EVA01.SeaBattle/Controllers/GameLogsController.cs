using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.GameLogs;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize]
    public class GameLogsController : Controller
    {
        private readonly SeaBattleDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameLogsController(SeaBattleDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameLogs
        public async Task<IActionResult> Index(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Administrator") && id != null)
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
            else if (userRoles.Contains("Administrator") && id == null)
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
            else if (userRoles.Contains("Player") && id != null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      join g in _context.Games on gl.GameId equals g.Id
                                      where gl.GameId == id && g.UserId == _userManager.GetUserId(User)
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
            else if (userRoles.Contains("Player") && id == null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      join g in _context.Games on gl.GameId equals g.Id
                                      where g.UserId == _userManager.GetUserId(User)
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
                return NotFound();
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
                .Where(a => a.Game.UserId == _userManager.GetUserId(User))
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

        [Authorize(Roles = "Administrator")]
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
            var vm = new DeleteViewModel
            {
                LogId = gameLog.Id,
                PlayerName = gameLog.PlayerName,
                LocationLetter = gameLog.LocationLetter,
                LocationNumber = gameLog.LocationNumber,
                Result = gameLog.Result
            };

            return View(vm);
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
