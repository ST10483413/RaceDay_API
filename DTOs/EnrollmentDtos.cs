using RaceDay_API.DTOs;

namespace RaceDay_API.DTOs
{
    public class EnrollmentDto
    {
        public int EnrollmentID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public CategoryDto? Category { get; set; }
        public EventDto? Event { get; set; }
        public ResultDto? Result { get; set; }
        public string? ParticipantName { get; set; }
    }
}