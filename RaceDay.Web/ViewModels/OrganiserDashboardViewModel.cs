namespace RaceDay.Web.ViewModels
{
    public class OrganiserDashboardViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public int MaxParticipants { get; set; }
        public int TotalEnrollments { get; set; }
    }
}