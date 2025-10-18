using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("ordenCompra_tipoGrano_lote")]
    public class OrdenCompraTipoGranoLote
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_OrdenCompra_TipoGrano")]
        public int OrdenCompraTipoGranoId { get; set; }

        [Column("id_Lote")]
        public int LoteId { get; set; }

        [Column("cantidadKg")]
        public int CantidadKg { get; set; }

        // Navigation properties
        [ForeignKey(nameof(OrdenCompraTipoGranoId))]
        public OrdenCompraTipoGrano OrdenCompraTipoGrano { get; set; } = null!;

        [ForeignKey(nameof(LoteId))]
        public Lotes Lote { get; set; } = null!;
    }
}
