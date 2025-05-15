using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BACK_END.Dto
{
    public class ProductCreateDto
    {
        public int ProdCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public IFormFile ImageFile { get; set; }
    }

}
