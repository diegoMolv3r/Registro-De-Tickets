using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace RegistroDeTickets.web.Controllers
{
    [Authorize(Roles = "Tecnico")]
    public class TecnicoController(ITicketService ticketService, IReporteService reporteService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IReporteService _reporteService = reporteService;

        public IActionResult Inicio()
        {
            return View();
        }

        public IActionResult ListarTickets()
        {
            int Id;
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claims = identity.Claims;
            // or
            Id = Int32.Parse(identity.FindFirst("Id").Value);

            return View(_ticketService.BuscarTicketsPorIdTecnico(Id)); // Por ahora uso un tecnico fijo, luego se debe obtener el usuario logueado);
        }

        [HttpGet]
        [AutoValidateAntiforgeryToken]
        public IActionResult VerTicket(int Id)
        {
            Ticket ticketBuscado = _ticketService.BuscarTicketPorIdConReporte(Id);
            if (ticketBuscado.IdTecnico != Int32.Parse((HttpContext.User.Identity as ClaimsIdentity).FindFirst("Id").Value)){
                return RedirectToAction("Inicio", "Tecnico");
            }

            ViewBag.Ticket = ticketBuscado;
            return View();
        }
        [HttpPost]
        public IActionResult GenerarReporte(ReporteViewModel reporteVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Ticket = _ticketService.BuscarTicketPorId(reporteVM.IdTicket);
                return View("VerTicket", reporteVM);
            }

            _reporteService.AgregarReporte(new ReporteTecnico
            {
                Descripcion = reporteVM.Descripcion,
                IdTicket = reporteVM.IdTicket
            });

            return RedirectToAction("ListarTickets");
        }
    }
}
