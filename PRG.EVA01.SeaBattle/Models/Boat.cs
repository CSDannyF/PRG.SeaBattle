namespace PRG.EVA01.SeaBattle.Models
{
    public enum BoatStatus
    {
        Active,
        Sunk
    }
    public class Boat
    {
        public Location Location { get; set; }
        public BoatStatus Status { get; set; }
    }
}
