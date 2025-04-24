using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Models;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTrungTam.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // return View();
        // var courses = _context.Courses.ToList();
        var courses = _context.Courses
            .Include(c => c.Enrollments) // 👈 dòng quan trọng để lấy số lượng đăng ký
            .ToList();
        return View("CoursesForStudent", courses); 
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // public IActionResult CourseEnrollments()
    // {
    //     var data = _context.Courses
    //         .Select(c => new CourseEnrollmentStatsViewModel
    //         {
    //             Course = c,
    //             EnrollmentCount = c.Enrollments.Count()
    //         })
    //         .ToList();

    //     return View(data);
    // }
}
