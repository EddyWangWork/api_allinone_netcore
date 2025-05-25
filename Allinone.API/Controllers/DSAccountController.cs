using Allinone.BLL.DS.Accounts;
using Allinone.Domain.DS.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DSAccountController(IDSAccountService dsAccountService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await dsAccountService.GetDSAccountsWithBalance();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await dsAccountService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DSAccountAddReq req)
        {
            var response = await dsAccountService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DSAccountAddReq req)
        {
            var response = await dsAccountService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await dsAccountService.Delete(id);
            return Ok(response);
        }
    }
}
