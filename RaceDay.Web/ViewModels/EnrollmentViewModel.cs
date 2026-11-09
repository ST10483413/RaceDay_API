namespace RaceDay.Web.ViewModels
{
    public class EnrollmentViewModel
    {
        public int EnrollmentID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;

        //  shows the participant's name on the Organiser page
        public string? ParticipantName { get; set; }

        public CategoryWithEventViewModel Category { get; set; } = new();
        public EventViewModel Event { get; set; } = new();

        // ADD THIS so we can map the result from the API
        public ResultViewModel? Result { get; set; }
    }

    public class CategoryWithEventViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}