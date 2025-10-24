using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("pedido_loteTerminado")]
    public class PedidoLoteTerminado
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_LoteTerminado")]
        public int LoteTerminadoId { get; set; }

        [Column("id_Producto")]
        public int ProductoId { get; set; }

        [Column("id_Pedido")]
        public int PedidoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LoteTerminadoId))]
        [JsonIgnore]
        public LotesTerminados? LoteTerminado { get; set; }

        [ForeignKey(nameof(ProductoId))]
        [JsonIgnore]
        public Productos? Producto { get; set; }

        [ForeignKey(nameof(PedidoId))]
        [JsonIgnore]
        public Pedidos? Pedido { get; set; }
    }
}
