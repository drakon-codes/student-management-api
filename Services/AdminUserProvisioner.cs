using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagementAPI.Data;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public class AdminUserProvisioner
    {
        private readonly StudentDbContext _context;
        private readonly IPasswordHasher<AdminUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AdminUserProvisioner(
            StudentDbContext context,
            IPasswordHasher<AdminUser> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task EnsureInitialAdminAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.AdminUsers.AnyAsync(cancellationToken))
            {
                return;
            }

            var email = _configuration["InitialAdmin:Email"]?.Trim();
            var password = _configuration["InitialAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                email.Length > 256 ||
                !new EmailAddressAttribute().IsValid(email))
            {
                throw new InvalidOperationException(
                    "A valid initial admin email must be configured as InitialAdmin:Email before first startup.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "An initial admin password must be configured as InitialAdmin:Password before first startup.");
            }

            var adminUser = new AdminUser { Email = email };
            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, password);

            _context.AdminUsers.Add(adminUser);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
