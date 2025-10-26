using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketDeTurno.Web.Models
{
    public class Alumno
    {
        [Key]
        [StringLength(18)]
        public string CURP { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Paterno { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Materno { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Correo { get; set; }

        // Relaciones
        public int NivelId { get; set; }
        public int MunicipioId { get; set; }

        [ForeignKey(nameof(NivelId))]
        public Nivel? Nivel { get; set; }

        [ForeignKey(nameof(MunicipioId))]
        public Municipio? Municipio { get; set; }

        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
