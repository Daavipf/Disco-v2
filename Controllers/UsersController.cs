using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Disco.Models;
using Disco.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Disco.Extensions;

namespace Disco.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return await _context.Users
                .Select(u => MapToDTO(u))
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return NotFound();

            return MapToDTO(user);
        }

        // PUT: api/Users/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDTO userDto)
        {
            if (id != userDto.Id) return BadRequest("ID do usuário inconsistente.");

            var currentUserId = User.GetId();
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = userDto.Name;
            user.Bio = userDto.Bio;
            user.Avatar = userDto.Avatar;
            user.Updatedat = DateTime.UtcNow;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Users (Apenas Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            user.Createdat = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToDTO(user));
        }

        // DELETE: api/Users/5 (Apenas Admin)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH ou DELETE: api/Users/me/deactivate
        [Authorize]
        [HttpDelete("me/deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = User.GetId();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (user.Deletedat != null)
            {
                return BadRequest("Esta conta já está desativada.");
            }

            user.Deletedat = DateTime.UtcNow;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id) => _context.Users.Any(e => e.Id == id);

        private static UserDTO MapToDTO(User user) => new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Isverified = user.Isverified,
            Createdat = user.Createdat,
            Updatedat = user.Updatedat,
            Role = user.Role,
            Bio = user.Bio,
            Avatar = user.Avatar
        };
    }
}