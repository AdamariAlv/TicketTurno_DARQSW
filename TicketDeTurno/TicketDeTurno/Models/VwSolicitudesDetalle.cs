namespace TicketDeTurno.Models
{
    public class VwSolicitudesDetalle
    {
        public Guid? SolicitudId { get; set; }
        public string? CURP { get; set; }
        public string? Alumno { get; set; }
        public string? Municipio { get; set; }
        public string? Nivel { get; set; }
        public string? Tramite { get; set; }
        public int? NumeroTurno { get; set; }
        public string? Estatus { get; set; }
        public DateTime? FechaAlta { get; set; }
    }

}
