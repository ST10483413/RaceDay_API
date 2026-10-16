using Microsoft.Extensions.Logging;
using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay_API.Models
{
    public class Organiser
    {
        [Key]
        public int OrganiserID { get; set; }

        [Required]
        public int UserID { get; set; }

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}