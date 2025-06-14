using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;

namespace PRG.EVA01.SeaBattle.Data;

public class SeaBattleDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<GameLog> GameLogs { get; set; }
    public DbSet<Boat> Boats { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Game> Games { get; set; }
    public SeaBattleDbContext(DbContextOptions<SeaBattleDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
