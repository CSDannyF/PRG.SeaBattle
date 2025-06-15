using System.ComponentModel.DataAnnotations;

namespace PRG.EVA01.SeaBattle.ViewModels.GameLogs
{
    public class DetailsViewModel
    {
        public int GameId { get; set; }
        [Display(Name = "Player")]
        public string PlayerName { get; set; }
        public string LocationLetter { get; set; }
        public string LocationNumber { get; set; }
        [Display(Name = "Throw location")]
        public string LocationInfo { get { return LocationLetter + "/" + LocationNumber; } }
        [Display(Name = "Throw result")]
        public string Result { get; set; }
        [Display(Name = "Created on")]
        public DateTime CreatedOn { get; set; }
    }
}
