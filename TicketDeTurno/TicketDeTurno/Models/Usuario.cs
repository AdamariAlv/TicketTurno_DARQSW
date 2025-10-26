using System.ComponentModel.DataAnnotations;

namespace TicketDeTurno.Web.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)]
        public string Rol { get; set; } = "ADMIN";

        public bool Activo { get; set; } = true;
    }
}
