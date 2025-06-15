using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.ViewModels.SeaBattle;

namespace PRG.EVA01.SeaBattle.Services.Interfaces
{
    public interface ISeaBattleService
    {
        Task<ThrowBombViewModel> GetGameAsync(int gameId);
        Task<ThrowBombViewModel> ThrowBombAsync(int gameId, string LocationLetter, string LocationNumber);
    }
}
