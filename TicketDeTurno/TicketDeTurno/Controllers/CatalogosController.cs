using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketDeTurno.Web.Data;
using TicketDeTurno.Web.Filters;
using TicketDeTurno.Web.Models;

namespace TicketDeTurno.Controllers
    {
        [AuthFilter]
        public class CatalogosController : Controller
        {
            private readonly ApplicationDbContext _context;//singleton DIP

            public CatalogosController(ApplicationDbContext context)
            {
                _context = context; //constructor dependency injection  
        }

        // CRUD Municipios
        public IActionResult Municipios()
            {
                var municipios = _context.Municipios.ToList();//repositorio para obtener todos los municipios  
            return View(municipios);
            }

            [HttpPost]
            public IActionResult CrearMunicipio(string nombre)
            {
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    _context.Municipios.Add(new Municipio { Nombre = nombre });
                    _context.SaveChanges();
                }
                return RedirectToAction("Municipios");
            }

            [HttpPost]
            public IActionResult EditarMunicipio(int id, string nombre)
            {
                var municipio = _context.Municipios.Find(id);
                if (municipio != null && !string.IsNullOrWhiteSpace(nombre))
                {
                    municipio.Nombre = nombre;
                    _context.SaveChanges();
                }
                return RedirectToAction("Municipios");
            }

            [HttpPost]
            public IActionResult EliminarMunicipio(int id)
            {
                var municipio = _context.Municipios.Find(id);
                if (municipio != null)
                {
                    _context.Municipios.Remove(municipio);
                    _context.SaveChanges();
                }
                return RedirectToAction("Municipios");
            }


        // CRUD Tipos de Trámite
        public IActionResult TiposTramite()
        {
            var tipos = _context.TiposTramite.ToList();
            return View(tipos);
        }

        [HttpPost]
        public IActionResult CrearTipoTramite(string nombre)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.TiposTramite.Add(new TipoTramite { Nombre = nombre });
                _context.SaveChanges();
            }
            return RedirectToAction("TiposTramite");
        }

        [HttpPost]
        public IActionResult EditarTipoTramite(int id, string nombre)
        {
            var tipo = _context.TiposTramite.Find(id);
            if (tipo != null && !string.IsNullOrWhiteSpace(nombre))
            {
                tipo.Nombre = nombre;
                _context.SaveChanges();
            }
            return RedirectToAction("TiposTramite");
        }

        [HttpPost]
        public IActionResult EliminarTipoTramite(int id)
        {
            var tipo = _context.TiposTramite.Find(id);
            if (tipo != null)
            {
                _context.TiposTramite.Remove(tipo);
                _context.SaveChanges();
            }
            return RedirectToAction("TiposTramite");
        }


        //crud niveles
        public IActionResult Niveles()
        {
            var niveles = _context.Niveles.ToList();
            return View(niveles);
        }

        [HttpPost]
        public IActionResult CrearNivel(string nombre)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Niveles.Add(new Nivel { Nombre = nombre });
                _context.SaveChanges();
            }
            return RedirectToAction("Niveles");
        }

        [HttpPost]
        public IActionResult EditarNivel(int id, string nombre)
        {
            var nivel = _context.Niveles.Find(id);
            if (nivel != null && !string.IsNullOrWhiteSpace(nombre))
            {
                nivel.Nombre = nombre;
                _context.SaveChanges();
            }
            return RedirectToAction("Niveles");
        }

        [HttpPost]
        public IActionResult EliminarNivel(int id)
        {
            var nivel = _context.Niveles.Find(id);
            if (nivel != null)
            {
                _context.Niveles.Remove(nivel);
                _context.SaveChanges();
            }
            return RedirectToAction("Niveles");
        }
        //crud solicitudes (para admin)
        public IActionResult Solicitudes(string busqueda)
        {
            var solicitudes = _context.Solicitudes
                .Include(s => s.Alumno) // Para poder mostrar info del alumno
                .Include(s => s.Municipio)
                .Include(s => s.Nivel)
                 .Include(s => s.TipoTramite)
                .AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.Trim().ToUpper();
                solicitudes = solicitudes.Where(s =>
                    s.CURP.ToUpper().Contains(busqueda) ||
                    (s.Alumno != null &&
                    (s.Alumno.Nombre + " " + s.Alumno.Paterno + " " + s.Alumno.Materno)
                        .ToUpper()
                        .Contains(busqueda))
                );
            }

            solicitudes = solicitudes.OrderByDescending(s => s.FechaAlta);
            ViewBag.TiposTramite = _context.TiposTramite.OrderBy(t => t.Nombre).ToList();
            ViewBag.Municipios = _context.Municipios.OrderBy(m => m.Nombre).ToList();
            ViewBag.Niveles = _context.Niveles.OrderBy(n => n.Nombre).ToList();
            return View("~/Views/Catalogos/Solicitudes.cshtml", solicitudes.ToList());
        }

        [HttpPost]
        public IActionResult EditarSolicitud(Guid id, string asunto, string quienRealiza, string estado, int idMunicipio, int idNivel)
        {
            var sol = _context.Solicitudes
                .Include(s => s.Alumno)
                .FirstOrDefault(s => s.SolicitudId == id);

            if (sol == null)
                return RedirectToAction("Solicitudes");

            sol.Asunto = asunto;
            sol.QuienRealiza = quienRealiza;
            sol.MunicipioId = idMunicipio;
            sol.NivelId = idNivel;

            sol.Estatus = estado?.Trim() ?? sol.Estatus;
            if (string.Equals(sol.Estatus, "Resuelto", StringComparison.OrdinalIgnoreCase))
            {
                sol.FechaAtencion = DateTime.Now;
            }
            else
            {
                sol.FechaAtencion = null;
            }

            _context.SaveChanges();
            return RedirectToAction("Solicitudes");
        }

        [HttpPost]
        public IActionResult EliminarSolicitud(Guid id)
        {
            var sol = _context.Solicitudes.Find(id);
            if (sol != null)
            {
                _context.Solicitudes.Remove(sol);
                _context.SaveChanges();
            }
            return RedirectToAction("Solicitudes");
        }

        [HttpPost]
        public IActionResult CrearSolicitud(Alumno alumno, Solicitud solicitud, int tipoTramiteId)
        {
            // Buscar si el alumno ya existe
            var alumnoExistente = _context.Alumnos.Find(alumno.CURP);
            if (alumnoExistente == null)
                _context.Alumnos.Add(alumno);
            else
                solicitud.CURP = alumnoExistente.CURP;

            // Generar número de turno secuencial por municipio
            var ultimoTurno = _context.Solicitudes
                .Where(s => s.MunicipioId == solicitud.MunicipioId)
                .OrderByDescending(s => s.NumeroTurno)
                .Select(s => s.NumeroTurno)
                .FirstOrDefault();

            solicitud.NumeroTurno = ultimoTurno + 1;

            // Asignar los demás valores
            solicitud.TipoTramiteId = tipoTramiteId;
            solicitud.FechaAlta = DateTime.Now;
            solicitud.Estatus = "Pendiente";

            _context.Solicitudes.Add(solicitud);
            _context.SaveChanges();

            return Ok();
        }


        [HttpGet]
        public JsonResult BuscarAlumnoPorCurp(string curp)
        {
            curp = curp?.Trim().ToUpper();
            var alumno = _context.Alumnos.FirstOrDefault(a => a.CURP == curp);

            if (alumno == null)
            {
                return Json(new { existe = false });
            }

            return Json(new  //dto devuelve un tipo texto plano json segun el modelo o clase
            {
                existe = true,
                nombre = alumno.Nombre,
                paterno = alumno.Paterno,
                materno = alumno.Materno,
                telefono = alumno.Telefono,
                correo = alumno.Correo,
                municipioId = alumno.MunicipioId,
                nivelId = alumno.NivelId
            });
        }


    }
}
