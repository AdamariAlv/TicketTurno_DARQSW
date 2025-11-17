using Microsoft.EntityFrameworkCore;
using System.Linq;
using TicketDeTurno.Models;
using TicketDeTurno.Web.Models;


namespace TicketDeTurno.Web.Data //SRP
{
    public class ApplicationDbContext : DbContext //singleton crea una unica instancia de la conexion a la base de datos
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Nivel> Niveles { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoTramite> TiposTramite { get; set; }
        public DbSet<VwSolicitudesDetalle> VistaSolicitudesDetalle { get; set; }
        public DbSet<VwAlumnosExtendido> VistaAlumnosExtendido { get; set; }
        public DbSet<VwTurnosPorDia> VistaTurnosPorDia { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Desactiva cascadas en todas las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<VwSolicitudesDetalle>()
                .ToView("vw_SolicitudesDetalle")
                .HasNoKey();

            modelBuilder.Entity<VwAlumnosExtendido>()
                .ToView("vw_AlumnosExtendido")
                .HasNoKey();

            modelBuilder.Entity<VwTurnosPorDia>()
                .ToView("vw_TurnosPorDia")
                .HasNoKey();
        }

    }


}
