using Allinone.BLL.Kanbans;
using Allinone.Domain.Kanbans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class KanbanController(IKanbanService kanbanService) : ControllerBase
    {
        [HttpGet]
        [Route("GetKanbans")]
        public async Task<IActionResult> GetKanbansAsync()
        {
            var response = await kanbanService.GetKanbansAsync();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetKanbans()
        {
            var response = await kanbanService.GetKanbansAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await kanbanService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(KanbanAddReq req)
        {
            var response = await kanbanService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, KanbanAddReq req)
        {
            var response = await kanbanService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await kanbanService.Delete(id);
            return Ok(response);
        }
    }
}
