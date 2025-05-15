using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/country")]
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> getAllCountries()
        {
            try
            {
                return Ok(await _context.Countries.Include(c => c.States).ToListAsync());
            }
            catch (Exception ex) 
            {
                return BadRequest("Error al listar los países" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> createCountry(Country country)
        {
            try{
                _context.Add(country);
            await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar crear el pais: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> updateCountry(Country country)
        {
            try{
                _context.Update(country);
            await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate")) return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar crear el pais: " + ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> deleteCountryById(int id)
        {
            try
            {
                var afectedRows = await _context.Countries
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
                return BadRequest("Error al intentar eliminar el pais" + ex.Message);
            }

        }
    }
}
