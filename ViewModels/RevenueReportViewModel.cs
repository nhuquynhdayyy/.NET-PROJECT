using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTam.ViewModels
{
    public class RevenueReportViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int StudentCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
