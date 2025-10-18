using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ICollection<PedidoRuta> PedidoRutas { get; set; } = new List<PedidoRuta>();
    }
}
