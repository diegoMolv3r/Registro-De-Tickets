using System.ComponentModel.DataAnnotations;


namespace RegistroDeTickets.web.Models
{
    [MetadataType(typeof(LoginViewModel))]
    public partial class Usuario
    {

    }
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "El username es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 20 caracteres")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string PasswordHash { get; set; }

    }
}
