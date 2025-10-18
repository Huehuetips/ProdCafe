using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public LotesTerminados LoteTerminado { get; set; } = null!;

        [ForeignKey(nameof(ProductoId))]
        public Productos Producto { get; set; } = null!;

        [ForeignKey(nameof(PedidoId))]
        public Pedidos Pedido { get; set; } = null!;
    }
}
