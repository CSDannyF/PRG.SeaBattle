using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Data;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services.Interfaces;
using PRG.EVA01.SeaBattle.ViewModels.GameLogs;

namespace PRG.EVA01.SeaBattle.Services
{
    public class GameLogService : IGameLogService
    {
        private readonly SeaBattleDbContext _context;

        public GameLogService(SeaBattleDbContext context)
        {
            _context = context;
        }

        public async Task<List<IndexViewModel>> GetGameLogsAsync(ApplicationUser user, IList<string> userRoles, int? gameId)
        {
            var userId = user.Id;
            if (userRoles.Contains("Administrator") && gameId != null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      where gl.GameId == gameId
                                      select new IndexViewModel
                                      {
                                          LogId = gl.Id,
                                          GameId = gl.GameId,
                                          PlayerName = gl.PlayerName,
                                          LocationLetter = gl.LocationLetter,
                                          LocationNumber = gl.LocationNumber,
                                          Result = gl.Result,
                                          CreatedOn = gl.CreatedOn
                                      }).ToListAsync();
                return gameLogs;
            }
            else if (userRoles.Contains("Administrator"))
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      select new IndexViewModel
                                      {
                                          LogId = gl.Id,
                                          GameId = gl.GameId,
                                          PlayerName = gl.PlayerName,
                                          LocationLetter = gl.LocationLetter,
                                          LocationNumber = gl.LocationNumber,
                                          Result = gl.Result,
                                          CreatedOn = gl.CreatedOn
                                      }).ToListAsync();
                return gameLogs;
            }
            else if (userRoles.Contains("Player") && gameId != null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      join g in _context.Games on gl.GameId equals g.Id
                                      where gl.GameId == gameId && g.UserId == userId
                                      select new IndexViewModel
                                      {
                                          LogId = gl.Id,
                                          GameId = gl.GameId,
                                          PlayerName = gl.PlayerName,
                                          LocationLetter = gl.LocationLetter,
                                          LocationNumber = gl.LocationNumber,
                                          Result = gl.Result,
                                          CreatedOn = gl.CreatedOn
                                      }).ToListAsync();
                return gameLogs; 
            }
            else if (userRoles.Contains("Player") && gameId == null)
            {
                var gameLogs = await (from gl in _context.GameLogs
                                      join g in _context.Games on gl.GameId equals g.Id
                                      where g.UserId == userId
                                      select new IndexViewModel
                                      {
                                          LogId = gl.Id,
                                          GameId = gl.GameId,
                                          PlayerName = gl.PlayerName,
                                          LocationLetter = gl.LocationLetter,
                                          LocationNumber = gl.LocationNumber,
                                          Result = gl.Result,
                                          CreatedOn = gl.CreatedOn
                                      }).ToListAsync();
                return gameLogs;
            }
            return new List<IndexViewModel>();
        }

        public async Task<DetailsViewModel> GetDetailsAsync(ApplicationUser user, IList<string> userRoles, int gameLogId)
        {
            if (gameLogId == null)
            {
                return null;
            }

            var gameLog = await _context.GameLogs
                .Include(g => g.Game)
                .FirstOrDefaultAsync(m => m.Id == gameLogId);

            if (gameLog == null)
            {
                return null;
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

            if (userRoles.Contains("Administrator") || gameLog.Game.UserId == user.Id)
            {
                return vm;
            }
            return null;                
        }

        public async Task<DeleteViewModel> GetDeleteViewModelAsync(int gameLogId)
        {
            var gameLog = await _context.GameLogs
                .FirstOrDefaultAsync(m => m.Id == gameLogId);

            if (gameLog == null)
            {
                return null;
            }

            var vm = new DeleteViewModel
            {
                LogId = gameLog.Id,
                PlayerName = gameLog.PlayerName,
                LocationLetter = gameLog.LocationLetter,
                LocationNumber = gameLog.LocationNumber,
                Result = gameLog.Result
            };
            return vm;
        }

        public async Task DeleteGameLogAsync(int gameLogId)
        {
            var gameLog = await _context.GameLogs.FindAsync(gameLogId);

            if (gameLog != null)
            {
                _context.GameLogs.Remove(gameLog);
            }
            await _context.SaveChangesAsync();
        }
    }
}
