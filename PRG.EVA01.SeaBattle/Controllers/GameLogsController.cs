using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services.Interfaces;
using PRG.EVA01.SeaBattle.ViewModels.GameLogs;

namespace PRG.EVA01.SeaBattle.Controllers
{
    [Authorize]
    public class GameLogsController : Controller
    {
        private readonly IGameLogService _gameLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameLogsController(IGameLogService gameLogService, UserManager<ApplicationUser> userManager)
        {
            _gameLogService = gameLogService;
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
            var logs = await _gameLogService.GetGameLogsAsync(user, userRoles, id);
            return View(logs);
        }

        // GET: GameLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(user);
            var gameLog = await _gameLogService.GetDetailsAsync(user, userRoles, id.Value);
            
            if (gameLog == null)
            {
                return NotFound();
            }
            return View(gameLog);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(user);
            var gameLog = await _gameLogService.GetDeleteViewModelAsync(id.Value);

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
            await _gameLogService.DeleteGameLogAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
