using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.Entity
{
    public class Product
    {
        [Key]
        public int Id {  get; set; }

        public int ProdCategoryId { get; set; }

        [Display(Name = "Producto")]
        [MaxLength(100)]
        [Required(ErrorMessage = "El campo es Obligatorio")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public ProductImage ProductImage { get; set; }
    }
}
