using RaceDay_API.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDay_API.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Organiser? Organiser { get; set; }
        public Participant? Participant { get; set; }
    }
}