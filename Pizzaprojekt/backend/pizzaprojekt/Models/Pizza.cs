using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [PrimaryKey(nameof(PizzaId))]
    public class Pizza //Pizza osztály az adatbázisban
    {
        [Column("Pizza_Id")]
        public int PizzaId { get; set; } //Pizza azonosítóa
        [Column("Pizza_ar")]
        public int pizzaar { get; set; } //Pizza ára
        [Column("Nev")]
        public string? Nev { get; set; } //Pizza neve
        public string? alap { get; set; } //Pizza alap típusa
        [Column("image")]
        public string? Image { get; set; } //Pizza képe
    }
}
