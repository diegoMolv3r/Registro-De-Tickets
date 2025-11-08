using System;
using System.Collections.Generic;

namespace RegistroDeTickets.Data.Entidades;

public partial class ReporteTecnico
{
    public int IdReporte { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public int IdTicket { get; set; }

    public virtual Ticket IdTicketNavigation { get; set; } = null!;
}
