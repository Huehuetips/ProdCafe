using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("proveedores")]
    public class Proveedores
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
    }
}