using Microsoft.EntityFrameworkCore;
using TicketDeTurno.Web.Models;
using System.Linq;


namespace TicketDeTurno.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Nivel> Niveles { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Desactiva cascadas en todas las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }


}
