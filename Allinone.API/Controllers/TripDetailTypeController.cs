using Allinone.BLL.Trips;
using Allinone.Domain.Trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TripDetailTypeController(ITripDetailTypeService tripDetailTypeService) : ControllerBase
    {
        [HttpGet]
        [Route("GetTripDetailTypes")]
        public async Task<IActionResult> GetTripDetailTypesAsync()
        {
            var response = await tripDetailTypeService.GetAllAsync();
            return Ok(response);
        }

        [HttpPost]
        [Route("addTripDetailType")]
        public async Task<IActionResult> AddTripDetailType(TripDetailTypeAddReq req)
        {
            var response = await tripDetailTypeService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateTripDetailType/{id}")]
        public async Task<IActionResult> UpdateTripDetailType(int id, TripDetailTypeAddReq req)
        {
            var response = await tripDetailTypeService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteTripDetailType/{id}")]
        public async Task<IActionResult> DeleteTripDetailType(int id)
        {
            var response = await tripDetailTypeService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await tripDetailTypeService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TripDetailTypeAddReq req)
        {
            var response = await tripDetailTypeService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripDetailTypeAddReq req)
        {
            var response = await tripDetailTypeService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await tripDetailTypeService.Delete(id);
            return Ok(response);
        }
    }
}
