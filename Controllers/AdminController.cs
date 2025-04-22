using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using QuanLyTrungTam.Data; 
using QuanLyTrungTam.Models;
using QuanLyTrungTam.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTrungTam.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Chỉ Admin mới được truy cập
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetInt32("Role");

            if (role != 1)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // ViewBag.TotalStudents = _context.Students.Count();
            // ViewBag.TotalCourses = _context.Courses.Count();
            // ViewBag.TotalEnrollments = _context.Enrollments.Count();
            var model = new DashboardViewModel
            {
                TotalStudents = _context.Students.Count(),
                TotalCourses = _context.Courses.Count(),
                TotalEnrollments = _context.Enrollments.Count()
            };
            return View(model);

            // return View();
        }

        public IActionResult RevenueReport(DateTime? startDate, DateTime? endDate)
        {
            // Kiểm tra quyền Admin từ Session
            if (HttpContext.Session.GetInt32("Role") != 1)
                return RedirectToAction("AccessDenied", "Account");

            var enrollments = _context.Enrollments.AsQueryable();

            // Lọc theo ngày nếu có
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

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(data);
        }

        public IActionResult ViewStudents(int courseId)
        {
            if (HttpContext.Session.GetInt32("Role") != 1)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var students = _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == courseId)
                .ToList() // Lấy ra khỏi DB trước (chuyển sang LINQ to Objects)
                .Select((e, index) => new StudentInCourseViewModel
                {
                    STT = index + 1,
                    StudentId = e.Student.StudentId,
                    FullName = e.Student.FullName,
                    Phone = e.Student.PhoneNumber,
                    RegisteredAt = e.RegisteredAt
                })
                .ToList();


            ViewBag.CourseId = courseId;
            return View(students); // Trỏ tới Views/Admin/ViewStudents.cshtml
        }

    }
}
