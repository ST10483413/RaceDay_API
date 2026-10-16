namespace RaceDay_API.DTOs
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? AgeGroup { get; set; }
    }

    public class CreateCategoryDto
    {
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? AgeGroup { get; set; }
    }
}