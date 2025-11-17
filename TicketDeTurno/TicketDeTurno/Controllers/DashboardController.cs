using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TicketDeTurno.Web.Data;
using TicketDeTurno.Web.Models;

namespace TuProyecto.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Cargar filtros en los dropdowns
            ViewBag.Municipios = await _context.Municipios
                .OrderBy(m => m.Nombre).ToListAsync();

            ViewBag.Niveles = await _context.Niveles
                .OrderBy(n => n.Nombre).ToListAsync();

            ViewBag.TiposTramite = await _context.TiposTramite
                .OrderBy(t => t.Nombre).ToListAsync();

            ViewBag.Solicitudes = await _context.Solicitudes
                .OrderBy(s => s.Estatus).Select(s => s.Estatus).Distinct().ToListAsync();

            // datos de las vistas SQL
            ViewBag.VistaDetalle = await _context.VistaSolicitudesDetalle.ToListAsync();
            ViewBag.VistaAlumnos = await _context.VistaAlumnosExtendido.ToListAsync();
            ViewBag.VistaTurnosDia = await _context.VistaTurnosPorDia.ToListAsync();

            return View();
        }

        [HttpGet]
        public JsonResult ObtenerDatos(string municipio, string nivel, string solicitud, string tipoTramite)
        {
            var query = _context.Solicitudes
                .Include(s => s.Municipio)
                .Include(s => s.Nivel)
                .Include(s => s.TipoTramite)
                .AsQueryable();

            if (!string.IsNullOrEmpty(municipio))
                query = query.Where(s => s.Municipio.Nombre == municipio);

            if (!string.IsNullOrEmpty(nivel))
                query = query.Where(s => s.Nivel.Nombre == nivel);

            if (!string.IsNullOrEmpty(solicitud))
                query = query.Where(s => s.Estatus == solicitud);

            if (!string.IsNullOrEmpty(tipoTramite))
                query = query.Where(s => s.TipoTramite.Nombre == tipoTramite);

            var porMunicipio = query
                .Where(s => s.Municipio != null)
                .GroupBy(s => s.Municipio.Nombre)
                .Select(g => new { Label = g.Key, Valor = g.Count() })
                .ToList();

            var porNivel = query
                .Where(s => s.Nivel != null)
                .GroupBy(s => s.Nivel.Nombre)
                .Select(g => new { Label = g.Key, Valor = g.Count() })
                .ToList();

            var porTramite = query
                .Where(s => s.TipoTramite != null)
                .GroupBy(s => s.TipoTramite.Nombre)
                .Select(g => new { Label = g.Key, Valor = g.Count() })
                .ToList();

            var total = query.Count();

            return Json(new
            {
                total,
                municipio = porMunicipio,
                nivel = porNivel,
                tramite = porTramite
            });


        }
        public IActionResult TurnosMunicipio(int id)
        {
            var turnos = _context.Solicitudes
                .FromSqlInterpolated($"EXEC sp_ObtenerTurnosPorMunicipio {id}")
                .ToList();

            return View(turnos);
        }

        public IActionResult ObtenerTurnosSP(int municipioId)
        {
            var resultado = _context.Solicitudes
                .FromSqlInterpolated($"EXEC sp_ObtenerTurnosPorMunicipio {municipioId}")
                .ToList();

            return Json(resultado);
        }
    }
}
