using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("pedido_ruta")]
    public class PedidoRuta
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_pedido")]
        public int PedidoId { get; set; }

        [Column("id_Ruta")]
        public int RutaId { get; set; }

        [Column("fechaSalida")]
        public DateOnly FechaSalida { get; set; }

        [Column("fechaEntrega")]
        public DateOnly? FechaEntrega { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PedidoId))]
        [JsonIgnore]
        public Pedidos? Pedido { get; set; }

        [ForeignKey(nameof(RutaId))]
        [JsonIgnore]
        public Rutas? Ruta { get; set; }
    }
}
