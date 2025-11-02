using System.ComponentModel.DataAnnotations;

namespace RegistroDeTickets.web.Models
{
    public class ReporteViewModel
    {
        [Required, MinLength(10)]
        public string Descripcion { get; set; }
        [Required]
        public int IdTicket { get; set; }
    }
}
