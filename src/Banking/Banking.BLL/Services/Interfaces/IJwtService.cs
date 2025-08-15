using System.Security.Claims;

namespace Banking.BLL.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string email);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
