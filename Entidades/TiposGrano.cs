using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("tiposGrano")]
    public class TiposGrano
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre(arábica|robusta|blends)")]
        public required string Nombre { get; set; }

        // Navigation properties
        public ICollection<OrdenCompraTipoGrano> OrdenCompraTipoGranos { get; set; } = new List<OrdenCompraTipoGrano>();
    }
}
