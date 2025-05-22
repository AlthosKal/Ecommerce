using System.ComponentModel.DataAnnotations;

namespace LIBRARY.Shared.DTO
{
    public class LoginDTO
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo")]
        [EmailAddress(ErrorMessage = "El correo no es valido")]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Contraseña")]
        [StringLength(maximumLength: 20, MinimumLength = 6
            , ErrorMessage = "La contraseña debe tener minimo seis caracteres")]
        public required string Password { get; set; }
    }
}
