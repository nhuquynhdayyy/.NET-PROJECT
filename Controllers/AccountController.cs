using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Models;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.ViewModels;
using System.Security.Cryptography;
using System.Text;
using QuanLyTrungTam.Controllers;

namespace TrainingCenterApp.Controllers
{
    public class AccountController : BaseController
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
                // // Check if username exists
                // var exists = _context.Students.Any(s => s.Username == model.Username);
                // if (exists)
                // {
                //     ModelState.AddModelError("Username", "Username already exists.");
                //     return View(model);
                // }
                // Kiểm tra username đã tồn tại chưa
                if (_context.Students.Any(s => s.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã được sử dụng.");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa
                if (_context.Students.Any(s => s.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
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

                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập.";
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
                // Tìm theo username hoặc email
                var user = _context.Students.FirstOrDefault(s =>
                    (s.Username == model.Username || s.Email == model.Username) &&
                    s.Password == model.Password);

                if (user != null)
                {
                    if (model.RememberMe)
                    {
                        // Ghi nhớ đăng nhập qua cookie
                        CookieOptions option = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(7), // Sống 7 ngày
                            HttpOnly = true,
                            IsEssential = true
                        };

                        Response.Cookies.Append("Username", user.Username, option);
                        Response.Cookies.Append("Role", user.Role.ToString(), option);
                        Response.Cookies.Append("StudentId", user.StudentId.ToString(), option);
                    }
                    else
                    {
                        // Nếu không ghi nhớ thì lưu bằng session
                        HttpContext.Session.SetString("Username", user.Username);
                        HttpContext.Session.SetInt32("Role", user.Role);
                        HttpContext.Session.SetInt32("StudentId", user.StudentId);
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập/email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Role");
            Response.Cookies.Delete("StudentId");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
