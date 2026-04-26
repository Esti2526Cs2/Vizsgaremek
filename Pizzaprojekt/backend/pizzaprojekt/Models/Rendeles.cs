namespace pizzaprojekt.Models
{
    //Anotálásokkal jelöljük az osztályokat és tulajdonságokat, hogy azok hogyan viszonyulnak az adatbázishoz.
    using System.ComponentModel.DataAnnotations; //Anotálásokkal jelöljük az osztályokat és tulajdonságokat, hogy azok hogyan viszonyulnak az adatbázishoz.
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore; //Az EF Core specifikus anotációk használatához
    [PrimaryKey(nameof(RendelesId))] //Összetett elsődleges kulcs definiálása a RendelesId és RendeloAzonosito mezőkből
    public class Rendeles //Rendelés osztály az adatbázisban
    {
        [Column("Rendelesi_id")] //Adatbázis oszlop neve
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RendelesId { get; set; } //Rendelés azonosító
        [Column("Mennyiseg")]
        public int Mennyiseg { get; set; } //Mennyiség
        [Column("Datum_ido")]
        public DateTime Datum { get; set; } //Dátum és idő
        [Column("Fizetesi_mod")]
        public string? FizetesMod { get; set; } //Fizetési mód
        [Column("Statusz")]
        public string? Statusz { get; set; } //Státusz

    }
}
