using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.DTOs
{
    public class CreateDepartmentDto
    {
        private string _departmentName = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DepartmentName
        {
            get => _departmentName;
            set => _departmentName = value?.Trim() ?? string.Empty;
        }
    }
}
