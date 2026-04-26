namespace pizzaprojekt.Models;

//Anotálásokkal jelöljük az osztályokat és tulajdonságokat, hogy azok hogyan viszonyulnak az adatbázishoz.
using System.ComponentModel.DataAnnotations; //Anotálásokkal jelöljük az osztályokat és tulajdonságokat, hogy azok hogyan viszonyuljanak az adatbázishoz.
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore; //Az EF Core specifikus anotációk használatához
[PrimaryKey(nameof(RendelesId), nameof(PizzaId))] //Összetett elsődleges kulcs definiálása a RendelesId és PizzaId mezőkből
public class Tartalmaz //Kapcsoló tábla a Rendeles és Pizza között (many-to-many kapcsolat)
{
    [Column("Rendelesi_Id")]
    public int RendelesId { get; set; } //Rendelés azonosító
    [ForeignKey(nameof(RendelesId))]
    public Rendeles? Rendeles { get; set; } //Navigációs tulajdonság a Rendeles entitáshoz  

    [Column("Pizza_Id")]
    public int PizzaId { get; set; } //Pizza azonosító
    [ForeignKey(nameof(PizzaId))]
    public Pizza? Pizza { get; set; } //Navigációs tulajdonság a Pizza entitáshoz
}
