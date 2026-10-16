namespace RaceDay_API.DTOs
{
    public class ResultDto
    {
        public int ResultID { get; set; }
        public int EnrollmentID { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
    }

    public class CreateResultDto
    {
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
    }
}