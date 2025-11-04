using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketDeTurno.Web.Models
{
    public class Solicitud //dto solicita estructura que devuelve un tipo texto plano json
    {
        [Key]
        public Guid SolicitudId { get; set; }

        [Required]
        [StringLength(18)]
        public string CURP { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string QuienRealiza { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Asunto { get; set; } = string.Empty;

        public int MunicipioId { get; set; }
        public int NivelId { get; set; }

        public int NumeroTurno { get; set; }

        [StringLength(15)]
        public string Estatus { get; set; } = "Pendiente";

        public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
        public DateTime? FechaAtencion { get; set; }

        [ForeignKey(nameof(CURP))]
        public Alumno? Alumno { get; set; }

        [ForeignKey(nameof(NivelId))]
        public Nivel? Nivel { get; set; }

        [ForeignKey(nameof(MunicipioId))]
        public Municipio? Municipio { get; set; }
        public int TipoTramiteId { get; set; }

        [ForeignKey(nameof(TipoTramiteId))]
        public TipoTramite TipoTramite { get; set; } //OCP

    }
}
