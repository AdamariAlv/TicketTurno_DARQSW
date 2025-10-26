using Microsoft.AspNetCore.Mvc;
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
            if (!ModelState.IsValid)
            {
                ViewBag.Municipios = _context.Municipios.ToList();
                ViewBag.Niveles = _context.Niveles.ToList();
                ViewBag.TiposTramite = _context.TiposTramite.ToList();
                return View(model);
            }

            // 1️⃣ Buscar o crear alumno
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

                // Adjuntar relaciones existentes
                alumno.Nivel = new Nivel { NivelId = model.NivelId };
                alumno.Municipio = new Municipio { MunicipioId = model.MunicipioId };
                _context.Attach(alumno.Nivel);
                _context.Attach(alumno.Municipio);

                _context.Alumnos.Add(alumno);
                _context.SaveChanges();
            }

            // 🔄 Reconsultar para asegurar que EF tiene la instancia actual
            alumno = _context.Alumnos.First(a => a.CURP == model.CURP);

            // 2️⃣ Asignar turno secuencial por municipio
            var ultimo = _context.Solicitudes
                .Where(s => s.MunicipioId == model.MunicipioId)
                .OrderByDescending(s => s.NumeroTurno)
                .FirstOrDefault();

            model.NumeroTurno = (ultimo?.NumeroTurno ?? 0) + 1;
            model.SolicitudId = Guid.NewGuid();
            model.Estatus = "Pendiente";
            model.FechaAlta = DateTime.Now;

            // ⚡ Vincular alumno y su CURP de manera explícita
            model.CURP = alumno.CURP;
            model.Alumno = alumno;

            // 3️⃣ Guardar solicitud
            _context.Solicitudes.Add(model);
            _context.SaveChanges();

            // 4️⃣ Confirmación
            TempData["Turno"] = model.NumeroTurno;
            TempData["CURP"] = model.CURP;

            return RedirectToAction("Confirmacion");
        }


    }
}
