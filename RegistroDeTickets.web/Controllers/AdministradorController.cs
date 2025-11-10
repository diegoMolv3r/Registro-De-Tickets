using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using Usuario = RegistroDeTickets.Data.Entidades.Usuario;
using System.Security.Claims;


namespace RegistroDeTickets.web.Controllers
{
    [Authorize(Roles = "Admin")]
   
    public class AdministradorController(ITicketService ticketService, IUsuarioService usuarioService, UserManager<Usuario> userManager) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly UserManager<Usuario> _userManager = userManager;

        public IActionResult Inicio()
        {
            return View();
        }
        



        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Listar()
        {
            return View(_ticketService.ObtenerTickets());
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet]

        [Authorize(Policy = "PuedeEliminar")]
        public IActionResult EliminarTicket(Ticket ticket)
        {
            _ticketService.EliminarTicket(ticket);
            return RedirectToAction("Listar");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult ListarUsuarios() {
            return View(_usuarioService.ObtenerUsuarios());
        }

        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "PuedeEliminar")]
        [HttpGet]

        public IActionResult EliminarUsuario(int id)
        {
            int idAdminActual = Int32.Parse((HttpContext.User.Identity as ClaimsIdentity).FindFirst("Id").Value);
            if (id == idAdminActual)
            {
                return RedirectToAction("ListarUsuarios");
            }

            _usuarioService.EliminarUsuario(_usuarioService.ObtenerUsuarioPorId(id));
            return RedirectToAction("ListarUsuarios");
        }
       
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult AsignarTecnicoATicket(int Id)
        {
            Ticket ticket = _ticketService.BuscarTicketPorId(Id);
            List<Usuario> tecnicos = _usuarioService.ObtenerTecnicos();
            ViewBag.Ticket = ticket;
            ViewBag.Tecnicos = tecnicos;
            return View();
        }
       
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult RegistrarUsuarioTecnico() {
            return View("RegistrarUsuarioTecnico");
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> RegistrarUsuarioTecnico(UsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                return View(usuarioVM);
            }

            Usuario nuevoTecnico = new Usuario
            {
                UserName = usuarioVM.Username,
                Email = usuarioVM.Email,
                PasswordHash = usuarioVM.PasswordHash,
                Estado = "Activo",
                // PARA QUE FIGURE EN NUESTRA TABLA dbo.Tecnico
                Tecnico = new Tecnico()

            };
            IdentityResult result = await _userManager.CreateAsync(nuevoTecnico, usuarioVM.PasswordHash);

            if (result.Succeeded)
            {
                // Asignar el rol de "Tecnico" al nuevo usuario
                await _userManager.AddToRoleAsync(nuevoTecnico, "Tecnico");
                /*_usuarioService.AgregarUsuario(nuevoTecnico);
                _usuarioService.DesignarUsuarioComoTecnico(nuevoTecnico);
                */
                return RedirectToAction("ListarUsuarios");
            }
            else
            {
                // Se juntan todos los errores en un solo string
                string errores = string.Join(" ", result.Errors.Select(e => e.Description));

                TempData["MensajeErrorE"] = errores;

                return RedirectToAction("RegistrarUsuarioTecnico");
            }
        }




        [Authorize(Roles = "Admin")]
        [HttpGet]
        //[HttpPost][ValidateAntiForgeryToken]
        public IActionResult AsignarTecnicoATicket(int idTecnico,int idTicket) {
            _ticketService.AsignarTecnicoATicket(idTicket, idTecnico);
            return RedirectToAction("Listar");
        }
    }
}
