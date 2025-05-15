using LIBRARY.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.Entity
{
    public class User:IdentityUser
    {

        [MaxLength(100)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name = "Documento")]
        public string Document { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Usuario")]
        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(50)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name = "Dirección")]
        public string Address { get; set; } = null!;

        [Display(Name = "Tipo de Usuario")]
        public UserType UserType { get; set; }


        public City? City { get; set; }

        [Display (Name = "Ciudad")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una ciudad")]
        public int CityId { get; set; }
    }
}
