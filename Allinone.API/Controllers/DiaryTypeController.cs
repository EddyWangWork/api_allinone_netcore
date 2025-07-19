using Allinone.BLL.Diarys;
using Allinone.Domain.Diarys.DiaryTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DiaryTypeController(IDiaryTypeService _diaryTypeService) : ControllerBase
    {
        [HttpGet]
        [Route("GetDiaryTypes")]
        public async Task<IActionResult> GetDiaryTypes()
        {
            var response = await _diaryTypeService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _diaryTypeService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await _diaryTypeService.GetAllByMemberAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DiaryTypeAddReq req)
        {
            var response = await _diaryTypeService.AddAsync(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DiaryTypeAddReq req)
        {
            var response = await _diaryTypeService.UpdateAsync(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _diaryTypeService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
