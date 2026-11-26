
using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.DTOs
{
    public class CreateStudentDto
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^(?!\p{N}+$)[\s\S]+$", ErrorMessage = "First name cannot contain only numbers.")]
        public string FirstName { get => _firstName; set => _firstName = value?.Trim() ?? string.Empty; }

        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^(?!\p{N}+$)[\s\S]+$", ErrorMessage = "Last name cannot contain only numbers.")]
        public string LastName { get => _lastName; set => _lastName = value?.Trim() ?? string.Empty; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get => _email; set => _email = value?.Trim() ?? string.Empty; }

        [IndianMobilePhone(ErrorMessage = "Enter a valid 10-digit Indian mobile number, optionally with +91.")]
        public string? Phone { get; set; }

        [NotFutureDate(ErrorMessage = "Date of birth cannot be in the future.")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }
    }
}
