using RegistroDeTickets.Data.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroDeTickets.Repository
{
    public interface IReporteRepository
    {
        void agregarReporte(ReporteTecnico reporte);
    }

    public class ReporteRepository : IReporteRepository
    {
        private static readonly RegistroDeTicketsPw3Context ctx = new();

        public void agregarReporte(ReporteTecnico reporte)
        {
            ctx.ReporteTecnicos.Add(reporte);
            ctx.SaveChanges();
        }
    }
}
