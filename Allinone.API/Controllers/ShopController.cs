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
        [HttpGet("GetShops")]
        public async Task<IActionResult> GetShopsAsync() =>
            Ok(await shopService.GetAllByMemberAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id) =>
            Ok(await shopService.Get(id));

        // "addShop" kept as a legacy alias for existing clients
        [HttpPost]
        [HttpPost("addShop")]
        public async Task<IActionResult> Add(ShopAddReq req) =>
            Ok(await shopService.Add(req));

        [HttpPut("{id:int}")]
        [HttpPut("updateShop/{id:int}")]
        public async Task<IActionResult> Update(int id, ShopAddReq req) =>
            Ok(await shopService.Update(id, req));

        [HttpDelete("{id:int}")]
        [HttpDelete("deleteShop/{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await shopService.Delete(id));
    }
}
