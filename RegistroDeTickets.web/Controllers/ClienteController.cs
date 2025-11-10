using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using System.Security.Claims;

namespace RegistroDeTickets.web.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteController(ITicketService ticketService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;

        public IActionResult Inicio()
        {
            return View();
        }

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Registrar()
        {
            return View();
        }
        [Authorize(Roles = "Cliente")]
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
                Descripcion = ticketVM.Descripcion,
                IdCliente = Int32.Parse((HttpContext.User.Identity as ClaimsIdentity).FindFirst("Id").Value)
            });
            return RedirectToAction("Listar");
        }
        [Authorize(Roles = "Cliente")]
        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult Listar()
        {
            int Id;

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claims = identity.Claims;

            Id = Int32.Parse(identity.FindFirst("Id").Value);

            return View(_ticketService.BuscarTicketsPorIdCliente(Id));
        }
    }
}
