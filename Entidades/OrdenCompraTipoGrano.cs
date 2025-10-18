using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("ordenCompra_tipoGrano")]
    public class OrdenCompraTipoGrano
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_OrdenCompra")]
        public int OrdenCompraId { get; set; }

        [Column("id_TipoGrano")]
        public int TipoGranoId { get; set; }

        [Column("cantidadKg")]
        public int CantidadKg { get; set; }

        [Column("precioUnitarioKg")]
        public double PrecioUnitarioKg { get; set; }

        [Column("precioTotal")]
        public double PrecioTotal { get; set; }

        // Navigation properties
        [ForeignKey(nameof(OrdenCompraId))]
        public OrdenesCompra OrdenCompra { get; set; } = null!;

        [ForeignKey(nameof(TipoGranoId))]
        public TiposGrano TipoGrano { get; set; } = null!;

        public ICollection<OrdenCompraTipoGranoLote> OrdenCompraTipoGranoLotes { get; set; } = new List<OrdenCompraTipoGranoLote>();
    }
}
