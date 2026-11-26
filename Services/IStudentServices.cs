
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudentsAsync();

        Task<Student?> GetStudentByIdAsync(int id);

        Task<Student> CreateStudentAsync(CreateStudentDto studentDto);

        Task<bool> UpdateStudentAsync(int id, UpdateStudentDto studentDto);

        Task<bool> DeleteStudentAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null);

        Task<bool> DepartmentExistsAsync(int departmentId);
    }
}
