using System;
using System.Collections.Generic;

namespace RegistroDeTickets.Data.Entidades;

public partial class Cliente
{
    public int Id { get; set; }

    public string? Domicilio { get; set; }

    public virtual Usuario IdNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
