using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_API.Data;
using RaceDay_API.DTOs;
using RaceDay_API.Models;
using System.Security.Claims;

namespace RaceDay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context) => _context = context;

        // GET: /api/categories
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Categories.Select(c => new CategoryDto
            {
                CategoryID = c.CategoryID,
                EventID = c.EventID,
                CategoryName = c.CategoryName,
                Distance = c.Distance,
                AgeGroup = c.AgeGroup
            }).ToListAsync();
            return Ok(list);
        }

        // GET: /api/events/{eventId}/categories
        [HttpGet("/api/events/{eventId}/categories")]
        [Authorize]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId))
                return NotFound(new { message = "Event not found." });

            var list = await _context.Categories
                .Where(c => c.EventID == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryID = c.CategoryID,
                    EventID = c.EventID,
                    CategoryName = c.CategoryName,
                    Distance = c.Distance,
                    AgeGroup = c.AgeGroup
                }).ToListAsync();
            return Ok(list);
        }

        // POST: /api/categories (Organiser only)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var ev = await _context.Events.FindAsync(dto.EventID);
            if (ev == null) return NotFound(new { message = "Event not found." });
            if (ev.OrganiserID != organiser.OrganiserID) return Forbid();

            var category = new Category
            {
                EventID = dto.EventID,
                CategoryName = dto.CategoryName,
                Distance = dto.Distance,
                AgeGroup = dto.AgeGroup
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new CategoryDto
            {
                CategoryID = category.CategoryID,
                EventID = category.EventID,
                CategoryName = category.CategoryName,
                Distance = category.Distance,
                AgeGroup = category.AgeGroup
            });
        }

        // DELETE: /api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            var category = await _context.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return NotFound();
            if (organiser == null || category.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}