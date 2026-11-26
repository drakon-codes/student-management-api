using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Services
{
    public enum DepartmentDeleteResult
    {
        Deleted,
        NotFound,
        HasStudents
    }

    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllDepartmentsAsync();
        Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
        Task<bool> DepartmentNameExistsAsync(string name, int? exceptDepartmentId = null);
        Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto departmentDto);
        Task<DepartmentDto?> UpdateDepartmentAsync(int id, UpdateDepartmentDto departmentDto);
        Task<DepartmentDeleteResult> DeleteDepartmentAsync(int id);
    }
}
