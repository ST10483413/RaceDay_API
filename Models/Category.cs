using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required, StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        [StringLength(20)]
        public string? AgeGroup { get; set; }

        [ForeignKey("EventID")]
        public Event? Event { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}