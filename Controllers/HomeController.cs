using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTam.Models;
using QuanLyTrungTam.Data;
using QuanLyTrungTam.ViewModels;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTam.Controllers;

namespace QuanLyTrungTam.Controllers;

public class HomeController : BaseController
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
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult CoursesForStudent()
    {
        var courses = _context.Courses
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student) 
            .ToList();
        return View("CoursesForStudent", courses); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
