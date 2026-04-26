using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace pizzaprojekt.Models
{
    [Table("lead")]
    [PrimaryKey(nameof(RendeloAzonosito), nameof(RendelesiId))]
    public class Lead
    {
        [Column("Rendelo_azonosito")]
        public int RendeloAzonosito { get; set; }   
        [Column("Rendelesi_Id")]
        public int RendelesiId { get; set; }         
        public Vasarlo? Vasarlo { get; set; }         
        public Rendeles? Rendeles { get; set; }     
    }
}
