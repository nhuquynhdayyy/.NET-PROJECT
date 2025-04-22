using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTam.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
    }

}
