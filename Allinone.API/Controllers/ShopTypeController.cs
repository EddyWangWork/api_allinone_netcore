using Allinone.BLL.Shops;
using Allinone.Domain.Shops.ShopTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShopTypeController(IShopTypeService shopTypeService) : ControllerBase
    {
        // Legacy routes kept for backward compatibility
        [HttpGet]
        [HttpGet("GetShopTypes")]
        public async Task<IActionResult> GetShopTypes()
        {
            var response = await shopTypeService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await shopTypeService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        [HttpPost("addShopType")]
        public async Task<IActionResult> Add(ShopTypeAddReq req)
        {
            var response = await shopTypeService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [HttpPut("updateShopType/{id}")]
        public async Task<IActionResult> Update(int id, ShopTypeAddReq req)
        {
            var response = await shopTypeService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [HttpDelete("deleteShopType/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await shopTypeService.Delete(id);
            return Ok(response);
        }
    }
}
