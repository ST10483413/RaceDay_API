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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ResultsController(ApplicationDbContext context) => _context = context;

        // POST /api/results (Organiser captures result)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult Create([FromBody] CreateResultDto dto)
        {
            return BadRequest(new { message = "Use POST /api/enrollments/{id}/results" });
        }

        // POST: /api/enrollments/{id}/results (Organiser only)
        [HttpPost("/api/enrollments/{enrollmentId}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddResultForEnrollment(int enrollmentId, [FromBody] CreateResultDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);
            if (enrollment == null) return NotFound(new { message = "Enrollment not found." });
            if (enrollment.Category!.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            if (await _context.Results.AnyAsync(r => r.EnrollmentID == enrollmentId))
                return Conflict(new { message = "Result already exists for this enrollment." });

            var result = new Result
            {
                EnrollmentID = enrollmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position
            };

            _context.Results.Add(result);
            enrollment.Status = "Completed";
            await _context.SaveChangesAsync();

            return StatusCode(201, new ResultDto
            {
                ResultID = result.ResultID,
                EnrollmentID = result.EnrollmentID,
                FinishTime = result.FinishTime,
                Position = result.Position
            });
        }

        //  PUT: /api/enrollments/{id}/results (Organiser updates a result)
        [HttpPut("/api/enrollments/{enrollmentId}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResultForEnrollment(int enrollmentId, [FromBody] CreateResultDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.UserID == userId);
            if (organiser == null) return Forbid();

            var enrollment = await _context.Enrollments
                .Include(e => e.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId);

            if (enrollment == null) return NotFound(new { message = "Enrollment not found." });
            if (enrollment.Category!.Event!.OrganiserID != organiser.OrganiserID) return Forbid();

            var existingResult = await _context.Results.FirstOrDefaultAsync(r => r.EnrollmentID == enrollmentId);
            if (existingResult == null) return NotFound(new { message = "Result not found to update." });

            existingResult.FinishTime = dto.FinishTime;
            existingResult.Position = dto.Position;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Result updated successfully." });
        }

        // GET: /api/results/me (Participant views own results)
        [HttpGet("me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.UserID == userId);
            if (participant == null) return Forbid();

            var results = await _context.Results
                .Include(r => r.Enrollment).ThenInclude(e => e!.Category).ThenInclude(c => c!.Event)
                .Where(r => r.Enrollment!.ParticipantID == participant.ParticipantID)
                .Select(r => new
                {
                    r.ResultID,
                    FinishTime = r.FinishTime.ToString(@"hh\:mm\:ss"),
                    r.Position,
                    EventName = r.Enrollment!.Category!.Event!.EventName,
                    EventDate = r.Enrollment.Category.Event.EventDate,
                    CategoryName = r.Enrollment.Category.CategoryName
                }).ToListAsync();

            return Ok(results);
        }

        // GET /api/events/{eventId}/results (Public leaderboard)
        [HttpGet("/api/events/{eventId}/results")]
        [Authorize]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId))
                return NotFound();

            var results = await _context.Results
                .Include(r => r.Enrollment).ThenInclude(e => e!.Category)
                .Include(r => r.Enrollment).ThenInclude(e => e!.Participant).ThenInclude(p => p!.User)
                .Where(r => r.Enrollment!.Category!.EventID == eventId)
                .OrderBy(r => r.Position)
                .Select(r => new
                {
                    r.Position,
                    FinishTime = r.FinishTime.ToString(@"hh\:mm\:ss"),
                    Participant = r.Enrollment!.Participant!.User!.FullName,
                    Category = r.Enrollment.Category!.CategoryName
                }).ToListAsync();

            return Ok(results);
        }
    }
}