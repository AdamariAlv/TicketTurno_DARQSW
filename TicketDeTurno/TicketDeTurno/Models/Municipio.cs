using System.ComponentModel.DataAnnotations;

namespace TicketDeTurno.Web.Models
{
    public class Municipio
    {
        [Key]
        public int MunicipioId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Alumno>? Alumnos { get; set; }
        public ICollection<Solicitud>? Solicitudes { get; set; }
    }
}
