using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.GameLogs;

namespace PRG.EVA01.SeaBattle.Services.Interfaces
{
    public interface IGameLogService
    {
        Task<List<IndexViewModel>> GetGameLogsAsync(ApplicationUser user, IList<string> userRoles,int? gameId);
        Task<DetailsViewModel> GetDetailsAsync(ApplicationUser user, IList<string> userRoles, int gameLogId);
        Task<DeleteViewModel> GetDeleteViewModelAsync(int gameLogId);
        Task DeleteGameLogAsync(int gameLogId);
    }
}
