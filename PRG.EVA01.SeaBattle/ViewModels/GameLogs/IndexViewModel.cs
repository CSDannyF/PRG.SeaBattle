using PRG.EVA01.SeaBattle.Models;
using System.ComponentModel.DataAnnotations;

namespace PRG.EVA01.SeaBattle.ViewModels.GameLogs
{
    public class IndexViewModel
    {
        public int LogId { get; set; }
        public int GameId { get; set; }
        public string LocationLetter { get; set; }
        public string LocationNumber { get; set; }
        public string LocationInfo { get { return LocationLetter + "/" + LocationNumber; } }
        public string Result { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
