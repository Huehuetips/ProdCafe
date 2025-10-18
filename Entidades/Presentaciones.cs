using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEjemplo.Models
{
    [Table("presentaciones")]
    public class Presentaciones
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo")]
        public required string Tipo { get; set; }

        // Navigation properties
        public ICollection<Productos> Productos { get; set; } = new List<Productos>();
    }
}
