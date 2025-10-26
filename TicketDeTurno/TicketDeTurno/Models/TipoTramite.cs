using System.ComponentModel.DataAnnotations;

namespace TicketDeTurno.Web.Models
{
    public class TipoTramite
    {
        [Key]
        public int TipoTramiteId { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }
    }
}
