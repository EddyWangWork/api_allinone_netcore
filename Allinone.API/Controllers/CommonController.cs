using Allinone.BLL;
using Allinone.Domain.Enums;
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
        [HttpGet("getDSTransTypes")]
        public ActionResult<IReadOnlyList<EnumModel>> GetDSTransTypes() =>
            Ok(commonService.GetDSTransTypes());

        [AllowAnonymous]
        [HttpGet("getTodolistTypes")]
        public ActionResult<IReadOnlyList<EnumModel>> GetTodolistTypes() =>
            Ok(commonService.GetTodolistTypes());
    }
}
