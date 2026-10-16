using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(20)]
        public string Status { get; set; } = "Registered";

        [ForeignKey("ParticipantID")]
        public Participant? Participant { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}