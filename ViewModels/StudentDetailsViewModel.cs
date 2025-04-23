using QuanLyTrungTam.Models;
using System.Collections.Generic;

namespace QuanLyTrungTam.ViewModels
{
    public class StudentDetailsViewModel
    {
        public Student Student { get; set; }
        public List<Course> Courses { get; set; }
    }
}
