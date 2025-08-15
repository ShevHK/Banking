using Banking.BLL.Exceptions;
using Banking.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Banking.BLL.Services
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";


            var response = exception switch
            {
                NotFoundException notFoundEx => ApiResponse<object>.ErrorResponse(
                    notFoundEx.Message, 404, "Resource not found"),
                InsufficientFundsException fundsEx => ApiResponse<object>.ErrorResponse(
                    fundsEx.Message, 400, "Insufficient funds"),
                Banking.BLL.Exceptions.ValidationException validationEx => ApiResponse<object>.ErrorResponse(
                    validationEx.Message, 400, "Validation failed"),
                BusinessException businessEx => ApiResponse<object>.ErrorResponse(
                    businessEx.Message, businessEx.StatusCode),
                UnauthorizedAccessException => ApiResponse<object>.ErrorResponse(
                    "Access denied", 401, "Unauthorized"),
                ArgumentException argEx => ApiResponse<object>.ErrorResponse(
                    argEx.Message, 400, "Invalid argument"),
                _ => ApiResponse<object>.ErrorResponse(
                    "An internal server error occurred", 500, "Internal server error")
            };
            context.Response.StatusCode = response.StatusCode;
            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
