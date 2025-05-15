using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRARY.Shared.Entity
{
    public class ProdCategory
    {
        [Key]
        public int Id { get; set; } 
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Productos")]
        public int ProductsNumber => Products == null ? 0 : Products.Count();
    }
}
