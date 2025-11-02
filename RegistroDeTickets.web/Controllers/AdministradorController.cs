using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;

namespace RegistroDeTickets.web.Controllers
{
    public class AdministradorController(ITicketService ticketService, IUsuarioService usuarioService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Listar()
        {
            return View(_ticketService.ObtenerTickets());
        }

        public IActionResult EliminarTicket(Ticket ticket)
        {
            _ticketService.EliminarTicket(ticket);
            return RedirectToAction("Listar");
        }

        public IActionResult ListarUsuarios() {
            return View(_usuarioService.ObtenerUsuarios());
        }

        // COMPLETAR LOS SIGUIENTES METODOS DEL LADO DEL SERVICIO Y REPOSITORIO
        public IActionResult EliminarUsuario(int id)
        {

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

        [HttpPost]
        public IActionResult AsignarTecnicoATicket(int idTecnico,int idTicket) {
            _ticketService.AsignarTecnicoATicket(idTicket, idTecnico);
            return RedirectToAction("Listar");
        }
    }
}
