using Microsoft.EntityFrameworkCore;

namespace PRG.EVA01.SeaBattle.Models
{
    public class SeaBattleDbContext : DbContext
    {
        public DbSet<GameLog> GameLogs { get; set; }
        public SeaBattleDbContext(DbContextOptions<SeaBattleDbContext> options) : base(options)
        {

        }
    }
}
