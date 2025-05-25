using Allinone.BLL.DS.DSItems;
using Allinone.Domain.DS.DSItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DSItemController(IDSItemService dsItemService) : ControllerBase
    {
        [HttpGet]
        [Route("getDSItemWithSub")]
        public async Task<IActionResult> GetDSItemWithSubV2()
        {
            var response = await dsItemService.GetDSItemWithSubV2();
            return Ok(response);
        }

        [HttpGet]
        [Route("getDSItemWithSubV3")]
        public async Task<IActionResult> GetWithSubsAsync()
        {
            var response = await dsItemService.GetDSItemWithSubV3();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await dsItemService.GetDSItems();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await dsItemService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("addWithSubItem")]
        public async Task<IActionResult> AddWithSub(DSItemAddWithSubItemReq req)
        {
            var response = await dsItemService.AddWithSub(req);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(DSItemAddReq req)
        {
            var response = await dsItemService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DSItemAddReq req)
        {
            var response = await dsItemService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await dsItemService.Delete(id);
            return Ok(response);
        }
    }
}
