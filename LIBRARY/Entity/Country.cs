

using System.ComponentModel.DataAnnotations;

namespace LIBRARY.Shared.Entity
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Pais")]
        [MaxLength(100)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        public string Name { get; set; }

        public ICollection<State> States { get; set; }

        [Display(Name ="Estados/Departamentos")]
        public int StatetsNumber => States == null ? 0 : States.Count(); 

    }
}
