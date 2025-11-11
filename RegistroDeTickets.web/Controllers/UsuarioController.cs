using Google.Apis.Auth;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using Sprache;
using Usuario = RegistroDeTickets.Data.Entidades.Usuario;

namespace RegistroDeTickets.web.Controllers
{
    public class UsuarioController : Controller
    {
        //jwt
        private readonly TokenService _tokenService;

        //
        private readonly IUsuarioService _usuarioService;
        private readonly ITelemetryService _telemetryService;
        private string UsuarioE;

        public readonly IEmailService _emailService;
        //
        private readonly UserManager<Usuario> _userManager;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        // LOS INYECTO AL CONSTRUCTOR

        public UsuarioController(IUsuarioService usuarioService, IEmailService emailService, TokenService tokenService, ITelemetryService telemetryService, UserManager<Usuario> userManager,
        IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
            _tokenService = tokenService;
            _telemetryService = telemetryService;
            //
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            //
        }


        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                return View(usuarioVM);
            }

            var nuevoUsuario = new Data.Entidades.Usuario
            {
                UserName = usuarioVM.Username,
                Email = usuarioVM.Email,
                PasswordHash = usuarioVM.PasswordHash,
                Estado = "Activo",
                // PARA QUE FIGURE EN NUESTRA TABLA dbo.Cliente
                Cliente = new Cliente()

            };
            //_usuarioService.AgregarUsuario(nuevoUsuario);
            // AHORA SE UTILIZA 'CREATEASYNC' PARA AGREGAR USUARIOS-CLIENTES A LA BD 
            IdentityResult result = await _userManager.CreateAsync(nuevoUsuario, usuarioVM.PasswordHash);

            if (result.Succeeded)
            {               
                await _userManager.AddToRoleAsync(nuevoUsuario, "Cliente");
                return RedirectToAction("IniciarSesion");
            }
            else
            {
                // Se juntan todos los errores en un solo string
                string errores = string.Join(" ", result.Errors.Select(e => e.Description));

                
                TempData["MensajeErrorE"] = errores;

               
                return RedirectToAction("Registrar");
            }
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult IniciarSesion()
        {
            ViewBag.GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel usuario)
        {

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var usuarioEncontrado = _usuarioService.BuscarPorEmail(usuario.Email);

            if (usuarioEncontrado == null)
            { 
                TempData["MensajeErrorE"] = "Credenciales incorrectas. Intentalo nuevamente";
                _telemetryService.RegistrarEvento("InicioSesionFallidoPorEmail", usuarioEncontrado);
                return View(usuario);
            }

            var resultadoVerificacion = _passwordHasher.VerifyHashedPassword(
                usuarioEncontrado,
                usuarioEncontrado.PasswordHash,
                usuario.PasswordHash

                );

            if (resultadoVerificacion == PasswordVerificationResult.Failed)
            {
                TempData["MensajeErrorP"] = "Credenciales incorrectas. Intentalo nuevamente";
                _telemetryService.RegistrarEvento("InicioSesionFallidoPorContraseña", usuarioEncontrado);
                return View(usuario);
            }

            _telemetryService.RegistrarEvento("InicioSesionExitoso", usuarioEncontrado);

            // BUSCO EL ROL EN LA BASE DE DATOS
            var rolesDelUsuario = await _userManager.GetRolesAsync(usuarioEncontrado);

            var claimsAdicionales = await _userManager.GetClaimsAsync(usuarioEncontrado);

            TempData["UsuarioE"] = usuarioEncontrado.UserName;
            //jwt 
            //var token = _tokenService.GenerateToken(usuarioEncontrado.UserName);

            // MODIFICO EL GENERATE TOKEN PARA QUE ACEPTE ROLES
            var token = _tokenService.GenerateToken(usuarioEncontrado.UserName, rolesDelUsuario,usuarioEncontrado.Id,
        claimsAdicionales);
            //cookie


            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddHours(1)
            });
            if (rolesDelUsuario.Contains("Tecnico")){
                return RedirectToAction("Inicio", "Tecnico");
            }
            if (rolesDelUsuario.Contains("Admin"))
            {
                return RedirectToAction("Inicio", "Administrador");
            }
            return RedirectToAction("Inicio","Cliente");
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
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
                        Audience = new[] { Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") }
                    });

                var usuarioEncontradoPorMail = _usuarioService.BuscarPorEmail(payload.Email);

                // 1. VERIFICACIÓN DE EMAIL (Devuelve JSON)
                if (usuarioEncontradoPorMail != null)
                {   
                    _telemetryService.RegistrarEvento("InicioSesionExitoso", usuarioEncontradoPorMail);
                                       
                    var rolesDelUsuario = await _userManager.GetRolesAsync(usuarioEncontradoPorMail);
                    var claimsAdicionales = await _userManager.GetClaimsAsync(usuarioEncontradoPorMail);

                    TempData["UsuarioE"] = usuarioEncontradoPorMail.UserName;
                    /*usuarioEncontrado.UserName, rolesDelUsuario,usuarioEncontrado.Id,claimsAdicionales*/
                    var token = _tokenService.GenerateToken(usuarioEncontradoPorMail.UserName, rolesDelUsuario, usuarioEncontradoPorMail.Id, claimsAdicionales);
                    
                    Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddHours(1)
                    });

                    return Ok(new
                    {
                        success = true,
                        redirectUrl = Url.Action("Listar", "Cliente")
                    });
                }

                string nombreUsuarioGoogle = _usuarioService.renombrarUsuarioGoogle(payload.Email, payload.Name);

                var nuevoUsuario = new Usuario
                {
                    UserName = nombreUsuarioGoogle,
                    Email = payload.Email,
                    Estado = "Activo",
                    Cliente = new Cliente()
                };

                // 2. CREACIÓN DE USUARIO (SIN PASSWORD)
                IdentityResult result = await _userManager.CreateAsync(nuevoUsuario);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(nuevoUsuario, "Cliente");

                    _telemetryService.RegistrarEvento("InicioSesionExitoso", nuevoUsuario);

                    var rolesDelUsuario = await _userManager.GetRolesAsync(nuevoUsuario);
                    var claimsAdicionales = await _userManager.GetClaimsAsync(nuevoUsuario);
                    TempData["UsuarioE"] = nuevoUsuario.UserName;
                    //var token = _tokenService.GenerateToken(usuarioEncontradoPorMail.UserName, rolesDelUsuario, usuarioEncontradoPorMail.Id, claimsAdicionales);
                    var token = _tokenService.GenerateToken(nuevoUsuario.UserName, rolesDelUsuario, nuevoUsuario.Id, claimsAdicionales);

                    Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddHours(1)
                    });

                    // 3. RESPUESTA DE ÉXITO (Devuelve JSON)
                    return Ok(new
                    {
                        success = true,
                        redirectUrl = Url.Action("Listar", "Cliente")
                    });
                }
                else
                {
                    // 4. RESPUESTA DE ERROR (Devuelve JSON)
                    string errores = string.Join(" ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { success = false, message = errores });
                }
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
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("IniciarSesion", "Usuario");

        }


        public IActionResult Listar()
        {
            return RedirectToAction("Registrar");
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
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
        [AutoValidateAntiforgeryToken]
        public IActionResult SolicitarRecuperacionConfirmacion()
        {
            return View();
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
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

