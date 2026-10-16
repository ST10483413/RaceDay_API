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
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EnrollmentsController(ApplicationDbContext context) => _context = context;

        // POST: /api/enrollments (Participant only)
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enroll([FromBody] int categoryId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return NotFound(new { message = "Category not found." });

            var exists = await _context.Enrollments
                .AnyAsync(e => e.ParticipantID == participant.ParticipantID && e.CategoryID == categoryId);
            if (exists) return Conflict(new { message = "Already enrolled in this category." });

            var enrollment = new Enrollment
            {
                ParticipantID = participant.ParticipantID,
                CategoryID = categoryId,
                Status = "Registered"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { enrollment.EnrollmentID, enrollment.Status, enrollment.EnrollmentDate });
        }

        // GET: /api/enrollments/me (Participant views own)
        [HttpGet("me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var list = await _context.Enrollments
                .Where(e => e.ParticipantID == participant.ParticipantID)
                .Include(e => e.Category).ThenInclude(c => c!.Event)
                .Include(e => e.Result)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentID = e.EnrollmentID,
                    ParticipantID = e.ParticipantID,
                    CategoryID = e.CategoryID,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Category = new CategoryDto
                    {
                        CategoryID = e.Category!.CategoryID,
                        CategoryName = e.Category.CategoryName,
                        Distance = e.Category.Distance,
                        AgeGroup = e.Category.AgeGroup,
                        EventID = e.Category.EventID
                    },
                    Event = new EventDto
                    {
                        EventID = e.Category.Event!.EventID,
                        EventName = e.Category.Event.EventName,
                        EventDate = e.Category.Event.EventDate,
                        Location = e.Category.Event.Location,
                        EventType = e.Category.Event.EventType,
                        Distance = e.Category.Event.Distance
                    },
                    Result = e.Result == null ? null : new ResultDto
                    {
                        ResultID = e.Result.ResultID,
                        EnrollmentID = e.Result.EnrollmentID,
                        FinishTime = e.Result.FinishTime,
                        Position = e.Result.Position
                    }
                }).ToListAsync();

            return Ok(list);
        }

        // GET: /api/enrollments/event/{eventId} (Organiser views all enrolments for their event)
        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var ev = await _context.Events.FindAsync(eventId);
            if (ev == null) return NotFound();
            if (ev.OrganiserID != organiser.OrganiserID) return Forbid();

            var list = await _context.Enrollments
                .Include(e => e.Participant).ThenInclude(p => p!.User)
                .Include(e => e.Category)
                .Where(e => e.Category!.EventID == eventId)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentID = e.EnrollmentID,
                    ParticipantID = e.ParticipantID,
                    ParticipantName = e.Participant!.User!.FullName,
                    CategoryID = e.CategoryID,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    Category = new CategoryDto
                    {
                        CategoryID = e.Category!.CategoryID,
                        CategoryName = e.Category.CategoryName,
                        Distance = e.Category.Distance,
                        AgeGroup = e.Category.AgeGroup
                    }
                }).ToListAsync();

            return Ok(list);
        }
    }
}