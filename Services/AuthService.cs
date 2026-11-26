using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentManagementAPI.Data;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly StudentDbContext _context;
        private readonly IPasswordHasher<AdminUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            StudentDbContext context,
            IPasswordHasher<AdminUser> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim();
            var adminUser = await _context.AdminUsers
                .SingleOrDefaultAsync(admin => admin.Email == email, cancellationToken);

            if (adminUser is null)
            {
                return null;
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(
                adminUser,
                adminUser.PasswordHash,
                request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, request.Password);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetExpiryMinutes());
            var signingKey = _configuration["Jwt:SigningKey"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var claims = new[]
            {
                new Claim("AdminUserId", adminUser.AdminUserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, adminUser.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new LoginResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expiresAtUtc,
                AdminUserId = adminUser.AdminUserId,
                Email = adminUser.Email
            };
        }

        private int GetExpiryMinutes()
        {
            return int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var expiryMinutes)
                ? expiryMinutes
                : 30;
        }
    }
}
