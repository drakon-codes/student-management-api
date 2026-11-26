using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [EnableRateLimiting("Login")]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            LoginRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return response is null
                ? Unauthorized(new { message = "Invalid email or password." })
                : Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("validate")]
        public IActionResult Validate()
        {
            return Ok(new { authenticated = true });
        }
    }
}
