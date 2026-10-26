using Allinone.BLL.Shops;
using Allinone.Domain.Shops.ShopDiarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ShopDiaryController(IShopDiaryService shopDiaryService) : ControllerBase
    {
        [HttpGet("getShopDiaries")]
        public async Task<IActionResult> GetShopDiaries() =>
            Ok(await shopDiaryService.GetShopDiaries());

        [HttpGet("getShopDiariesByShop/{shopId}")]
        public async Task<IActionResult> GetShopDiariesByShop(int shopId) =>
            Ok(await shopDiaryService.GetShopDiaries(shopId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) =>
            Ok(await shopDiaryService.Get(id));

        [HttpPost]
        [HttpPost("addShopDiary")]
        public async Task<IActionResult> Add(ShopDiaryAddReq req) =>
            Ok(await shopDiaryService.Add(req));

        [HttpPut("{id}")]
        [HttpPut("updateShopDiary/{id}")]
        public async Task<IActionResult> Update(int id, ShopDiaryAddReq req) =>
            Ok(await shopDiaryService.Update(id, req));

        [HttpDelete("{id}")]
        [HttpDelete("deleteShopDiary/{id}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await shopDiaryService.Delete(id));
    }
}
