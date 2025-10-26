using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Filters;

namespace TicketDeTurno.Controllers
{
    [AuthFilter]
    public class SolicitudesController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Solicitudes";
            return View();
        }
    }
}
