using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Models;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace TrainingCenterApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username exists
                var exists = _context.Students.Any(s => s.Username == model.Username);
                if (exists)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                var student = new Student
                {
                    Username = model.Username,
                    Password = model.Password,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    Role = 0 // Default role
                };

                _context.Students.Add(student);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Students.FirstOrDefault(s =>
                    s.Username == model.Username && s.Password == model.Password);

                if (user != null)
                {
                    // Store user info in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetInt32("Role", user.Role);
                    HttpContext.Session.SetInt32("StudentId", user.StudentId);

                    if (user.Role == 1)
                        return RedirectToAction("Dashboard", "Admin");
                    else
                        return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
