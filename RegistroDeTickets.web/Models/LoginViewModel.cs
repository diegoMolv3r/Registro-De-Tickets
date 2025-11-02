using System.ComponentModel.DataAnnotations;

namespace RegistroDeTickets.web.Models
{
    public class LoginViewModel
    {
       
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string PasswordHash { get; set; }
    }
}
