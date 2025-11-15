using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Allinone.Domain;

namespace Allinone.API.Filters
{
    public class GlobalResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ssss = "";
            // No action needed before execution
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                // Wrap the result in ApiResponse
                var apiResponse = new ApiResponse(objectResult.Value)
                {
                    Message = objectResult.StatusCode == 200 ? "Request successful" : objectResult.StatusCode.ToString()
                };

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new ApiResponse(null));
            }
        }
    }

    //public class ApiResponse
    //{
    //    public bool Success { get; set; } = true;
    //    public string Message { get; set; } = "Request successful";
    //    public object? Data { get; set; }

    //    public ApiResponse(object? data) => Data = data;
    //}
}
