using Microsoft.EntityFrameworkCore;

namespace PRG.EVA01.SeaBattle.Models
{
    public class SeaBattleDbContext : DbContext
    {
        public DbSet<GameLog> GameLogs { get; set; }
        public DbSet<Boat> Boats { get; set; }
        public DbSet<Location> Locations { get; set; } 
        public DbSet<Game> Games { get; set; }
        public SeaBattleDbContext(DbContextOptions<SeaBattleDbContext> options) : base(options)
        {

        }
    }
}
