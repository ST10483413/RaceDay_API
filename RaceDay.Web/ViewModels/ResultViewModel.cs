namespace RaceDay.Web.ViewModels
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}