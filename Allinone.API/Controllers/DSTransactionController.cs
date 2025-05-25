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
        [HttpGet("getDSMonthlyItemExpenses")]
        public async Task<IActionResult> GetDSMonthlyItemExpensesAsync(int year, int month, int monthDuration)
        {
            return Ok(await dsTransactionService.GetDSMonthlyItemExpensesAsync(year, month, monthDuration));
        }

        [HttpPost("getDSMonthlyPeriodCreditDebit")]
        public async Task<IActionResult> GetDSMonthlyPeriodCreditDebitAsync(GetDSMonthlyPeriodCreditDebitReq req)
        {
            return Ok(await dsTransactionService.GetDSMonthlyPeriodCreditDebitAsync(req));
        }

        [HttpPost("getDSMonthlyCommitmentAndOther")]
        public async Task<IActionResult> GetDSMonthlyCommitmentAndOtherAsync(GetDSMonthlyCommitmentAndOtherReq req)
        {
            return Ok(await dsTransactionService.GetDSMonthlyCommitmentAndOtherAsync(req));
        }


        [HttpGet("getDSYearExpenses")]
        public async Task<IActionResult> GetDSYearExpenses(int year)
        {
            return Ok(await dsTransactionService.GetDSYearExpensesAsync(year));
        }

        [HttpGet("getDSYearCreditDebitDiff")]
        public async Task<IActionResult> GetDSYearCreditDebitDiff(int year)
        {
            return Ok(await dsTransactionService.GetDSYearCreditDebitDiffAsync(year));
        }

        [HttpGet("getDSMonthlyExpenses")]
        public async Task<IActionResult> GetDSMonthlyExpensesAsync(int year, int month)
        {
            return Ok(await dsTransactionService.GetDSMonthlyExpensesAsync(year, month));
        }


        [HttpPost]
        [Route("getDSTransactionV2")]
        public async Task<IActionResult> GetDSTransactionsAsync(DSTransactionWithDateReq req)
        {
            var response = await dsTransactionService.GetDSTransactionAsyncV3(req);
            return Ok(response);
        }

        [HttpPost("getDSTransactionWithDate")]
        public async Task<IActionResult> GetDSTransactionWithDateAsync(DSTransactionWithDateReq req)
        {
            return Ok(await dsTransactionService.GetDSTransactionAsyncV3(req));
        }

        [HttpPost]
        [Route("getDSTransactionsByDSAccount")]
        public async Task<IActionResult> GetDSTransactionsByDSAccountAsync(GetDSTransactionAsyncV2Req req)
        {
            var response = await dsTransactionService.GetDSTransactionByDSAccountAsync(req);
            return Ok(response);
        }

        [HttpGet]
        [Route("getAllByMemberId")]
        public async Task<IActionResult> GetAllByMemberIdAsync()
        {
            var response = await dsTransactionService.GetAllByMemberIdAsync();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await dsTransactionService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DSTransactionReq req)
        {
            var response = await dsTransactionService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DSTransactionReq req)
        {
            var response = await dsTransactionService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await dsTransactionService.Delete(id);
            return Ok(response);
        }
    }
}
