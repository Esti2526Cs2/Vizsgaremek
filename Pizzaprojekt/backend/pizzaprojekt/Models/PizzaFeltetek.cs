namespace pizzaprojekt.Models
{
    using Microsoft.EntityFrameworkCore; //Az EF Core specifikus anotációk használatához
    //Kapcsoló tábla a Pizza és Feltétek között (many-to-many kapcsolat)
    using System.ComponentModel.DataAnnotations; //Anotálásokkal jelöljük az osztályokat és tulajdonságokat, hogy azok hogyan viszonyulnak az adatbázishoz.
    using System.ComponentModel.DataAnnotations.Schema;
    [PrimaryKey(nameof(PizzaId), nameof(FeltetId))] //Összetett elsődleges kulcs definiálása a pizzaId és FeltetekId mezőkből
    public class Pizza_Feltetek //Kapcsoló tábla a Pizza és Feltétek között (many-to-many kapcsolat)
    {
        [Column("Pizza_Id")]
        public int PizzaId { get; set; } //Pizza azonosító
        public Pizza? pizza { get; set; } //Navigációs tulajdonság a Pizza entitáshoz
        [Column("Feltet_Id")]
        public int FeltetId { get; set; } //Feltétek azonosító
        [Column("Feltet")]
        public Feltetek? feltet { get; set; } //Navigációs tulajdonság a Feltetek entitáshoz
    }
}
