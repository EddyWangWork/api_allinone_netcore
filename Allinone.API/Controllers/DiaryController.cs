using Allinone.BLL.Diarys;
using Allinone.Domain.Diarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DiaryController(IDiaryService _diaryService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryService.GetAllByMemberOrderByDateAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _diaryService.GetByMemberAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DiaryAddReq req)
        {
            var response = await _diaryService.AddAsync(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryAddReq req)
        {
            var response = await _diaryService.UpdateAsync(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _diaryService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
