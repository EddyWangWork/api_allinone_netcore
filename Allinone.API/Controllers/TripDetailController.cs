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
        [Route("GetTripDetails")]
        public async Task<IActionResult> GetTripDetailsAsync()
        {
            var response = await tripDetailService.GetAllAsync();
            return Ok(response);
        }

        [HttpPost]
        [Route("addTripDetail")]
        public async Task<IActionResult> AddTripDetail(TripDetailAddReq req)
        {
            var response = await tripDetailService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateTripDetail/{id}")]
        public async Task<IActionResult> UpdateTripDetail(int id, TripDetailAddReq req)
        {
            var response = await tripDetailService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteTripDetail/{id}")]
        public async Task<IActionResult> DeleteTripDetail(int id)
        {
            var response = await tripDetailService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await tripDetailService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TripDetailAddReq req)
        {
            var response = await tripDetailService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripDetailAddReq req)
        {
            var response = await tripDetailService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await tripDetailService.Delete(id);
            return Ok(response);
        }
    }
}
