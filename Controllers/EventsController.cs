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
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context) => _context = context;

        // GET api/events (For Participants gets all events)
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.Events
                .Include(e => e.Categories)
                .Select(e => new
                {
                    e.EventID,
                    e.OrganiserID,
                    e.EventName,
                    e.Description,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.MaxParticipants,
                    e.ImageUrl,
                    Categories = e.Categories.Select(c => new
                    {
                        c.CategoryID,
                        c.CategoryName,
                        c.Distance,
                        c.AgeGroup
                    }).ToList()
                }).ToListAsync();

            return Ok(events);
        }

        // GET api/events/my-events (For Organisers - gets only THEIR events)
        
        [HttpGet("my-events")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var events = await _context.Events
                .Where(e => e.OrganiserID == organiser.OrganiserID)
                .Select(e => new
                {
                    e.EventID,
                    e.EventName,
                    e.EventDate,
                    e.Location,
                    e.Distance,
                    e.EventType,
                    e.MaxParticipants,
                    e.Description,
                    TotalEnrollments = e.Categories.SelectMany(c => c.Enrollments).Count()
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET api/events/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _context.Events
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.EventID == id);

            if (e == null) return NotFound();

            return Ok(new
            {
                e.EventID,
                e.OrganiserID,
                e.EventName,
                e.Description,
                e.EventDate,
                e.Location,
                e.Distance,
                e.EventType,
                e.MaxParticipants,
                e.ImageUrl,
                Categories = e.Categories.Select(c => new
                {
                    c.CategoryID,
                    c.CategoryName,
                    c.Distance,
                    c.AgeGroup
                }).ToList()
            });
        }

        // POST api/events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var ev = new Event
            {
                OrganiserID = organiser.OrganiserID,
                EventName = dto.EventName,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType,
                MaxParticipants = dto.MaxParticipants
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return Ok(new { ev.EventID });
        }

        // DELETE api/events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            if (ev.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return NoContent(); // should return 204 Success
        }
    }
}