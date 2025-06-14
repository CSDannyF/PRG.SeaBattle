using Microsoft.AspNetCore.Identity;

namespace PRG.EVA01.SeaBattle.Models
{
    public class AppUser :IdentityUser 
    {
        public ICollection<Game> Games { get; set; }
    }
}
