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
        [HttpGet]
        [Route("getShopDiaries")]
        public async Task<IActionResult> GetShopDiarysAsync()
        {
            var response = await shopDiaryService.GetShopDiaries();
            return Ok(response);
        }

        [HttpGet]
        [Route("getShopDiariesByShop/{shopId}")]
        public async Task<IActionResult> getShopDiariesByShopAsync(int shopId)
        {
            var response = await shopDiaryService.GetShopDiaries(shopId);
            return Ok(response);
        }

        [HttpPost]
        [Route("addShopDiary")]
        public async Task<IActionResult> AddShopDiary(ShopDiaryAddReq req)
        {
            var response = await shopDiaryService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateShopDiary/{id}")]
        public async Task<IActionResult> UpdateShopDiary(int id, ShopDiaryAddReq req)
        {
            var response = await shopDiaryService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteShopDiary/{id}")]
        public async Task<IActionResult> DeleteShopDiary(int id)
        {
            var response = await shopDiaryService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await shopDiaryService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShopDiaryAddReq req)
        {
            var response = await shopDiaryService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ShopDiaryAddReq req)
        {
            var response = await shopDiaryService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await shopDiaryService.Delete(id);
            return Ok(response);
        }
    }
}
