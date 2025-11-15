using Allinone.BLL.Auditlogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuditlogController(IAuditlogService _auditlogService) : ControllerBase
    {
        [HttpGet]
        [Route("GetAuditlogs")]
        public async Task<IActionResult> GetAuditlogs()
        {
            var response = await _auditlogService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _auditlogService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _auditlogService.GetAllByMemberAsync(id);
            return Ok(response);
        }
    }
}
