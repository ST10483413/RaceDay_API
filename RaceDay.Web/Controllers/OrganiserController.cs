using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly IApiService _apiService;

        public OrganiserController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("JwtToken") == null || HttpContext.Session.GetString("UserRole") != "Organiser")
                return RedirectToAction("Login", "Auth");

            ViewBag.Name = HttpContext.Session.GetString("UserFullName");
            var events = await _apiService.GetAsync<List<OrganiserDashboardViewModel>>("api/events/my-events", includeToken: true);
            return View(events ?? new List<OrganiserDashboardViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return View(model);

            var eventData = new { model.EventName, model.Description, model.EventDate, model.Location, model.Distance, model.EventType, model.MaxParticipants };
            var response = await _apiService.PostAsync("api/events", eventData, includeToken: true);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);
                var newEventId = data.GetProperty("eventID").GetInt32();

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    await _apiService.PostFileAsync($"api/events/{newEventId}/upload-image", model.ImageFile, includeToken: true);
                }

                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = "Failed to create event.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCategories(int id)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventDetailsViewModel>($"api/events/{id}", includeToken: true);
            if (ev == null) return NotFound();

            var model = new ManageCategoriesViewModel
            {
                EventID = ev.EventID,
                EventName = ev.EventName,
                Categories = ev.Categories.Select(c => new CategoryItemViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    Distance = c.Distance,
                    AgeGroup = c.AgeGroup
                }).ToList()
            };
            ViewBag.EventId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CreateCategoryViewModel model)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid) return RedirectToAction(nameof(ManageCategories), new { id = model.EventID });

            var response = await _apiService.PostAsync("api/categories", model, includeToken: true);
            if (response.IsSuccessStatusCode) TempData["Success"] = "Category added!";
            else TempData["Error"] = "Failed to add category.";

            return RedirectToAction(nameof(ManageCategories), new { id = model.EventID });
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnrolments(int id)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var ev = await _apiService.GetAsync<EventDetailsViewModel>($"api/events/{id}", includeToken: true);
            if (ev == null) return NotFound();

            var enrolments = await _apiService.GetAsync<List<EnrollmentViewModel>>($"api/enrollments/event/{id}", includeToken: true);

            var model = new ManageEnrolmentsViewModel
            {
                EventID = ev.EventID,
                EventName = ev.EventName,
                Enrolments = enrolments?.Select(e => new EnrollmentItemViewModel
                {
                    EnrollmentID = e.EnrollmentID,
                    ParticipantName = e.ParticipantName ?? "Unknown",
                    CategoryName = e.Category?.CategoryName ?? "Unknown",
                    Status = e.Status,
                    FinishTime = e.Result?.FinishTime,
                    Position = e.Result?.Position
                }).ToList() ?? new()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddResult(AddResultViewModel model)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input. Time must be hh:mm:ss.";
                return RedirectToAction(nameof(ViewEnrolments), new { id = model.EventID });
            }

            var payload = new { FinishTime = model.FinishTime, Position = model.Position };

            // 1. Try to UPDATE an existing result (PUT)
            var response = await _apiService.PutAsync($"api/enrollments/{model.EnrollmentID}/results", payload, includeToken: true);

            // 2. If update fails (meaning no result exists yet), CREATE a new one (POST)
            if (!response.IsSuccessStatusCode)
            {
                response = await _apiService.PostAsync($"api/enrollments/{model.EnrollmentID}/results", payload, includeToken: true);
            }

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Result saved successfully!";
            else
                TempData["Error"] = "Failed to save result.";

            return RedirectToAction(nameof(ViewEnrolments), new { id = model.EventID });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JwtToken") == null) return RedirectToAction("Login", "Auth");
            var response = await _apiService.DeleteAsync($"api/events/{id}", includeToken: true);
            if (response.IsSuccessStatusCode) TempData["Success"] = "Event deleted!";
            else TempData["Error"] = "Failed to delete event.";
            return RedirectToAction(nameof(Index));
        }
    }
}