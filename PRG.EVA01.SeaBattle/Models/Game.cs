using Microsoft.AspNetCore.Identity;

namespace PRG.EVA01.SeaBattle.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public DateTime StartedPlayingOn { get; set; }
        public virtual List<Boat> Boats { get; set; }
        public virtual List<GameLog> GameLogs { get; set; }
        //UserId is de foreignkey van AspNetUsers van de ApplicationUser (IdentityUser) klasse
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
