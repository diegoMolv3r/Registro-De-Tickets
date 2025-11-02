using Microsoft.AspNetCore.Mvc;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Service;
using RegistroDeTickets.web.Models;

namespace RegistroDeTickets.web.Controllers
{
    public class TecnicoController(ITicketService ticketService, IReporteService reporteService) : Controller
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly IReporteService _reporteService = reporteService;

        public IActionResult ListarTickets()
        {       
            return View(_ticketService.BuscarTicketsPorIdTecnico(3)); // Por ahora uso un tecnico fijo, luego se debe obtener el usuario logueado);
        }

        [HttpGet]
        public IActionResult VerTicket(int Id)
        {
            ViewBag.Ticket = _ticketService.BuscarTicketPorIdConReporte(Id);
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
