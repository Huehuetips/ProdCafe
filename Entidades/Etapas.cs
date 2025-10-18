using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("etapas")]
    public class Etapas
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre(Tostado|Molienda|Empaque)")]
        public required string Nombre { get; set; }

        // Navigation properties
        public ICollection<LoteEtapa> LoteEtapas { get; set; } = new List<LoteEtapa>();
    }
}
