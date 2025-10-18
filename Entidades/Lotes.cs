using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("lotes")]
    public class Lotes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo")]
        [StringLength(6)]
        public required string Codigo { get; set; }

        [Column("fechaIngreso")]
        public DateOnly FechaIngreso { get; set; }

        [Column("fechaLote")]
        public DateOnly FechaLote { get; set; }

        [Column("fechaVencimiento")]
        public DateOnly FechaVencimiento { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Navigation properties
        public ICollection<OrdenCompraTipoGranoLote> OrdenCompraTipoGranoLotes { get; set; } = new List<OrdenCompraTipoGranoLote>();
        public ICollection<LotesTerminados> LotesTerminados { get; set; } = new List<LotesTerminados>();
        public ICollection<LoteEtapa> LoteEtapas { get; set; } = new List<LoteEtapa>();
    }
}
