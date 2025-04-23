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

namespace QuanLyTrungTam.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = _context.Students
                .Where(s => s.Role == 0)
                .ToList();
            return View(students);
            // return View(await _context.Students.ToListAsync());
        }

        // GET: Students/Details/5
        // public async Task<IActionResult> Details(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var student = await _context.Students
        //         .FirstOrDefaultAsync(m => m.StudentId == id);
        //     if (student == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(student);
        // }

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
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,FullName,DateOfBirth,PhoneNumber,Email,Username,Password")] Student student)
        {
            // Kiểm tra tên đăng nhập đã tồn tại
            bool usernameExists = await _context.Students.AnyAsync(s => s.Username == student.Username);
            
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
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
        // public async Task<IActionResult> Edit(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var student = await _context.Students.FindAsync(id);
        //     if (student == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(student);
        // }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)  // Đảm bảo load thông tin khóa học qua Enrollments
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            var courses = student.Enrollments
                .Select(e => e.Course) // Lấy các khóa học mà học viên đã đăng ký
                .ToList();

            var viewModel = new StudentDetailsViewModel
            {
                Student = student,
                Courses = courses
            };

            return View(viewModel); // Trả ViewModel vào View
        }



        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FullName,DateOfBirth,PhoneNumber,Email,Username,Password")] Student student)
        {
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
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // [HttpGet]
        // public IActionResult GetRegisteredCourses(int studentId)
        // {
        //     var courses = _context.Enrollments
        //         .Include(r => r.Course)
        //         .Where(r => r.StudentId == studentId)
        //         .Select(r => new {
        //             courseName = r.Course.CourseName,
        //             // description = r.Course.Description,
        //             fee = r.Course.TuitionFee
        //         })
        //         .ToList();

        //     return Json(courses);
        // }

    }
}
