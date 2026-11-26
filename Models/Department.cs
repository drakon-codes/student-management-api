
using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        // One department can have multiple students
        public ICollection<Student> Students { get; set; }
            = new List<Student>();
    }
}
