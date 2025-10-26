using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Filters;

namespace TicketDeTurno.Controllers
{
    [AuthFilter]
    public class CatalogosController : Controller
    {
        public IActionResult Municipios()
        {
            ViewData["Title"] = "Municipios";
            return View();
        }

        public IActionResult Niveles()
        {
            ViewData["Title"] = "Niveles";
            return View();
        }
    }
}
