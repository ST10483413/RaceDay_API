using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}