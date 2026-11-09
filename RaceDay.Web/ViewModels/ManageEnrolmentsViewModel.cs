using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class ManageEnrolmentsViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public List<EnrollmentItemViewModel> Enrolments { get; set; } = new();
    }

    public class EnrollmentItemViewModel
    {
        public int EnrollmentID { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public string? FinishTime { get; set; }
        public int? Position { get; set; }
    }

    public class AddResultViewModel
    {
        public int EnrollmentID { get; set; }
        public int EventID { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]{2}):([0-5][0-9]):([0-5][0-9])$", ErrorMessage = "Format must be hh:mm:ss")]
        public string FinishTime { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000)]
        public int Position { get; set; }
    }
}