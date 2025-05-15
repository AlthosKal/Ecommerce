using BACK_END.Data;
using LIBRARY.Shared.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BACK_END.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                                 .Include(u => u.City)
                                 .ThenInclude(c => c.State)
                                 .ThenInclude(s => s.Country)
                                 .ToListAsync();
        }

        // GET: api/v1/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users
                                     .Include(u => u.City)
                                     .ThenInclude(c => c.State)
                                     .ThenInclude(s => s.Country)
                                     .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/v1/user
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error al guardar usuario: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // PUT: api/v1/user/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
                return BadRequest("El ID proporcionado no coincide con el del usuario.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExist = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExist)
                return NotFound();

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest($"Error de concurrencia: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // DELETE: api/v1/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
