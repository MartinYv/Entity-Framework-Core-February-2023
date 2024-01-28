using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }
        public StudentSystemContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasKey(sc => new { sc.CourseId, sc.StudentId });
            });

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasOne(s => s.Student).
                WithMany(ce => ce.CourseEnrollments).
                HasForeignKey(st => st.StudentId);
            });

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasOne(c => c.Course).
                WithMany(st => st.StudentsEnrolled).
                HasForeignKey(c => c.CourseId);
            });

        }
    }
}
