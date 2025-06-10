using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PRG.EVA01.SeaBattle.ViewModels.SeaBattle
{
    public class ThrowBombViewModel
    {
        public int GameId { get; set; }
        public string PlayerName { get; set; }
        [Required]
        [Display(Name = "Letter")]
        public string LocationLetter { get; set; }
        [Required]
        [Display(Name = "Number")]
        public string LocationNumber { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [Display(Name = "Throw Result")]
        public string Result { get; set; }
        public string TotalSunkenBoats { get; set; }
    }
}
