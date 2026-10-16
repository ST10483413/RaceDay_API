using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}