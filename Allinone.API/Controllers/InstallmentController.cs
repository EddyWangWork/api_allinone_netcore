using Allinone.BLL.Installments;
using Allinone.Domain.Installments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class InstallmentController(IInstallmentService installmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool? isActive = null,
            [FromQuery] InstallmentSortBy sortBy = InstallmentSortBy.UpdatedTime)
        {
            var response = await installmentService.GetAllAsync(isActive, sortBy);
            return Ok(response);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var response = await installmentService.GetSummaryAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await installmentService.Get(id);
            return Ok(response);
        }

        [HttpGet("{id}/schedule")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var response = await installmentService.GetScheduleAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(InstallmentAddReq req)
        {
            var response = await installmentService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InstallmentAddReq req)
        {
            var response = await installmentService.Update(id, req);
            return Ok(response);
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var response = await installmentService.ToggleActiveAsync(id);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await installmentService.Delete(id);
            return Ok(response);
        }
    }
}
