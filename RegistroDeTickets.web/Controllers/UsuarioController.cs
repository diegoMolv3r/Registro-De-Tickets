using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Service;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;

namespace RegistroDeTickets.web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ITelemetryService _telemetryService;

        public UsuarioController(IUsuarioService usuarioService, ITelemetryService telemetryService)
        {
            _usuarioService = usuarioService;
            _telemetryService = telemetryService;
        }


        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(UsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                return View(usuarioVM);
            }
            _usuarioService.AgregarUsuario(new Usuario
            {
                Username = usuarioVM.Username,
                Email = usuarioVM.Email,
                PasswordHash = usuarioVM.Contrasenia
            });
            return RedirectToAction("IniciarSesion");
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(LoginViewModel usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            
            var usuarioEncontrado = _usuarioService.BuscarUsuarioPorEmail(usuario.Email);

            if (usuarioEncontrado == null)
            {
                TempData["Mensaje"] = "Usuario Inexistente";

               
                _telemetryService.RegistrarEvento("InicioSesionFallido", usuarioEncontrado);
                return View(usuario);
            }

            _telemetryService.RegistrarEvento("InicioSesionExitoso", usuarioEncontrado);

            return RedirectToAction("Inicio", "Home");
        }

       

        public IActionResult Listar()
        {
            return RedirectToAction("Registrar");
        }
    }
}
