using Allinone.BLL.DS.Transactions;
using Allinone.Domain.DS.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("DSTransaction")]
    public class DSDashboardController(IDSTransactionService dsTransactionService) : ControllerBase
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
    }
}
