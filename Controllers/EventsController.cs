using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        // GET: /api/events
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.Events
                .Include(e => e.Categories)
                .Select(e => new EventDto
                {
                    EventID = e.EventID,
                    OrganiserID = e.OrganiserID,
                    EventName = e.EventName,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    MaxParticipants = e.MaxParticipants,
                    CreatedAt = e.CreatedAt,
                    Categories = e.Categories.Select(c => new CategoryDto
                    {
                        CategoryID = c.CategoryID,
                        EventID = c.EventID,
                        CategoryName = c.CategoryName,
                        Distance = c.Distance,
                        AgeGroup = c.AgeGroup
                    }).ToList()
                }).ToListAsync();

            return Ok(events);
        }

        // GET: /api/events/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _context.Events
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.EventID == id);
            if (e == null) return NotFound();

            return Ok(new EventDto
            {
                EventID = e.EventID,
                OrganiserID = e.OrganiserID,
                EventName = e.EventName,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                MaxParticipants = e.MaxParticipants,
                CreatedAt = e.CreatedAt,
                Categories = e.Categories.Select(c => new CategoryDto
                {
                    CategoryID = c.CategoryID,
                    EventID = c.EventID,
                    CategoryName = c.CategoryName,
                    Distance = c.Distance,
                    AgeGroup = c.AgeGroup
                }).ToList()
            });
        }

        // POST: /api/events (Organiser only)
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

            return CreatedAtAction(nameof(GetById), new { id = ev.EventID }, new EventDto
            {
                EventID = ev.EventID,
                OrganiserID = ev.OrganiserID,
                EventName = ev.EventName,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                Distance = ev.Distance,
                EventType = ev.EventType,
                MaxParticipants = ev.MaxParticipants,
                CreatedAt = ev.CreatedAt
            });
        }

        // PUT: /api/events/{id} (Organiser only, owns event)
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEventDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            var ev = await _context.Events.FindAsync(id);

            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            ev.EventName = dto.EventName;
            ev.Description = dto.Description;
            ev.EventDate = dto.EventDate;
            ev.Location = dto.Location;
            ev.Distance = dto.Distance;
            ev.EventType = dto.EventType;
            ev.MaxParticipants = dto.MaxParticipants;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            var ev = await _context.Events.FindAsync(id);

            if (ev == null) return NotFound();
            if (organiser == null || ev.OrganiserID != organiser.OrganiserID) return Forbid();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}