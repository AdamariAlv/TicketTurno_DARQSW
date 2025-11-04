using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Data;

namespace TicketDeTurno.Web.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Solicitudes
        public IActionResult Index()
        {
            var solicitudes = _context.Solicitudes.ToList();
            return View("~/Views/Catalogos/Solicitudes.cshtml", solicitudes);
        }

        // POST: CrearSolicitud
        [HttpPost]
        public IActionResult CrearSolicitud(string descripcion, string estado)
        {
            // Lógica temporal (porque el modelo Solicitud original tiene más campos obligatorios)
            return RedirectToAction("Index");
        }

        // POST: EditarSolicitud
        [HttpPost]
        public IActionResult EditarSolicitud(Guid id, string descripcion, string estado)
        {
            // Lógica para actualizar
            return RedirectToAction("Index");
        }

        // POST: EliminarSolicitud
        [HttpPost]
        public IActionResult EliminarSolicitud(Guid id)
        {
            var solicitud = _context.Solicitudes.Find(id);
            if (solicitud != null)
            {
                _context.Solicitudes.Remove(solicitud);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
