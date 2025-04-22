using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace QuanLyTrungTam.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly AppDbContext _context;

        public EnrollmentController(AppDbContext context)
        {
            _context = context;
        }

        // Đăng ký khóa học
        [HttpPost]
        public IActionResult Register(int courseId)
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
            {
                // Chưa đăng nhập => chuyển hướng sang trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra xem học viên đã đăng ký khóa học này chưa
            var existingEnrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                TempData["Message"] = "You have already registered for this course.";
                return RedirectToAction("MyCourses", "Students");
            }

            // Kiểm tra số lượng đăng ký
            int enrolledCount = _context.Enrollments
                .Count(e => e.CourseId == courseId);

            var course = _context.Courses.Find(courseId);

            if (course == null)
            {
                return NotFound();
            }

            // if (course.StartDate <= DateTime.Today)
            // {
            //     TempData["Message"] = "This course has already started. Registration is closed.";
            //     return RedirectToAction("MyCourses", "Students");
            // }

            if (enrolledCount >= course.MaxStudents)
            {
                TempData["Message"] = "This course has reached the maximum number of students.";
                return RedirectToAction("MyCourses", "Students");
            }

            // Tạo bản ghi mới
            var enrollment = new Enrollment
            {
                StudentId = studentId.Value,
                CourseId = courseId,
                RegisteredAt = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Successfully registered!";
            return RedirectToAction("MyCourses", "Students");
        }

        // Hủy đăng ký (nếu chưa khai giảng)
        [HttpPost]
        public IActionResult Cancel(int enrollmentId)
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var enrollment = _context.Enrollments
                .Include(e => e.Course)
                .FirstOrDefault(e => e.EnrollmentId == enrollmentId && e.StudentId == studentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            if (enrollment.Course.StartDate <= DateTime.Now)
            {
                TempData["Message"] = "Cannot cancel enrollment after the course has started.";
                return RedirectToAction("MyCourses", "Students");
            }

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Enrollment cancelled.";
            return RedirectToAction("MyCourses", "Students");
        }
    }
}
