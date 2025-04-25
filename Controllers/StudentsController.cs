using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.Models;
using QuanLyTrungTam.ViewModels; 
using QuanLyTrungTam.Controllers;

namespace QuanLyTrungTam.Controllers
{
    public class StudentsController : BaseController
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetInt32("Role");
            return userRole == 1;
        }
        private bool IsStudent()
        {
            var userRole = HttpContext.Session.GetInt32("Role");
            return userRole == 0;
        }
        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var students = _context.Students
                .Where(s => s.Role == 0)
                .ToList();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return NotFound();

            var courses = student.Enrollments
                .Select(e => e.Course)
                .ToList();

            var viewModel = new StudentDetailsViewModel
            {
                Student = student,
                Courses = courses
            };

            return View(viewModel);
        }


        // GET: Students/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FullName,DateOfBirth,PhoneNumber,Email,Username,Password")] Student student)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            bool usernameExists = await _context.Students.AnyAsync(s => s.Username == student.Username);
            bool emailExists = await _context.Students.AnyAsync(s => s.Email == student.Email);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
                return View(student);
            }
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng nhập email khác.");
                return View(student);
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsStudent())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course) 
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            var courses = student.Enrollments
                .Select(e => e.Course) 
                .ToList();

            var viewModel = new StudentDetailsViewModel
            {
                Student = student,
                Courses = courses
            };

            return View(viewModel); 
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FullName,DateOfBirth,PhoneNumber,Email,Username,Password")] Student student)
        {
            if (!IsStudent())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var role = HttpContext.Session.GetInt32("Role");

                if (role == 1)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Details", new { id = student.StudentId });
                }
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        public IActionResult MyCourses()
        {
            var username = HttpContext.Session.GetString("Username");
            var student = _context.Students.FirstOrDefault(s => s.Username == username);

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var courseList = _context.Enrollments
                .Where(e => e.StudentId == student.StudentId)
                .Include(e => e.Course)
                .Select(e => new MyCourseViewModel
                {
                    EnrollmentId = e.EnrollmentId,
                    Course = e.Course
                })
                .ToList();

            return View(courseList);
        }
    }
}
