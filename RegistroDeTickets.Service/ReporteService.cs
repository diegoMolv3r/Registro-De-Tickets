using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Repository;

namespace RegistroDeTickets.Service
{
    public interface IReporteService
    {
        public void AgregarReporte(ReporteTecnico reporte);
    }
    public class ReporteService(IReporteRepository reporteRepository) : IReporteService
    {
        private readonly IReporteRepository _reporteRepository = reporteRepository;
        public void AgregarReporte(ReporteTecnico reporte)
        {
            _reporteRepository.agregarReporte(reporte);
        }
    }
}
