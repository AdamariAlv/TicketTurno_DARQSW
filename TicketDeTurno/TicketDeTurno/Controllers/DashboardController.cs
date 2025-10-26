using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Filters;

namespace TicketDeTurno.Controllers
{
    [AuthFilter]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }
    }
}
