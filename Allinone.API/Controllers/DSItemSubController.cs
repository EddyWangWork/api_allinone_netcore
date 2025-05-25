using Allinone.BLL.DS.DSItems;
using Allinone.Domain.DS.DSItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DSItemSubController(IDSItemSubService dsItemSubService) : ControllerBase
    {
        [HttpGet]
        [Route("getAllDSSubItems")]
        public async Task<IActionResult> GetAllDSSubItems()
        {
            var response = await dsItemSubService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await dsItemSubService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DSItemSubAddReq req)
        {
            var response = await dsItemSubService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DSItemSubAddReq req)
        {
            var response = await dsItemSubService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await dsItemSubService.Delete(id);
            return Ok(response);
        }
    }
}
