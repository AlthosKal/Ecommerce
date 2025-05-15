using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.Entity
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public int StateId { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(100)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        public string Name { get; set; }

        public State State { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
