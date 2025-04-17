using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTam.ViewModels
{
    public class CourseEnrollmentStatsViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
