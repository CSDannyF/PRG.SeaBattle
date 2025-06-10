using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Services.Interfaces
{
    public interface ISeaBattleService
    {
        Task<Game> GetGameAsync(int gameId);
        Task<string> ThrowBombAsync(int gameId, string LocationLetter, string LocationNumber);
        int GetTotalSunkenBoats(List<Boat> boats);
    }
}
