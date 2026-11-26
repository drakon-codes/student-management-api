
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public class StudentService : IStudentService
    {
        private readonly StudentDbContext _context;

        // Constructor
        public StudentService(StudentDbContext context)
        {
            _context = context;
        }

        // 1. Get all students
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Department)
                .ToListAsync();
        }

        // 2. Get student by ID
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        // 3. Create student
        public async Task<Student> CreateStudentAsync(CreateStudentDto studentDto)
        {
            var student = new Student
            {
                FirstName = studentDto.FirstName.Trim(),
                LastName = studentDto.LastName.Trim(),
                Email = studentDto.Email.Trim(),
                Phone = studentDto.Phone,
                DateOfBirth = studentDto.DateOfBirth,
                DepartmentId = studentDto.DepartmentId
            };

            _context.Students.Add(student);

            await _context.SaveChangesAsync();
            await _context.Entry(student)
            .Reference(s => s.Department)
             .LoadAsync();

            return student;
        }

        // 4. Update student
        public async Task<bool> UpdateStudentAsync(
            int id,
            UpdateStudentDto studentDto)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return false;
            }

            student.FirstName = studentDto.FirstName.Trim();
            student.LastName = studentDto.LastName.Trim();
            student.Email = studentDto.Email.Trim();
            student.Phone = studentDto.Phone;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.DepartmentId = studentDto.DepartmentId;

            await _context.SaveChangesAsync();

            return true;
        }

        // 5. Delete student
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return false;
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return true;
        }
        // Check whether an email already exists
        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeStudentId = null)
        {
            return await _context.Students.AnyAsync(s =>
                s.Email == email.Trim() &&
                (!excludeStudentId.HasValue || s.StudentId != excludeStudentId.Value));
        }

        // Check whether a department exists
        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId);
        }
    }
}
