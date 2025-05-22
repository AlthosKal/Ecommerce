using LIBRARY.Shared.Entity;
using System.ComponentModel.DataAnnotations;

namespace LIBRARY.Shared.DTO
{
    public class UserDTO:User
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Este campo es requerido")]
        [Display(Name = "Contraseña")]
        [StringLength(maximumLength:20, MinimumLength = 6
            , ErrorMessage ="La contraseña debe tener minimo seis caracteres")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Contraseña")]
        [StringLength(maximumLength: 20, MinimumLength = 6
            , ErrorMessage = "La contraseña debe tener minimo seis caracteres")]
        public required string ConfirmPassword { get; set; }
    }
}
