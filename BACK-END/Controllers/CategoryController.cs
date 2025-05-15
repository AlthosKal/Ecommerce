using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/category")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAllCategories()
        {
            try
            {
                return Ok(await _context
                .Categories.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar los ciudades: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> createCategory(Category category)
        {
            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar crear la categoria: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> updateCategory(Category category)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar actualizar la categoria: " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> deleteCategoryById(int id)
        {
            try
            {
                var afectedRows = await _context.Categories
                .Where(c => c.Id == id)
                .ExecuteDeleteAsync();

                if (afectedRows == 0)
                {
                    return NotFound();
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Error al eliminar la ciudad" + ex.Message);
            }
        }
    }
}
