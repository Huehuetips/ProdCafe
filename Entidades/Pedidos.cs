using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("pedidos")]
    public class Pedidos
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_Cliente")]
        public int ClienteId { get; set; }

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        [Column("tipo")]
        public string? Tipo { get; set; }

        [Column("prioritaria")]
        public bool Prioritaria { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ClienteId))]
        public Clientes Cliente { get; set; } = null!;

        public ICollection<PedidoLoteTerminado> PedidoLoteTerminados { get; set; } = new List<PedidoLoteTerminado>();
        public ICollection<PedidoRuta> PedidoRutas { get; set; } = new List<PedidoRuta>();
    }
}
