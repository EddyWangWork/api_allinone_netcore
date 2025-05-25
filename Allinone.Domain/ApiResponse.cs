namespace Allinone.Domain
{
    public class ApiResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Request successful";
        public object? Data { get; set; }

        public ApiResponse(object? data) => Data = data;
    }
}
