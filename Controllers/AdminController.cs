using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyTrungTam.Data; 
using QuanLyTrungTam.Models;
using QuanLyTrungTam.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTam.Controllers;

namespace QuanLyTrungTam.Controllers
{
    public class AdminController : BaseController
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetInt32("Role");

            if (role != 1)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new DashboardViewModel
            {
                TotalStudents = _context.Students.Count(s => s.Role == 0),
                TotalCourses = _context.Courses.Count(),
                TotalEnrollments = _context.Enrollments.Count()
            };
            return View(model);

        }

        public IActionResult RevenueReport(DateTime? startDate, DateTime? endDate)
        {
            if (HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("AccessDenied", "Account");

            var enrollments = _context.Enrollments.AsQueryable(); //áp dụng các phép toán truy vấn động

            if (startDate.HasValue)
                enrollments = enrollments.Where(e => e.RegisteredAt >= startDate.Value);
            if (endDate.HasValue)
                enrollments = enrollments.Where(e => e.RegisteredAt <= endDate.Value);

            var data = enrollments
                .GroupBy(e => new { e.CourseId, e.Course.CourseName })
                .Select(g => new RevenueReportViewModel
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.CourseName,
                    StudentCount = g.Count(),
                    Revenue = g.Count() * g.First().Course.TuitionFee
                })
                .ToList();
            
            ViewBag.StartDate = startDate; //thộc tính động
            ViewBag.EndDate = endDate;

            return View(data);
        }

        public IActionResult ViewStudents(int courseId)
        {
            if (HttpContext.Session.GetInt32("Role") != 1)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var course = _context.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null)
            {
                return NotFound(); 
            }

            var students = _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .ToList() //load từ database vào bộ nhớ để hỗ trợ index
                .Select((e, index) => new StudentInCourseViewModel
                {
                    STT = index + 1,
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Phone = e.Student.PhoneNumber,
                    RegisteredAt = e.RegisteredAt
                })
                .ToList();

            ViewBag.Course = course;
            ViewBag.CourseId = courseId;
            return View(students); 
        }
        public IActionResult CourseEnrollments()
        {
            var data = _context.Courses
                .Select(c => new CourseEnrollmentStatsViewModel
                {
                    Course = c,
                    EnrollmentCount = c.Enrollments.Count()
                })
                .ToList();

            return View(data);
        }
    }
}
