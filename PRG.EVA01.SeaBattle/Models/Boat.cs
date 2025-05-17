namespace PRG.EVA01.SeaBattle.Models
{
    public enum BoatStatus
    {
        Active,
        Sunk
    }
    public class Boat
    {
        public int Id { get; set; }
        public virtual Location Location { get; set; }
        public int LocationId { get; set; }
        public BoatStatus Status { get; set; }
        public virtual Game Game { get; set; }
        public int GameId { get; set; }
    }
}
