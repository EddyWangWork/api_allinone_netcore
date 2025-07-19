using Allinone.BLL.Diarys;
using Allinone.Domain.Diarys.DiaryEmotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DiaryEmotionController(IDiaryEmotionService _diaryEmotionService) : ControllerBase
    {
        [HttpGet]
        [Route("GetDiaryEmotions")]
        public async Task<IActionResult> GetDiaryEmotions()
        {
            var response = await _diaryEmotionService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryEmotionService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _diaryEmotionService.GetAllByMemberAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DiaryEmotionAddReq req)
        {
            var response = await _diaryEmotionService.AddAsync(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryEmotionAddReq req)
        {
            var response = await _diaryEmotionService.UpdateAsync(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _diaryEmotionService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
