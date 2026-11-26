using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class DepartmentsController : ControllerBase
    {
        private const string DepartmentInUseMessage =
            "This department cannot be deleted while students are assigned to it.";

        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetAllDepartments()
        {
            return Ok(await _departmentService.GetAllDepartmentsAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            return department is null
                ? NotFound(new { message = "Department not found." })
                : Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto departmentDto)
        {
            if (await _departmentService.DepartmentNameExistsAsync(departmentDto.DepartmentName))
            {
                const string message = "A department with this name already exists.";
                return Conflict(FieldError(nameof(CreateDepartmentDto.DepartmentName), message));
            }

            try
            {
                var department = await _departmentService.CreateDepartmentAsync(departmentDto);
                return CreatedAtAction(nameof(GetDepartment), new { id = department.DepartmentId }, department);
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 2601, 2627))
            {
                const string message = "A department with this name already exists.";
                return Conflict(FieldError(nameof(CreateDepartmentDto.DepartmentName), message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, UpdateDepartmentDto departmentDto)
        {
            if (await _departmentService.DepartmentNameExistsAsync(departmentDto.DepartmentName, id))
            {
                const string message = "A department with this name already exists.";
                return Conflict(FieldError(nameof(UpdateDepartmentDto.DepartmentName), message));
            }

            try
            {
                var department = await _departmentService.UpdateDepartmentAsync(id, departmentDto);
                return department is null
                    ? NotFound(new { message = "Department not found." })
                    : Ok(department);
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 2601, 2627))
            {
                const string message = "A department with this name already exists.";
                return Conflict(FieldError(nameof(UpdateDepartmentDto.DepartmentName), message));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                return await _departmentService.DeleteDepartmentAsync(id) switch
                {
                    DepartmentDeleteResult.Deleted => NoContent(),
                    DepartmentDeleteResult.NotFound => NotFound(new { message = "Department not found." }),
                    _ => Conflict(new { message = DepartmentInUseMessage })
                };
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 547))
            {
                return Conflict(new { message = DepartmentInUseMessage });
            }
        }

        private static object FieldError(string field, string message) => new
        {
            message,
            errors = new Dictionary<string, string[]> { [field] = new[] { message } }
        };

        private static bool HasSqlError(DbUpdateException exception, params int[] errorNumbers)
        {
            return exception.GetBaseException() is SqlException sqlException &&
                   errorNumbers.Contains(sqlException.Number);
        }
    }
}
