using Banking.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Banking.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireApiKeyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedApiKey = configuration["AuthApiKey:X-Api-Key"];

            if (string.IsNullOrEmpty(expectedApiKey))
            {
                context.Result = new ObjectResult(ApiResponse<object>.ErrorResponse(
                    "API Key configuration missing", 500))
                {
                    StatusCode = 500
                };
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var providedApiKey))
            {
                context.Result = new ObjectResult(ApiResponse<object>.ErrorResponse(
                    "X-Api-Key header is required", 401))
                {
                    StatusCode = 401
                };
                return;
            }

            if (providedApiKey != expectedApiKey)
            {
                context.Result = new ObjectResult(ApiResponse<object>.ErrorResponse(
                    "Invalid API Key", 401))
                {
                    StatusCode = 401
                };
                return;
            }
        }
    }
}
