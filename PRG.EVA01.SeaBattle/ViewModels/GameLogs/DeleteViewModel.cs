using System.ComponentModel.DataAnnotations;

namespace PRG.EVA01.SeaBattle.ViewModels.GameLogs
{
    public class DeleteViewModel
    {
        public int LogId { get; set; }
        public string PlayerName { get; set; }
        public string LocationLetter { get; set; }
        public string LocationNumber { get; set; }
        [Display(Name = "Location info")]
        public string LocationInfo { get { return LocationLetter + "/" + LocationNumber; } }
        public string Result { get; set; }
    }
}
