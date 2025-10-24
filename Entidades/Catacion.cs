using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("catacion")]
    public class Catacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_LoteTerminado")]
        public int LoteTerminadoId { get; set; }

        [Column("puntaje")]
        public double Puntaje { get; set; }

        [Column("humedad")]
        public double Humedad { get; set; }

        [Column("notas")]
        public string? Notas { get; set; }

        [Column("aprobado")]
        public bool Aprobado { get; set; }

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LoteTerminadoId))]
        [JsonIgnore]
        public LotesTerminados? LoteTerminado { get; set; }
    }
}
