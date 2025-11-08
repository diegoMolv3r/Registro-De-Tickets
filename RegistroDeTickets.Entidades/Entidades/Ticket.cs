using System;
using System.Collections.Generic;

namespace RegistroDeTickets.Data.Entidades;

public partial class Ticket
{
    public int Id { get; set; }

    public string Motivo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public int IdCliente { get; set; }

    public int? IdTecnico { get; set; }

    public int EstadoId { get; set; }

    public int PrioridadId { get; set; }

    public virtual TicketEstado Estado { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Tecnico? IdTecnicoNavigation { get; set; }

    public virtual TicketPrioridad Prioridad { get; set; } = null!;

    public virtual ICollection<ReporteTecnico> ReporteTecnicos { get; set; } = new List<ReporteTecnico>();
}
