using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("lotesTerminados")]
    public class LotesTerminados
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_Lote")]
        public int LoteId { get; set; }

        [Column("id_Producto")]
        public int ProductoId { get; set; }

        [Column("fechaEnvasado")]
        public DateOnly FechaEnvasado { get; set; }

        [Column("fechaVencimiento")]
        public DateOnly FechaVencimiento { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LoteId))]
        [JsonIgnore]
        public Lotes? Lote { get; set; }

        [ForeignKey(nameof(ProductoId))]
        [JsonIgnore]
        public Productos? Producto { get; set; }

        [JsonIgnore]
        public ICollection<Catacion> Cataciones { get; set; } = new List<Catacion>();
        
        [JsonIgnore]
        public ICollection<PedidoLoteTerminado> PedidoLoteTerminados { get; set; } = new List<PedidoLoteTerminado>();
    }
}
