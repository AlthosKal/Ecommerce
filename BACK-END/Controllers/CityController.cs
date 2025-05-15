using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/city")]
    public class CityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAllStates()
        {
            try
            {
                return Ok(await _context
                .Cities.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar los ciudades: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> createState(City city)
        {
            try
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return Ok(city);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar crear la ciudad: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> updateState(City city)
        {
            try
            {
                _context.Update(city);
                await _context.SaveChangesAsync();
                return Ok(city);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar actualizar ls ciudad: " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> deleteStateById(int id)
        {
            try
            {
                var afectedRows = await _context.Cities
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
