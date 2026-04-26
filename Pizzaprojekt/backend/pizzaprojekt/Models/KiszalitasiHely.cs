using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [PrimaryKey(nameof(CimId))]
    public class Kiszallitasi_Hely
    {
        [Column("Cim_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CimId { get; set; }
        [Column("Varos")]
        public string? Varos { get; set; }
        [Column("Iranyitoszam")]
        public string? Iranyitoszam { get; set; }
        public string? Utca { get; set; }
        [Column("Hazszam")]
        public string ? Hazszam { get; set; }
        [Column("Emelet_ajto")]
        public string? EmeletAjto { get; set; }
    }
}
