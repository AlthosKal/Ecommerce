using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.Entity
{
    public class State
    {
        [Key]
        public int Id { get; set; }
        public int CountryId { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        [Display(Name ="Departamento/Estado")]
        public string Name { get; set; }    

        public Country Country { get; set; }

        public ICollection<City> Cities{ get; set; }

        [Display(Name = "Ciudades")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count();
    }
}
