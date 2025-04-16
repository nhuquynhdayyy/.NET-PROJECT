using Microsoft.EntityFrameworkCore;
using QuanLyTrungTam.Models;

namespace QuanLyTrungTam.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ánh xạ precision cho TuitionFee
            modelBuilder.Entity<Course>()
                .Property(c => c.TuitionFee)
                .HasColumnType("decimal(18,2)");
        }
    }
}
