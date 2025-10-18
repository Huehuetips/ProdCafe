using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ICollection<OrdenesCompra> OrdenesCompras { get; set; } = new List<OrdenesCompra>();
    }
}