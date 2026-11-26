using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly StudentDbContext _context;

        public DepartmentService(StudentDbContext context)
        {
            _context = context;
        }

        public Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            return _context.Departments
                .AsNoTracking()
                .OrderBy(department => department.DepartmentName)
                .Select(department => new DepartmentDto
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName,
                    StudentCount = department.Students.Count
                })
                .ToListAsync();
        }

        public Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
        {
            return _context.Departments
                .AsNoTracking()
                .Where(department => department.DepartmentId == id)
                .Select(department => new DepartmentDto
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName,
                    StudentCount = department.Students.Count
                })
                .FirstOrDefaultAsync();
        }

        public Task<bool> DepartmentNameExistsAsync(string name, int? exceptDepartmentId = null)
        {
            var normalizedName = name.Trim();
            return _context.Departments.AnyAsync(department =>
                department.DepartmentName == normalizedName &&
                (!exceptDepartmentId.HasValue || department.DepartmentId != exceptDepartmentId.Value));
        }

        public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto departmentDto)
        {
            var department = new Department
            {
                DepartmentName = departmentDto.DepartmentName.Trim()
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                StudentCount = 0
            };
        }

        public async Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto departmentDto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department is null)
            {
                return null;
            }

            department.DepartmentName = departmentDto.DepartmentName.Trim();
            await _context.SaveChangesAsync();

            return await GetDepartmentByIdAsync(id);
        }

        public async Task<DepartmentDeleteResult> DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department is null)
            {
                return DepartmentDeleteResult.NotFound;
            }

            if (await _context.Students.AnyAsync(student => student.DepartmentId == id))
            {
                return DepartmentDeleteResult.HasStudents;
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return DepartmentDeleteResult.Deleted;
        }
    }
}
