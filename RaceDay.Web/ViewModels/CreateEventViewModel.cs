using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace RaceDay.Web.ViewModels
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Event Name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string EventName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Event Date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(30);

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distance is required")]
        [Range(0.1, 1000, ErrorMessage = "Distance must be between 0.1 and 1000 km")]
        public decimal Distance { get; set; }

        [Required(ErrorMessage = "Event Type is required")]
        public string EventType { get; set; } = "Run";

        [Required(ErrorMessage = "Max Participants is required")]
        [Range(1, 100000)]
        public int MaxParticipants { get; set; } = 100;

        public IFormFile? ImageFile { get; set; }
    }
}