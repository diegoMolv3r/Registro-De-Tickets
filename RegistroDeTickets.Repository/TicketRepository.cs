using Microsoft.EntityFrameworkCore;
using RegistroDeTickets.Data.Entidades;

namespace RegistroDeTickets.Repository
{
    public interface ITicketRepository // esta interfaz se podria hacer generica para no hacer una por cada entidad?
    {
        void AgregarTicket(Ticket ticket);
        List<Ticket> ObtenerTickets();
        void EditarTicket(Ticket ticket);
        void EliminarTicket(Ticket ticket);
        Ticket BuscarTicketPorId(int id);
        Ticket BuscarTicketPorIdConReporte(int id);

        List<Ticket> BuscarTicketsPorIdTecnico(int idTecnico);
        List<Ticket> BuscarTicketsPorIdCliente(int idCliente);
    }

    public class TicketRepository : ITicketRepository
    {
        private static readonly RegistroDeTicketsPw3Context ctx = new();

        public void AgregarTicket(Ticket ticket)
        {
            ctx.Tickets.Add(ticket);
            ctx.SaveChanges();
        }

        public Ticket BuscarTicketPorId(int id) 
        {
            return ctx.Tickets.Include(t => t.Estado).Include(t => t.Prioridad).Include(t => t.Estado).FirstOrDefault(t => t.Id == id);
        }

        public void EditarTicket(Ticket ticket)
        {
            // Editar ticket seria solo para modificar el estado y el tecnico asignado
            ctx.Tickets.Update(ticket);
            ctx.SaveChanges();
        }

        public void EliminarTicket(Ticket ticket)
        {
            ctx.Tickets.Remove(BuscarTicketPorId(ticket.Id));
            ctx.SaveChanges();
        }

        public List<Ticket> ObtenerTickets()
        {
            return ctx.Tickets.Include(t=>t.Prioridad).Include(t=>t.Estado).ToList();
        }
        
        public List<Ticket> BuscarTicketsPorIdTecnico(int idTecnico)
        {
            return ctx.Tickets
                .Where(t => t.IdTecnico == idTecnico).Include(t => t.Prioridad).Include(t => t.Estado)
                .ToList();
        }

        public Ticket BuscarTicketPorIdConReporte(int id) 
        {
               
            return ctx.Tickets
                    .Include(t => t.ReporteTecnicos).Include(t => t.Prioridad).Include(t => t.Estado)
                    .First(t => t.Id == id);

        }

        public List<Ticket> BuscarTicketsPorIdCliente(int idCliente)
        {
            return ctx.Tickets
                .Where(t => t.IdCliente == idCliente).Include(t => t.Prioridad).Include(t => t.Estado)
                .ToList();
        }
    }
}
