using Allinone.BLL.Trips;
using Allinone.Domain.Trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TripDetailController(ITripDetailService tripDetailService) : ControllerBase
    {
        [HttpGet]
        [HttpGet("GetTripDetails")]
        public async Task<IActionResult> GetTripDetailsAsync()
        {
            var response = await tripDetailService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("trip/{tripId}")]
        public async Task<IActionResult> GetByTripId(int tripId)
        {
            var response = await tripDetailService.GetByTripIdAsync(tripId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await tripDetailService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        [HttpPost("addTripDetail")]
        public async Task<IActionResult> Add(TripDetailAddReq req)
        {
            var response = await tripDetailService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [HttpPut("updateTripDetail/{id}")]
        public async Task<IActionResult> Update(int id, TripDetailAddReq req)
        {
            var response = await tripDetailService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [HttpDelete("deleteTripDetail/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await tripDetailService.Delete(id);
            return Ok(response);
        }
    }
}