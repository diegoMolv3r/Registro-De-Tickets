using Newtonsoft.Json.Serialization;
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
        [EmailAddress(ErrorMessage = "Ingrese un email válido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "La contraseña debe tener entre 4 y 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{4,8}$",
        ErrorMessage = "La contraseña debe tener al menos una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&)")]
        public string PasswordHash { get; set; }

    }
}
