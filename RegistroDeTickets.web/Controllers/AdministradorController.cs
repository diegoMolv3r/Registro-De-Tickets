using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using Usuario = RegistroDeTickets.Data.Entidades.Usuario;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RegistroDeTickets.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministradorController(ITicketService ticketService, IUsuarioService usuarioService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        public IActionResult Inicio()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Listar()
        {
            return View(_ticketService.ObtenerTickets());
        }

        [Authorize(Policy = "PuedeEliminar")]
        public IActionResult EliminarTicket(Ticket ticket)
        {
            _ticketService.EliminarTicket(ticket);
            return RedirectToAction("Listar");
        }
      
        [HttpGet]
        public IActionResult ListarUsuarios() {
            return View(_usuarioService.ObtenerUsuarios());
        }

        // COMPLETAR LOS SIGUIENTES METODOS DEL LADO DEL SERVICIO Y REPOSITORIO
        [Authorize(Policy = "PuedeEliminar")]
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

        public IActionResult DesignarTecnico(int id) {
            _usuarioService.DesignarUsuarioComoTecnico(_usuarioService.ObtenerUsuarioPorId(id));
            return RedirectToAction("ListarUsuarios");
        }

        public IActionResult DesignarCliente(int id)
        {
            _usuarioService.DesignarUsuarioComoCliente(_usuarioService.ObtenerUsuarioPorId(id));
            return RedirectToAction("ListarUsuarios");
        }

        [HttpGet]
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
        public IActionResult RegistrarUsuarioTecnico() {
            return View("RegistrarUsuarioTecnico");
        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult RegistrarUsuarioTecnico(UsuarioViewModel usuarioVM)
        {
            if (!ModelState.IsValid)
            {
                return View(usuarioVM);
            }

            Usuario nuevoTecnico = new Usuario
            {
                UserName = usuarioVM.Username,
                Email = usuarioVM.Email,
                PasswordHash = usuarioVM.PasswordHash
            };

            _usuarioService.AgregarUsuario(nuevoTecnico);
            _usuarioService.DesignarUsuarioComoTecnico(nuevoTecnico);

            return RedirectToAction("IniciarSesion", "Usuario");
        }


          

        [HttpPost]
        public IActionResult AsignarTecnicoATicket(int idTecnico,int idTicket) {
            _ticketService.AsignarTecnicoATicket(idTicket, idTecnico);
            return RedirectToAction("Listar");
        }
    }
}
