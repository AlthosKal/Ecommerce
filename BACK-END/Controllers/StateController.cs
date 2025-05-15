using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/state")]
    public class StateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAllStates()
        {
            try
            {
                return Ok(await _context
                .States.Include(c => c.Cities).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest("Error al listar los estados/departamentos: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> createState(State state)
        {
            try
            {
                _context.Add(state);
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate"))return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar crear el estado/departamento: " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> updateState(State state)
        {
            try
            {
                _context.Update(state);
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException.Message.Contains("duplicate"))return BadRequest("Ya hay un registro con el mismo Nombre");

                return BadRequest(dbEx.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al intentar actualizar el estado/departamento: " + ex.Message);
            }
    }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> deleteStateById(int id)
        {
            try
            {
                var afectedRows = await _context.States
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
                return BadRequest("Error al eliminar el estado/departamento" + ex.Message);
            }
        }

    }
}
