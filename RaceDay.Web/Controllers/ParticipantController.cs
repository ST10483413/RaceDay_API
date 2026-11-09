using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IApiService _apiService;

        public ParticipantController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            ViewBag.Name = HttpContext.Session.GetString("UserFullName");
            var events = await _apiService.GetAsync<List<EventViewModel>>("api/events", includeToken: true);
            return View(events ?? new List<EventViewModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventDetailsViewModel>($"api/events/{id}", includeToken: true);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int categoryId, int eventId)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var response = await _apiService.PostAsync("api/enrollments", categoryId, includeToken: true);
            if (response.IsSuccessStatusCode) TempData["Success"] = "You have successfully enrolled!";
            else TempData["Error"] = "Could not enrol.";
            return RedirectToAction("Details", new { id = eventId });
        }

        public async Task<IActionResult> MyEvents()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var enrolments = await _apiService.GetAsync<List<EnrollmentViewModel>>("api/enrollments/me", includeToken: true);
            return View(enrolments ?? new List<EnrollmentViewModel>());
        }

        public async Task<IActionResult> MyResults()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var results = await _apiService.GetAsync<List<ResultViewModel>>("api/results/me", includeToken: true);
            return View(results ?? new List<ResultViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var profile = await _apiService.GetAsync<UserProfileViewModel>("api/users/profile", includeToken: true);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");

            var updateData = new { model.FullName, model.Email };
            await _apiService.PutAsync("api/users/profile", updateData, includeToken: true);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                await _apiService.PostFileAsync("api/users/upload-profile-picture", model.ImageFile, includeToken: true);
            }

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }
    }
}