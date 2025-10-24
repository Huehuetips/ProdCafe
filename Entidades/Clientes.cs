using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("clientes")]
    public class Clientes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo")]
        public string? Tipo { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        [Column("contacto")]
        public string? Contacto { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<Pedidos> Pedidos { get; set; } = new List<Pedidos>();
    }
}
