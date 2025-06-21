using Allinone.BLL.Diarys;
using Allinone.Domain.Diarys.DiaryBooks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DiaryBookController(IDiaryBookService _diaryBookService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryBookService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _diaryBookService.GetAllByMemberAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DiaryBookAddReq req)
        {
            var response = await _diaryBookService.AddAsync(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryBookAddReq req)
        {
            var response = await _diaryBookService.UpdateAsync(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _diaryBookService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
