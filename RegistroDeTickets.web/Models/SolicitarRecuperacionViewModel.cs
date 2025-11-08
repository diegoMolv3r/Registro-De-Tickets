using System.ComponentModel.DataAnnotations;

namespace RegistroDeTickets.web.Models
{
    public class SolicitarRecuperacionViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; }
    }
}
