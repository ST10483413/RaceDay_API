using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int EnrollmentID { get; set; }

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Required, Range(1, 100000)]
        public int Position { get; set; }

        [ForeignKey("EnrollmentID")]
        public Enrollment? Enrollment { get; set; }
    }
}