using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using Microsoft.IdentityModel.Tokens;

namespace RegistroDeTickets.web.Controllers
{
    public class UsuarioController : Controller
    {
        //jwt
        private readonly TokenService _tokenService;

        //
        private readonly IUsuarioService _usuarioService;
        private string UsuarioE;

        public readonly IEmailService _emailService;

        public UsuarioController(IUsuarioService usuarioService, IEmailService emailService, TokenService tokenService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
            _tokenService = tokenService;
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
            _usuarioService.AgregarUsuario(new Data.Entidades.Usuario
            {
                Username = usuarioVM.Username,
                Email = usuarioVM.Email,
                PasswordHash = usuarioVM.PasswordHash,
                Estado = "Activo"
            });
                return RedirectToAction("IniciarSesion");
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(LoginViewModel usuario)
        {

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioEncontrado = _usuarioService.BuscarPorEmail(usuario.Email);

            if (usuarioEncontrado == null)
            { 
                TempData["MensajeErrorE"] = "Usuario Inexistente";
                return View(usuario);
            }

            if (usuarioEncontrado.PasswordHash != usuario.PasswordHash)
            {
                TempData["MensajeErrorP"] = "Contraseña incorrecta";
                return View(usuario);
            }



            TempData["UsuarioE"] = usuarioEncontrado.Username;
            //jwt 
            var token = _tokenService.GenerateToken(usuarioEncontrado.Username);
            //cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(1)
            });

            return RedirectToAction("Inicio","Home");
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
        

        public IActionResult CerrarSesion()
        {
            Response.Cookies.Delete("jwt");
        
            return RedirectToAction("IniciarSesion", "Usuario");

        }


        public IActionResult Listar()
        {
            return RedirectToAction("Registrar");
        }

        [HttpGet]
        public IActionResult SolicitarRecuperacion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarRecuperacion(SolicitarRecuperacionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var token = _usuarioService.GenerarTokenRecuperacion(vm.Email);

            if (token != null)
            {
                var link = GenerarLinkRecuperacion(vm.Email, token);

                var cuerpoEmail = GenerarCuerpoEmailRecuperacion(link);

                await _emailService.EnviarEmail(vm.Email, "Recuperación de Contraseña", cuerpoEmail);
            }

            return RedirectToAction("SolicitarRecuperacionConfirmacion");
        }

        private string GenerarLinkRecuperacion(string email, string token)
        {
            return Url.Action(
                action: "RestablecerContrasenia",
                controller: "Usuario",
                values: new { email = email, token = token },
                protocol: Request.Scheme
            );
        }

        private string GenerarCuerpoEmailRecuperacion(string link)
        {
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>

        </head>
        <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; '>
            <h1 style='color: #2c3e50; font-size: 24px; font-weight: bold; margin-bottom: 20px;'>
                Recuperación de Contraseña
            </h1>
            
            <p style='margin-bottom: 15px;'>
                Recibimos una solicitud para restablecer tu contraseña. 
                Si no fuiste vos, ignorá este mensaje.
            </p>
            
            <p style='margin-bottom: 15px;'>
                Hacé click en el siguiente botón para continuar:
            </p>
            
            <p style='margin-bottom: 15px;'>
                <a href='{link}' 
                   style='background-color: #3498db; 
                          color: white; 
                          padding: 12px 24px; 
                          text-decoration: none; 
                          border-radius: 5px; 
                          display: inline-block;'>
                    Restablecer mi contraseña
                </a>
            </p>
            
            <p style='margin-top: 20px; color: #7f8c8d; font-size: 14px;'>
                El enlace expirará en 30 minutos.
            </p>
        </body>
        </html>
    ";
        }

        [HttpGet]
        public IActionResult SolicitarRecuperacionConfirmacion()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RestablecerContrasenia(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("IniciarSesion");
            }

            var vm = new RestablecerContraseniaViewModel
            {
                Email = email,
                Token = token
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult RestablecerContrasenia(RestablecerContraseniaViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var exito = _usuarioService.RestablecerContrasenia(
                vm.Email,
                vm.Token,
                vm.NuevaContrasenia
            );

            if (exito)
            {
                TempData["MensajeExito"] = "¡Tu contraseña ha sido actualizada con éxito!";
                return RedirectToAction("IniciarSesion");
            }

            ModelState.AddModelError(string.Empty, "El enlace de recuperación no es válido o ha expirado. Por favor, solicitá uno nuevo.");
            return View(vm);
        }
    }
}

