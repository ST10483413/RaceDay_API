using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST /Auth/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Send registration data to the API
            var response = await _apiService.PostAsync("api/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction(nameof(Login));
            }

            // If it fails (eg return email has already exists)
            var error = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Registration failed: {error}";
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(content);

                var token = data.GetProperty("token").GetString();
                var role = data.GetProperty("user").GetProperty("role").GetString();
                var fullName = data.GetProperty("user").GetProperty("fullName").GetString();

                //  EXTRACT USER ID FROM JSON
                var userId = data.GetProperty("user").GetProperty("userID").GetInt32();

                HttpContext.Session.SetString("JwtToken", token!);
                HttpContext.Session.SetString("UserRole", role!);
                HttpContext.Session.SetString("UserFullName", fullName!);

                //  SAVE USER ID TO SESSION
                HttpContext.Session.SetInt32("UserID", userId);

                if (role == "Organiser")
                    return RedirectToAction("Index", "Organiser");
                else
                    return RedirectToAction("Index", "Participant");
            }

            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}