using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("rutas")]
    public class Rutas
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo")]
        public string? Tipo { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        [Column("zona")]
        public string? Zona { get; set; }

        [Column("tiempoEstimadoH")]
        public double TiempoEstimadoH { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<PedidoRuta> PedidoRutas { get; set; } = new List<PedidoRuta>();
    }
}
