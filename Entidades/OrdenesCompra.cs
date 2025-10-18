using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("ordenesCompra")]
    public class OrdenesCompra
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_proveedor")]
        public int ProveedorId { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("fechaEmision")]
        public DateOnly FechaEmision { get; set; }

        [Column("fechaRecepcion")]
        public DateOnly? FechaRecepcion { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ProveedorId))]
        public Proveedores Proveedor { get; set; } = null!;

        public ICollection<OrdenCompraTipoGrano> OrdenCompraTipoGranos { get; set; } = new List<OrdenCompraTipoGrano>();
    }
}
