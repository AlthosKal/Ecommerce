using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.DTO
{
    public class EmailDTO
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo")]
        public string Email { get; set; }
    }
}
