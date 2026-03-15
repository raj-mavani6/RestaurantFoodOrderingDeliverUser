using Microsoft.AspNetCore.Mvc;
using RestaurantFoodOrderingDeliverUser.Data;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Controllers
{
    public class AuthController : Controller
    {
        private readonly IDeliveryUserRepository _userRepository;

        public AuthController(IDeliveryUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: Login Page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Email and password are required";
                return View();
            }

            try
            {
                // Try database authentication first
                var user = await _userRepository.GetByEmailAsync(email);
                
                if (user != null && user.Password == password)
                {
                    // Set session
                    HttpContext.Session.SetString("DeliveryUserId", user.DeliveryUserId.ToString());
                    HttpContext.Session.SetString("DeliveryUserName", user.FullName ?? "User");
                    HttpContext.Session.SetString("DeliveryUserEmail", email);

                    return RedirectToAction("Dashboard", "Home");
                }
                
                // Fallback to mock authentication for testing
                if (email == "delivery@example.com" && password == "password123")
                {
                    HttpContext.Session.SetString("DeliveryUserId", "1");
                    HttpContext.Session.SetString("DeliveryUserName", "Test User");
                    HttpContext.Session.SetString("DeliveryUserEmail", email);

                    return RedirectToAction("Dashboard", "Home");
                }
            }
            catch (Exception ex)
            {
                // If database fails, fallback to mock authentication
                if (email == "delivery@example.com" && password == "password123")
                {
                    HttpContext.Session.SetString("DeliveryUserId", "1");
                    HttpContext.Session.SetString("DeliveryUserName", "Test User");
                    HttpContext.Session.SetString("DeliveryUserEmail", email);

                    return RedirectToAction("Dashboard", "Home");
                }
                
                ViewBag.ErrorMessage = $"Login error: {ex.Message}";
                return View();
            }

            ViewBag.ErrorMessage = "Invalid email or password";
            return View();
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
