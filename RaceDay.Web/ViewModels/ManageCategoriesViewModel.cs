using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.ViewModels
{
    public class ManageCategoriesViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public List<CategoryItemViewModel> Categories { get; set; } = new();
    }

    public class CategoryItemViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string? AgeGroup { get; set; }
    }

    public class CreateCategoryViewModel
    {
        public int EventID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distance is required")]
        [Range(0.1, 1000)]
        public decimal Distance { get; set; }

        public string? AgeGroup { get; set; }
    }
}