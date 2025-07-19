using Allinone.BLL.Diarys;
using Allinone.Domain.Diarys.DiaryDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DiaryDetailController(IDiaryDetailService _diaryDetailService) : ControllerBase
    {
        [HttpGet]
        [Route("GetDiaryDetailsByDiary/{diaryId}")]
        public async Task<IActionResult> getShopDiariesByShopAsync(int diaryId)
        {
            var response = await _diaryDetailService.GetAllDtoByDiaryIDAsync(diaryId);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetDiaryDetails")]
        public async Task<IActionResult> GetDiaryDetails()
        {
            var response = await _diaryDetailService.GetAllDtoByMemberAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryDetailService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _diaryDetailService.GetByMemberAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DiaryDetailAddReq req)
        {
            var response = await _diaryDetailService.AddAsync(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryDetailAddReq req)
        {
            var response = await _diaryDetailService.UpdateAsync(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _diaryDetailService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
