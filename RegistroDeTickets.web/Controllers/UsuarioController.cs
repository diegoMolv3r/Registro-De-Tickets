using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Service;
using RegistroDeTickets.Data.Entidades;
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

        public UsuarioController(IUsuarioService usuarioService,TokenService tokenService)
        {
            _usuarioService = usuarioService;
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
                PasswordHash = usuarioVM.PasswordHash
            });
                return RedirectToAction("Listar");
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

        public IActionResult CerrarSesion()
        {
            Response.Cookies.Delete("jwt");
        
            return RedirectToAction("IniciarSesion", "Usuario");

        }


        public IActionResult Listar()
        {
            return View();
        }
    }
}
