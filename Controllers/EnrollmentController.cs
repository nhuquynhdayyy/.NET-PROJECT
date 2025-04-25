using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using QuanLyTrungTam.Controllers;

namespace QuanLyTrungTam.Controllers
{
    public class EnrollmentController : BaseController
    {
        private readonly AppDbContext _context;

        public EnrollmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register(int courseId)
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingEnrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                TempData["Message"] = "Bạn đã đăng ký khoá học này rồi.";
                return RedirectToAction("MyCourses", "Students");
            }

            int enrolledCount = _context.Enrollments
                .Count(e => e.CourseId == courseId);

            var course = _context.Courses.Find(courseId);

            if (course == null)
            {
                return NotFound();
            }

            // if (course.StartDate <= DateTime.Today)
            // {
            //     TempData["Message"] = "Khóa học này đã bắt đầu. Đăng ký đã đóng.";
            //     return RedirectToAction("MyCourses", "Students");
            // }

            if (enrolledCount >= course.MaxStudents)
            {
                TempData["Message"] = "Khóa học này đã đạt số lượng học viên tối đa.";
                return RedirectToAction("MyCourses", "Students");
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId.Value,
                CourseId = courseId,
                RegisteredAt = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Đăng ký thành công!";
            return RedirectToAction("MyCourses", "Students");
        }
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
                TempData["Message"] = "Không thể hủy đăng ký sau khi khóa học đã bắt đầu.";
                return RedirectToAction("MyCourses", "Students");
            }

            _context.Enrollments.Remove(enrollment);
            _context.SaveChanges();

            TempData["Message"] = "Đăng ký đã bị hủy.";
            return RedirectToAction("MyCourses", "Students");
        }
    }
}
