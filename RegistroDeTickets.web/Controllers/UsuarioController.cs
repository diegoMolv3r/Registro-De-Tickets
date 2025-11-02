using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;

namespace RegistroDeTickets.web.Controllers
{
    public class UsuarioController(IUsuarioService usuarioService) : Controller
    {
        public readonly IUsuarioService _usuarioService = usuarioService;

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
            return RedirectToAction("Listar");
        }

        public IActionResult IniciarSesion()
        {
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            return View();
        }

        [HttpGet]
        public IActionResult GoogleSignIn()
        {
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            return RedirectToAction("IniciarSesion");
        }

        [HttpPost]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleTokenDto data)
        {
            if (string.IsNullOrEmpty(data?.Credential))
                return BadRequest(new { success = false, message = "Token inválido o vacío." });

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    data.Credential,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[]
                        {
                    Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID")
                        }
                    });

                _usuarioService.RegistrarUsuarioGoogle(payload.Email, payload.Name);

                return Ok(new
                {
                    success = true,
                    redirectUrl = Url.Action("Inicio", "Home")
                });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { success = false, message = "Token de Google inválido." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}
