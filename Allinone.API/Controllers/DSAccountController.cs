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

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id) =>
            Ok(await dsAccountService.Get(id));

        [HttpPost]
        public async Task<IActionResult> Add(DSAccountAddReq req) =>
            Ok(await dsAccountService.Add(req));

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, DSAccountAddReq req) =>
            Ok(await dsAccountService.Update(id, req));

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await dsAccountService.Delete(id));
    }
}
