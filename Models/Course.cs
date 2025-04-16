using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTam.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        public string Lecturer { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TuitionFee { get; set; }

        [Range(1, 1000)]
        public int MaxStudents { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
