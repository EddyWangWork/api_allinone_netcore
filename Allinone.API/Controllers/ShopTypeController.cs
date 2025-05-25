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
        [HttpGet]
        [Route("GetShopTypes")]
        public async Task<IActionResult> GetShopTypesAsync()
        {
            var response = await shopTypeService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpPost]
        [Route("addShopType")]
        public async Task<IActionResult> AddShopType(ShopTypeAddReq req)
        {
            var response = await shopTypeService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateShopType/{id}")]
        public async Task<IActionResult> UpdateShopType(int id, ShopTypeAddReq req)
        {
            var response = await shopTypeService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteShopType/{id}")]
        public async Task<IActionResult> DeleteShopType(int id)
        {
            var response = await shopTypeService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await shopTypeService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShopTypeAddReq req)
        {
            var response = await shopTypeService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopTypeAddReq req)
        {
            var response = await shopTypeService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await shopTypeService.Delete(id);
            return Ok(response);
        }
    }
}
