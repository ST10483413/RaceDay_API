using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganiserID { get; set; }

        [Required, StringLength(100, MinimumLength = 3)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required, StringLength(255)]
        public string Location { get; set; } = string.Empty;

        [Required, Range(0.1, 1000)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("^(Run|Walk|Cycle)$")]
        public string EventType { get; set; } = string.Empty;

        [Required, Range(1, 100000)]
        public int MaxParticipants { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrganiserID")]
        public Organiser? Organiser { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}