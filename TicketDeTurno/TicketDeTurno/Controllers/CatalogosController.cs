using Microsoft.AspNetCore.Mvc;
using TicketDeTurno.Web.Data;
using TicketDeTurno.Web.Models;
using TicketDeTurno.Web.Filters;

namespace TicketDeTurno.Controllers
    {
        [AuthFilter]
        public class CatalogosController : Controller
        {
            private readonly ApplicationDbContext _context;

            public CatalogosController(ApplicationDbContext context)
            {
                _context = context;
            }

        // CRUD Municipios
        public IActionResult Municipios()
            {
                var municipios = _context.Municipios.ToList();
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

    }
}
