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
        public async Task<IActionResult> GetAll()
        {
            var response = await installmentService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await installmentService.Get(id);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await installmentService.Delete(id);
            return Ok(response);
        }
    }
}
