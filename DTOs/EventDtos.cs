using RaceDay_API.DTOs;

namespace RaceDay_API.DTOs
{
    public class EventDto
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CreateEventDto
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
    }
}