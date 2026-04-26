using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [PrimaryKey(nameof(CimId), nameof(RendeloAzonosito))] // Összetett elsődleges kulcs definiálása a CimId és RendelesAzonosito mezőkből
    public class Keri
    {
        [Column("Cim_Id")]
        public int CimId { get; set; }               // Elsődleges kulcs
        [Column("Rendelo_Azonosito")]
        [ForeignKey("Vasarlo")]
        public int RendeloAzonosito { get; set; }   // Külső kulcs Lead-hez
        public Kiszallitasi_Hely? KiszalitasiHely { get; set; }
        public Vasarlo? Vasarlo { get; set; }              // Navigációs property
    }
}
