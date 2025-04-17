using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTam.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        
        // Khóa ngoại đến khóa học
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        // Khóa ngoại đến học viên
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
