using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Data;
using TicketDeTurno.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace TicketDeTurno.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Página principal del login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Procesa el login
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.Activo);

            if (usuario == null)
            {
                ViewBag.Error = "Usuario no encontrado o inactivo.";
                return View(model);
            }

            // Comparar contraseñas (temporal: texto plano)
            if (usuario.PasswordHash != model.Password)
            {
                ViewBag.Error = "Contraseña incorrecta.";
                return View(model);
            }

            // Guardar sesión
            HttpContext.Session.SetString("Usuario", usuario.Login);
            HttpContext.Session.SetString("Rol", usuario.Rol);

            return RedirectToAction("Index", "Dashboard");
        }

        // Cerrar sesión
        public IActionResult Logout()
        {
            // Limpia todas las variables de sesión
            HttpContext.Session.Clear();

            // Evita que el navegador guarde el cache
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Index", "Login");
        }
    }
}
