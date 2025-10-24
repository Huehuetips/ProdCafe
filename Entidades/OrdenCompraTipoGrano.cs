using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public OrdenesCompra? OrdenCompra { get; set; }

        [ForeignKey(nameof(TipoGranoId))]
        [JsonIgnore]
        public TiposGrano? TipoGrano { get; set; }

        [JsonIgnore]
        public ICollection<OrdenCompraTipoGranoLote> OrdenCompraTipoGranoLotes { get; set; } = new List<OrdenCompraTipoGranoLote>();
    }
}
