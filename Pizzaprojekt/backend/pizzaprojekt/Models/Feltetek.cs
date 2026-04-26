using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [PrimaryKey(nameof(FeltetId))]
    public class Feltetek //Feltétek osztály az adatbázisban
    {
        [Column("Feltet_Id")]
        public int FeltetId { get; set; } //Feltétek azonosító
        [Column("Nev")]
        public string? Nev { get; set; } //Feltétek neve
        [Column("Feltet_ar")]
        public int Feltetar { get; set; } //Feltétek ára
    }
}
