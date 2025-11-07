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
        //private readonly ILogger<UsuarioController> _logger;
        private readonly TelemetryClient _telemetryClient;

        public UsuarioController(IUsuarioService usuarioService, TelemetryClient telemetryClient)
        {
            _usuarioService = usuarioService;
            _telemetryClient = telemetryClient;
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
            var propiedades = new Dictionary<string, string>();
            var usuarioEncontrado = _usuarioService.BuscarUsuarioPorEmail(usuario.Email);

            if (usuarioEncontrado == null)
            { 
                TempData["Mensaje"] = "Usuario Inexistente";
                propiedades = new Dictionary<string, string>
            {
                { "UsuarioId", "" },
                { "MetodoLogin", "EmailYPassword" }
            };
                _telemetryClient.TrackEvent("InicioSesionFallidoPorEmail", propiedades);
                return View(usuario);
            }
                propiedades = new Dictionary<string, string>
            {
                { "UsuarioId", usuarioEncontrado.Id.ToString() },
                { "MetodoLogin", "EmailYPassword" }
            };
            _telemetryClient.TrackEvent("InicioSesionExitoso", propiedades);

            return RedirectToAction("Inicio","Home");
        }
        public IActionResult Listar()
        {
            return RedirectToAction("Registrar");
        }
    }
}
