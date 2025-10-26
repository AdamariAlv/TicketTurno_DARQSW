using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TicketDeTurno.Web.Filters
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                // Limpia cache por seguridad adicional
                context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.HttpContext.Response.Headers["Pragma"] = "no-cache";
                context.HttpContext.Response.Headers["Expires"] = "0";

                // Redirige al login
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
