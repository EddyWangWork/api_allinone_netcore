using Allinone.BLL.Shops;
using Allinone.Domain.Shops;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShopController(IShopService shopService) : ControllerBase
    {
        [HttpGet]
        [Route("GetShops")]
        public async Task<IActionResult> GetShopsAsync()
        {
            var response = await shopService.GetAllByMemberAsync();
            return Ok(response);
        }

        [HttpPost]
        [Route("addShop")]
        public async Task<IActionResult> AddShop(ShopAddReq req)
        {
            var response = await shopService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateShop/{id}")]
        public async Task<IActionResult> UpdateShop(int id, ShopAddReq req)
        {
            var response = await shopService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteShop/{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            var response = await shopService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await shopService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShopAddReq req)
        {
            var response = await shopService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopAddReq req)
        {
            var response = await shopService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await shopService.Delete(id);
            return Ok(response);
        }
    }
}
