using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/country")]
    public class CountryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            try
            {
                var countries = await _context.Countries
                    .Include(c => c.States)
                    .ToListAsync();

                return Ok(countries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al listar los países: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            try
            {
                var country = await _context.Countries
                    .Include(c => c.States)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (country == null)
                {
                    return NotFound($"No se encontró el país con ID: {id}");
                }

                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener el país: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] Country country)
        {
            try
            {
                if (country == null)
                {
                    return BadRequest("Los datos del país son requeridos");
                }

                if (string.IsNullOrWhiteSpace(country.Name))
                {
                    return BadRequest("El nombre del país es requerido");
                }

                // Verificar si ya existe un país con el mismo nombre
                var existingCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == country.Name.ToLower());

                if (existingCountry != null)
                {
                    return BadRequest("Ya existe un país con ese nombre");
                }

                _context.Countries.Add(country);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCountryById), new { id = country.Id }, country);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("duplicate") == true)
                {
                    return BadRequest("Ya hay un registro con el mismo Nombre");
                }
                return BadRequest($"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al intentar crear el país: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] Country country)
        {
            try
            {
                if (country == null)
                {
                    return BadRequest("Los datos del país son requeridos");
                }

                if (id != country.Id)
                {
                    return BadRequest("El ID del país no coincide");
                }

                if (string.IsNullOrWhiteSpace(country.Name))
                {
                    return BadRequest("El nombre del país es requerido");
                }

                var existingCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existingCountry == null)
                {
                    return NotFound($"No se encontró el país con ID: {id}");
                }

                // Verificar si ya existe otro país con el mismo nombre
                var duplicateCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == country.Name.ToLower() && c.Id != id);

                if (duplicateCountry != null)
                {
                    return BadRequest("Ya existe otro país con ese nombre");
                }

                existingCountry.Name = country.Name;
                // No actualizar States aquí para evitar problemas de tracking

                _context.Countries.Update(existingCountry);
                await _context.SaveChangesAsync();

                return Ok(existingCountry);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException?.Message.Contains("duplicate") == true)
                {
                    return BadRequest("Ya hay un registro con el mismo Nombre");
                }
                return BadRequest($"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al intentar actualizar el país: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCountryById(int id)
        {
            try
            {
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (country == null)
                {
                    return NotFound($"No se encontró el país con ID: {id}");
                }

                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al intentar eliminar el país: {ex.Message}");
            }
        }
    }
}