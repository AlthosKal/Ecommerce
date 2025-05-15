using BACK_END.Data;
using BACK_END.Dto;
using BACK_END.Service;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly CloudinaryService _cloudinary;

        public ProductController(ApplicationDbContext context, CloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IActionResult> getAllProducts()
        {
            try
            {
                return Ok(await _context
                .Products.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar los ciudades: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImage)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> createProduct([FromForm] ProductCreateDto dto)
        {
            try
            {
                var uploadResult = await _cloudinary.UploadImageAsync(dto.ImageFile);

                var image = new ProductImage
                {
                    Name = dto.ImageFile.FileName,
                    ImageUrl = uploadResult.SecureUrl.ToString(),
                    ImageId = uploadResult.PublicId
                };

                var product = new Product
                {
                    ProdCategoryId = dto.ProdCategoryId,
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    ProductImage = image
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al crear el producto: " + ex.Message);
            }
        }


        [HttpPut]
        public async Task<IActionResult> updateProduct (Product product)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar actualizar el producto: " + ex.Message);
            }
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> deleteProductById(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImage)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                    return NotFound();

                if (product.ProductImage != null)
                {
                    await _cloudinary.DeleteImageAsync(product.ProductImage.ImageId);
                    _context.ProductImages.Remove(product.ProductImage);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar el producto: " + ex.Message);
            }
        }

    }
}
