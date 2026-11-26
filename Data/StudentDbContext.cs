
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        {
        }

        // Represents the Students table
        public DbSet<Student> Students { get; set; }

        // Represents the Departments table
        public DbSet<Department> Departments { get; set; }

        public DbSet<AdminUser> AdminUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students", "dbo");

                entity.HasKey(s => s.StudentId);

                entity.HasIndex(s => s.Email).IsUnique();

                entity.Property(s => s.CreatedAt)
                    .HasDefaultValueSql("SYSDATETIME()")
                    .ValueGeneratedOnAdd();

                entity.HasOne(s => s.Department)
                    .WithMany(d => d.Students)
                    .HasForeignKey(s => s.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments", "dbo");

                entity.HasKey(d => d.DepartmentId);

                entity.HasIndex(d => d.DepartmentName).IsUnique();
            });

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("AdminUsers", "dbo");

                entity.HasKey(admin => admin.AdminUserId);

                entity.Property(admin => admin.Email)
                    .HasMaxLength(256)
                    .IsRequired();

                entity.HasIndex(admin => admin.Email)
                    .IsUnique()
                    .HasDatabaseName("UX_AdminUsers_Email");

                entity.Property(admin => admin.PasswordHash)
                    .HasMaxLength(512)
                    .IsRequired();

                entity.Property(admin => admin.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()")
                    .ValueGeneratedOnAdd();
            });
        }
    }
}
