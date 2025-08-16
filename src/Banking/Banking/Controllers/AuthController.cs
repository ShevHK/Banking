using Banking.Attributes;
using Banking.BLL.Models;
using Banking.BLL.Models.Auth;
using Banking.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Banking.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        [RequireApiKey]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            //TODO
            //enybody with api-key and basically any email can start session, has to be changed and check email if it is somebody from out partners table, also api-keys have to be unique for every
            _logger.LogInformation("Login attempt for email {Email}", request.Email);
            
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new Banking.BLL.Exceptions.ValidationException("Email is required");
            }

            var token = _jwtService.GenerateToken(request.Email);
            var authResponse = new AuthResponse
            {
                Token = token,
                Email = request.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _logger.LogInformation("Login successful for email {Email}", request.Email);

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Login successful"));
        }

        [HttpPost("validate")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateToken()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Token validation successful for email {Email}", email);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Token is valid"));
        }
    }
}
