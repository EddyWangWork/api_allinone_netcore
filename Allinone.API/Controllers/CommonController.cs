using Allinone.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommonController(ICommonService commonService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("getDSTransTypes")]
        public async Task<IActionResult> Get()
        {
            var response = await commonService.GetDSTransTypes();
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("getTodolistTypes")]
        public async Task<IActionResult> GetTodolistTypes()
        {
            var response = await commonService.GetTodolistTypes();
            return Ok(response);
        }
    }
}
