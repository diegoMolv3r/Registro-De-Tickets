using System.ComponentModel.DataAnnotations;

namespace RegistroDeTickets.web.Models
{
    public class RestablecerContraseniaViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_\-\.#])[A-Za-z\d@$!%*?&_\-\.#]{8,}$",
        ErrorMessage = "La contraseña debe contener al menos: una mayúscula, una minúscula, un número y un carácter especial (@$!%*?&_-.#).")]
        public string NuevaContrasenia { get; set; }

        [DataType(DataType.Password)]
        [Compare("NuevaContrasenia", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasenia { get; set; }
    }
}
