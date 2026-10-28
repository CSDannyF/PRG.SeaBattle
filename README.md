# SeaBattle

Een Battleship-webapplicatie, gebouwd als examenopdracht voor het vak
Programmeren (graduaat Programmeren, UCLL).

## Technologie
- C# / ASP.NET Core
- Entity Framework Core met migraties
- ASP.NET Core Identity (authenticatie en rollen)
- SQL Server

## Functionaliteit
- Registratie en login met gebruikersrollen
- Autogeneratie van boten via externe API call voorzien door de school
- Bommen gooien op boten met feedback "HIT", "MISS" of "OUT OF BOUNDS"
- Ophalen van loggingen over gespeelde spellen
## Opmerking
Oorspronkelijk ontwikkeld met een database op Azure. Die database is niet
meer beschikbaar, dus de applicatie draait lokaal pas na het aanmaken van
een eigen database (via de meegeleverde EF-migraties). De broncode geeft
een volledig beeld van de opzet en werking.
