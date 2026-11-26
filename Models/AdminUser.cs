using System.ComponentModel.DataAnnotations;

namespace StudentManagementAPI.Models
{
    public class AdminUser
    {
        [Key]
        public int AdminUserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
