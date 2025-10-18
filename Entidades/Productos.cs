using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("productos")]
    public class Productos
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        [Column("id_Presentacion")]
        public int PresentacionId { get; set; }

        [Column("nivelTostado")]
        public string? NivelTostado { get; set; }

        [Column("tipoMolido")]
        public string? TipoMolido { get; set; }

        [Column("precio")]
        public double Precio { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PresentacionId))]
        public Presentaciones Presentacion { get; set; } = null!;

        public ICollection<LotesTerminados> LotesTerminados { get; set; } = new List<LotesTerminados>();
        public ICollection<PedidoLoteTerminado> PedidoLoteTerminados { get; set; } = new List<PedidoLoteTerminado>();
    }
}
