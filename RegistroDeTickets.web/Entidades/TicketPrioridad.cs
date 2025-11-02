using System;
using System.Collections.Generic;

namespace RegistroDeTickets.web.Entidades;

public partial class TicketPrioridad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
