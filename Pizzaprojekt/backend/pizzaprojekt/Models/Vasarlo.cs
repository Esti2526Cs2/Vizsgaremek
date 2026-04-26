using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [PrimaryKey(nameof(RendeloAzonosito))]
    public class Vasarlo
    {
        [Column("Rendelo_azonosito")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RendeloAzonosito { get; set; }
        [Column("Nev")]
        [Required] public string? Nev { get; set; }
        [Column("Telefonszam")]
        public string? Telefonszam { get; set; }
    }
}
