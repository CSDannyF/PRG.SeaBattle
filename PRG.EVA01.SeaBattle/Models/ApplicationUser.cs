using Microsoft.AspNetCore.Identity;

namespace PRG.EVA01.SeaBattle.Models
{
    //https://www.reddit.com/r/dotnet/comments/14y6whe/customizing_identity_user/
    //https://stackoverflow.com/questions/62695894/asp-net-identityuser-add-custom-list-property
    //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-9.0#custom-user-data
    //Extra klasse voorzien om een gebruiker meerdere games te laten spelen. IdentityUser kan enkel "aangepast" worden indien we ervan overervan
    //We ervan ervan over en behouden alle functionaliteiten van deze klasse en voegen een lijst van games toe. AspNetUsers heeft nu een één op veel relatie met games
    //Op elke plek waar we normaal IdentityUser aanroepen moet nu ApplicationUser aangeroepen worden.
    public class ApplicationUser :IdentityUser 
    {
        public ICollection<Game> Games { get; set; }
    }
}
