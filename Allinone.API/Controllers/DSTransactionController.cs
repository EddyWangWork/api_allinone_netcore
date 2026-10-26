using Allinone.BLL.DS.Transactions;
using Allinone.Domain.DS.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DSTransactionController(IDSTransactionService dsTransactionService) : ControllerBase
    {
        [HttpPost("getDSTransactionV2")]
        [HttpPost("getDSTransactionWithDate")]
        public async Task<IActionResult> GetDSTransactionWithDateAsync(DSTransactionWithDateReq req)
        {
            return Ok(await dsTransactionService.GetDSTransactionAsyncV3(req));
        }

        [HttpPost("getDSTransactionsByDSAccount")]
        public async Task<IActionResult> GetDSTransactionsByDSAccountAsync(GetDSTransactionAsyncV2Req req)
        {
            return Ok(await dsTransactionService.GetDSTransactionByDSAccountAsync(req));
        }

        [HttpGet("getAllByMemberId")]
        public async Task<IActionResult> GetAllByMemberIdAsync()
        {
            return Ok(await dsTransactionService.GetAllByMemberIdAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await dsTransactionService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add(DSTransactionReq req)
        {
            return Ok(await dsTransactionService.Add(req));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DSTransactionReq req)
        {
            return Ok(await dsTransactionService.Update(id, req));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await dsTransactionService.Delete(id));
        }
    }
}
