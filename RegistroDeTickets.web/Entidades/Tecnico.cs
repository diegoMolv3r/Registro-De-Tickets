using System;
using System.Collections.Generic;

namespace RegistroDeTickets.web.Entidades;

public partial class Tecnico
{
    public int Id { get; set; }

    public virtual Usuario IdNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
