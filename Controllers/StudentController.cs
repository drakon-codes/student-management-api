
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();

            var result = students.Select(s => new
            {
                s.StudentId,
                s.FirstName,
                s.LastName,
                s.Email,
                s.Phone,
                s.DateOfBirth,
                s.CreatedAt,
                s.DepartmentId,
                DepartmentName = s.Department?.DepartmentName
            });

            return Ok(result);
        }

        // GET: api/students/7
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);

            if (student == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(new
            {
                student.StudentId,
                student.FirstName,
                student.LastName,
                student.Email,
                student.Phone,
                student.DateOfBirth,
                student.CreatedAt,
                student.DepartmentId,
                DepartmentName = student.Department?.DepartmentName
            });
        }

        // POST: api/students
        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            CreateStudentDto studentDto)
        {
            // Check duplicate email
            var emailExists = await _studentService
                .EmailExistsAsync(studentDto.Email);

            if (emailExists)
            {
                const string message = "This email is already registered.";
                return Conflict(FieldError(nameof(CreateStudentDto.Email), message));
            }

            // Check department
            var departmentExists = await _studentService
                .DepartmentExistsAsync(studentDto.DepartmentId);

            if (!departmentExists)
            {
                const string message = "The selected department does not exist.";
                return BadRequest(FieldError(nameof(CreateStudentDto.DepartmentId), message));
            }

            // Create student
            StudentManagementAPI.Models.Student student;
            try
            {
                student = await _studentService.CreateStudentAsync(studentDto);
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 2601, 2627))
            {
                const string message = "This email is already registered.";
                return Conflict(FieldError(nameof(CreateStudentDto.Email), message));
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 547))
            {
                const string message = "The selected department does not exist.";
                return BadRequest(FieldError(nameof(CreateStudentDto.DepartmentId), message));
            }

            // Fetch the created student with department details
            var createdStudent = await _studentService
                .GetStudentByIdAsync(student.StudentId);

            // Return only the required fields
            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.StudentId },
                new
                {
                    createdStudent!.StudentId,
                    createdStudent.FirstName,
                    createdStudent.LastName,
                    createdStudent.Email,
                    createdStudent.Phone,
                    createdStudent.DateOfBirth,
                    createdStudent.CreatedAt,
                    createdStudent.DepartmentId,
                    DepartmentName = createdStudent.Department?.DepartmentName
                }
            );
        }

        // PUT: api/students/7
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStudent(
            int id,
            UpdateStudentDto studentDto)
        {
            var existingStudent = await _studentService
                .GetStudentByIdAsync(id);

            if (existingStudent == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            // Check duplicate email, excluding current student
            var emailExists = await _studentService
                .EmailExistsAsync(studentDto.Email, id);

            if (emailExists)
            {
                const string message = "This email is already used by another student.";
                return Conflict(FieldError(nameof(UpdateStudentDto.Email), message));
            }

            // Check department
            var departmentExists = await _studentService
                .DepartmentExistsAsync(studentDto.DepartmentId);

            if (!departmentExists)
            {
                const string message = "The selected department does not exist.";
                return BadRequest(FieldError(nameof(UpdateStudentDto.DepartmentId), message));
            }

            bool updated;
            try
            {
                updated = await _studentService.UpdateStudentAsync(id, studentDto);
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 2601, 2627))
            {
                const string message = "This email is already used by another student.";
                return Conflict(FieldError(nameof(UpdateStudentDto.Email), message));
            }
            catch (DbUpdateException exception) when (HasSqlError(exception, 547))
            {
                const string message = "The selected department does not exist.";
                return BadRequest(FieldError(nameof(UpdateStudentDto.DepartmentId), message));
            }

            if (!updated)
            {
                return NotFound(new { message = "Student not found." });
            }

            return Ok(new
            {
                message = "Student updated successfully."
            });
        }

        // DELETE: api/students/7
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return NoContent();
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
