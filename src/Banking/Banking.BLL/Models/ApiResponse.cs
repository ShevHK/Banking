namespace Banking.BLL.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> ErrorResponse(string errorMessage, int statusCode = 400, string message = "Operation failed")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }
}
