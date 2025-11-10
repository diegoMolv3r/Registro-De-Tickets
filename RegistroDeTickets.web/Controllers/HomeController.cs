using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Service;

namespace RegistroDeTickets.web.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Inicio()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            //var token = Request.Cookies["jwt"];
            //if (string.IsNullOrEmpty(token))
            //{
            //    return RedirectToAction("IniciarSesion", "Usuario");
            //}

            return View();
        }
    }
}
