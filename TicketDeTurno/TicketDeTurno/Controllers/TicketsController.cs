using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketDeTurno.Web.Data;
using TicketDeTurno.Web.Models;

namespace TicketDeTurno.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Confirmacion()
        {
            return View();
        }


        // GET: /Tickets/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            ViewBag.Municipios = _context.Municipios.ToList();
            ViewBag.Niveles = _context.Niveles.ToList();
            ViewBag.TiposTramite = _context.TiposTramite.ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ValidarCurp(string curp)
        {
            if (string.IsNullOrWhiteSpace(curp))
                return Json(new { ok = false, error = "CURP vacía" });

            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(8);

                // Endpoint público (no oficial) que devuelve HTML
                var url = $"https://consultas.curp.gob.mx/CurpSP/curp2.do?strCurp={Uri.EscapeDataString(curp)}";
                var html = await http.GetStringAsync(url);

                // Heurísticas simples (pueden cambiar si RENAPO cambia el HTML)
                var notFound =
                    html.Contains("no se encuentra", StringComparison.OrdinalIgnoreCase) ||
                    html.Contains("no existe", StringComparison.OrdinalIgnoreCase);
                var ok = !notFound && html.Length > 500; // página con datos suele ser grandecita

                return Json(new { ok });
            }
            catch
            {
                // Si falla la consulta, no bloqueamos el flujo (demo)
                return Json(new { ok = false, error = "fallo_http" });
            }
        }


        // POST: /Tickets/Registrar
        [HttpPost]
        public IActionResult Registrar(Solicitud model)
        {
            // 🔄 Evita validar la propiedad de navegación que aún no está asignada
            ModelState.Remove("TipoTramite");

            if (!ModelState.IsValid)
            {
                ViewBag.Municipios = _context.Municipios.ToList();
                ViewBag.Niveles = _context.Niveles.ToList();
                ViewBag.TiposTramite = _context.TiposTramite.ToList();
                return View(model);
            }

            // 🔍 Buscar o crear alumno
            var alumno = _context.Alumnos.FirstOrDefault(a => a.CURP == model.CURP);
            if (alumno == null)
            {
                alumno = new Alumno
                {
                    CURP = model.CURP,
                    Nombre = model.Alumno?.Nombre ?? "",
                    Paterno = model.Alumno?.Paterno ?? "",
                    Materno = model.Alumno?.Materno ?? "",
                    Telefono = model.Alumno?.Telefono ?? "",
                    Correo = model.Alumno?.Correo ?? "",
                    NivelId = model.NivelId,
                    MunicipioId = model.MunicipioId
                };

                _context.Alumnos.Add(alumno);
                _context.SaveChanges();
            }
            else
            {
                // 📌 Actualiza municipio y nivel solo si fueron cambiados
                alumno.MunicipioId = model.MunicipioId;
                alumno.NivelId = model.NivelId;
                _context.Update(alumno);
                _context.SaveChanges();
            }

            // 🧠 Reconsultar alumno con contexto actualizado
            alumno = _context.Alumnos.First(a => a.CURP == model.CURP);

            // 🔢 Turno secuencial por municipio
            var ultimo = _context.Solicitudes
                .Where(s => s.MunicipioId == model.MunicipioId)
                .OrderByDescending(s => s.NumeroTurno)
                .FirstOrDefault();

            model.NumeroTurno = (ultimo?.NumeroTurno ?? 0) + 1;
            model.SolicitudId = Guid.NewGuid();
            model.Estatus = "Pendiente";
            model.FechaAlta = DateTime.Now;

            // ✅ Relaciona tipo de trámite
            model.TipoTramite = _context.TiposTramite.First(t => t.TipoTramiteId == model.TipoTramiteId);

            // ✅ Relación con alumno
            model.CURP = alumno.CURP;
            model.Alumno = alumno;

            // 💾 Guardar solicitud
            _context.Solicitudes.Add(model);
            _context.SaveChanges();

            // 🎉 Confirmación
            TempData["Turno"] = model.NumeroTurno;
            TempData["CURP"] = model.CURP;

            return RedirectToAction("Confirmacion");
        }

        [HttpGet]
        public IActionResult BuscarAlumnoPorCurp(string curp)
        {
            if (string.IsNullOrWhiteSpace(curp))
                return Json(new { existe = false });

            var alumno = _context.Alumnos
                .Where(a => a.CURP == curp)
                .Select(a => new
                {
                    existe = true,
                    nombre = a.Nombre,
                    paterno = a.Paterno,
                    materno = a.Materno,
                    telefono = a.Telefono,
                    correo = a.Correo,
                    municipioId = a.MunicipioId,
                    nivelId = a.NivelId
                })
                .FirstOrDefault();

            if (alumno != null)
                return Json(alumno);
            else
                return Json(new { existe = false });
        }

    [HttpGet]
        public IActionResult Consultar()
        {
            ViewBag.Municipios = _context.Municipios.ToList();
            ViewBag.Niveles = _context.Niveles.ToList();
            ViewBag.TiposTramite = _context.TiposTramite.ToList();
            return View();
        }

        [HttpGet]
        public IActionResult BuscarEstatus(string curp, int municipioId, int turno)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.Alumno)
                .Include(s => s.Municipio)
                .Include(s => s.TipoTramite)
                .FirstOrDefault(s =>
                    s.CURP == curp &&
                    s.MunicipioId == municipioId &&
                    s.NumeroTurno == turno
                );

            if (solicitud == null)
                return Json(new { encontrado = false });

            return Json(new
            {
                encontrado = true,
                solicitudId = solicitud.SolicitudId,
                curp = solicitud.CURP,
                nombre = $"{solicitud.Alumno?.Nombre} {solicitud.Alumno?.Paterno}",
                municipio = solicitud.Municipio?.Nombre,
                tramite = solicitud.TipoTramite?.Nombre,
                estatus = solicitud.Estatus,
                fechaAlta = solicitud.FechaAlta.ToString("dd/MM/yyyy")
            });
        }




        // Obtener datos para edición
        [HttpGet]
        public IActionResult ObtenerSolicitud(Guid id)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.Alumno)
                .Include(s => s.Municipio)
                .Include(s => s.Nivel)
                .Include(s => s.TipoTramite)
                .FirstOrDefault(s => s.SolicitudId == id);

            if (solicitud == null)
                return Json(new { ok = false, msg = "No se encontró la solicitud" });

            return Json(new
            {
                ok = true,
                id = solicitud.SolicitudId,
                curp = solicitud.CURP,
                nombre = $"{solicitud.Alumno?.Nombre} {solicitud.Alumno?.Paterno} {solicitud.Alumno?.Materno}",
                municipioId = solicitud.MunicipioId,
                nivelId = solicitud.NivelId,
                tipoTramiteId = solicitud.TipoTramiteId,
                asunto = solicitud.Asunto,
                estatus = solicitud.Estatus
            });
        }


        // Guardar cambios desde el público
        [HttpPost]
        public IActionResult ActualizarSolicitud(Guid id, int municipioId, int nivelId, int tipoTramiteId, string asunto)
        {
            var solicitud = _context.Solicitudes.FirstOrDefault(s => s.SolicitudId == id);

            if (solicitud == null)
                return Json(new { ok = false, msg = "No se encontró la solicitud." });

            if (solicitud.Estatus == "Resuelto")
                return Json(new { ok = false, msg = "La solicitud ya está resuelta y no puede editarse." });

            solicitud.MunicipioId = municipioId;
            solicitud.NivelId = nivelId;
            solicitud.TipoTramiteId = tipoTramiteId;
            solicitud.Asunto = asunto.ToUpper();

            _context.SaveChanges();

            return Json(new { ok = true, msg = "Solicitud actualizada correctamente." });
        }

    }


}

