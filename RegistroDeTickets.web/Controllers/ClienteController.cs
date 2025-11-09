using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;

namespace RegistroDeTickets.web.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteController(ITicketService ticketService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Verificar si el token de autenticacion es válido, solo peticiones POST
        public IActionResult Registrar(TicketViewModel ticketVM)
        {
            if (!ModelState.IsValid)
            {
                return View(ticketVM);
            }

            _ticketService.AgregarTicket(new Ticket
            {
                Motivo = ticketVM.Motivo,
                PrioridadId = ticketVM.Prioridad,
                Descripcion = ticketVM.Descripcion
            });
            return RedirectToAction("Listar");
        }

        [HttpGet]
        public IActionResult Listar()
        {
            return View(_ticketService.ObtenerTickets());
        }
    }
}
