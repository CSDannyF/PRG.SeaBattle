using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services.Interfaces;
using PRG.EVA01.SeaBattle.ViewModels.SeaBattle;

namespace PRG.EVA01.SeaBattle.Services
{
    public class SeaBattleService : ISeaBattleService
    {
        private readonly SeaBattleDbContext _context;
        private string letters = "ABCDEF";
        private string numbers = "123456";

        public SeaBattleService(SeaBattleDbContext context)
        {
            _context = context;
        }

        public async Task<Game> GetGameAsync(int gameId)
        {
            // Include Boats voor genest object van Game anders is Boats null
            return await _context.Games
           .Include(g => g.Boats)
           .FirstOrDefaultAsync(m => m.Id == gameId);
        }

        public async Task<string> ThrowBombAsync(int gameId, string LocationLetter, string LocationNumber)
        {
            //ophalen van Game
            // Eager loading, voor geneste objecten
            // Als ik enkel de Game ophaal is Boats null en zo ook Location. Dus verplicht ik het Boats en daarna ook Locatie mee op te halen
            // https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager
            var game = _context.Games
                .Include(g => g.Boats)
                .ThenInclude(l => l.Location)
                .FirstOrDefault(m => m.Id == gameId);

            // GameLog object om later te op te slaan in database indien er een geldige poging is
            GameLog gameLog = new GameLog()
            {
                GameId = game.Id,
                PlayerName = game.PlayerName,
                LocationLetter = LocationLetter,
                LocationNumber = LocationNumber,
                CreatedOn = DateTime.Now
            };

            // Check als inkomende parameters binnen het bereik zijn
            if (letters.Contains(LocationLetter.ToUpper()) && numbers.Contains(LocationNumber))
            {
                // Loop door de lijst van boten, indien een boot geraakt word, wordt de ThrowBomb view aangeroepen
                foreach (Boat boat in game.Boats)
                {
                    // Indien een boot nog niet gezonken is kan hij geraakt worden
                    if (boat.Status == BoatStatus.Active)
                    {
                        // Check als de bomlocatie overeen komt met de bootlocatie indien true dan hit
                        if (boat.Location.Letter.Equals(LocationLetter.ToUpper()) && boat.Location.Number.Equals(LocationNumber))
                        {
                            boat.Status = BoatStatus.Sunk;
                            gameLog.Result = "HIT";
                            _context.Add(gameLog);
                            await _context.SaveChangesAsync();
                            return "HIT";
                        }
                    }
                }
                //Indien geen hit
                gameLog.Result = "MISS";
                _context.Add(gameLog);
                await _context.SaveChangesAsync();
                return "MISS";
            }
            // Indien ongeldige poging maar beide parameters zijn wel meegegeven
            return "OUT_OF_BOUNDS";
        }

        /// <summary>
        /// Geeft terug hoeveel boten gezonken zijn via Linq
        /// </summary>
        /// <returns>Aantal gezonken boten</returns>
        public int GetTotalSunkenBoats(List<Boat> boats)
        {
            return boats.Count(b => b.Status == BoatStatus.Sunk);
        }
    }
}
