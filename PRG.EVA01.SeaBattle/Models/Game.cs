namespace PRG.EVA01.SeaBattle.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public DateTime StartedPlayingOn { get; set; }
        public virtual List<Boat> Boats { get; set; }
        public virtual List<GameLog> GameLogs { get; set; }
    }
}
