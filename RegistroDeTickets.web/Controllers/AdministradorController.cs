using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using Usuario = RegistroDeTickets.Data.Entidades.Usuario;
using Microsoft.AspNetCore.Authorization;

namespace RegistroDeTickets.web.Controllers
{
    public class AdministradorController(ITicketService ticketService, IUsuarioService usuarioService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        

       

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Listar()
        {
            return View(_ticketService.ObtenerTickets());
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EliminarTicket(Ticket ticket)
        {
            _ticketService.EliminarTicket(ticket);
            return RedirectToAction("Listar");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ListarUsuarios() {
            return View(_usuarioService.ObtenerUsuarios());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EliminarUsuario(int id)
        {

            _usuarioService.EliminarUsuario(_usuarioService.ObtenerUsuarioPorId(id));
            return RedirectToAction("ListarUsuarios");
        }
       
        [Authorize(Roles = "Admin")]
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
                PasswordHash = usuarioVM.PasswordHash,
                Estado = "Activo"

            };

            _usuarioService.AgregarUsuario(nuevoTecnico);
            _usuarioService.DesignarUsuarioComoTecnico(nuevoTecnico);

            return RedirectToAction("ListarUsuarios");
        }




        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AsignarTecnicoATicket(int idTecnico,int idTicket) {
            _ticketService.AsignarTecnicoATicket(idTicket, idTecnico);
            return RedirectToAction("Listar");
        }
    }
}
