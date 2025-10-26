using System.ComponentModel.DataAnnotations;

namespace TicketDeTurno.Web.Models
{
    public class Nivel
    {
        [Key]
        public int NivelId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Alumno>? Alumnos { get; set; }
    }
}
