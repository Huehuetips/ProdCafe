using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiEjemplo.Models
{
    [Table("lote_etapa")]
    public class LoteEtapa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_Lote")]
        public int LoteId { get; set; }

        [Column("id_Etapa")]
        public int EtapaId { get; set; }

        [Column("fechaInicio")]
        public DateOnly FechaInicio { get; set; }

        [Column("fechaFin")]
        public DateOnly? FechaFin { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LoteId))]
        [JsonIgnore]
        public Lotes? Lote { get; set; }

        [ForeignKey(nameof(EtapaId))]
        [JsonIgnore]
        public Etapas? Etapa { get; set; }
    }
}
