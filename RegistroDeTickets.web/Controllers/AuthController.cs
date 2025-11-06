using Microsoft.AspNetCore.Mvc;

namespace RegistroDeTickets.web.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
