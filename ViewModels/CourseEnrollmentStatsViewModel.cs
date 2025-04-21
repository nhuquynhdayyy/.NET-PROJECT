using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyTrungTam.Models;  

namespace QuanLyTrungTam.ViewModels
{
    public class CourseEnrollmentStatsViewModel
    {
        // public int CourseId { get; set; }
        // public string CourseName { get; set; }
        public int EnrollmentCount { get; set; }
        public Course Course { get; set; }
    }
}
