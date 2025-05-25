using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Allinone.Domain;
using Allinone.Helper.Extension;

namespace Allinone.API.Filters
{
    public class ValidateModelFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var apiResponse = new ApiResponse(null)
                {
                    Success = false,
                    Message = context.ModelState
                        .Select(x => x.Value?.Errors.FirstOrDefault()?.ErrorMessage)
                        .Where(x => x.IsNotNullOrEmpty())
                        .FirstOrDefault()
                };

                context.Result = new JsonResult(apiResponse)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

                //var customResponse = new BaseResponse
                //{
                //    Result = 0,
                //    Resultmessage = context.ModelState
                //        .Select(x => x.Value?.Errors.FirstOrDefault()?.ErrorMessage)
                //        .FirstOrDefault()
                //};

                //context.Result = new JsonResult(customResponse)
                //{
                //    StatusCode = StatusCodes.Status400BadRequest
                //};
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
