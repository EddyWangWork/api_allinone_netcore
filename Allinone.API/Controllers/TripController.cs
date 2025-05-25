using Allinone.BLL.Shops;
using Allinone.BLL.Trips;
using Allinone.Domain.Trips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TripController(ITripService tripService) : ControllerBase
    {
        [HttpGet]
        [Route("GetTripDetails")]
        public async Task<IActionResult> GetAllDetailsV2Async()
        {
            var response = await tripService.GetAllDetailsV2Async();
            return Ok(response);
        }

        [HttpGet]
        [Route("GetTrips")]
        public async Task<IActionResult> GetTripsAsync()
        {
            var response = await tripService.GetAllDetailsV2Async();
            return Ok(response);
        }

        [HttpPost]
        [Route("addTrip")]
        public async Task<IActionResult> AddTrip(TripAddReq req)
        {
            var response = await tripService.Add(req);
            return Ok(response);
        }

        [HttpPut("updateTrip/{id}")]
        public async Task<IActionResult> UpdateTrip(int id, TripAddReq req)
        {
            var response = await tripService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("deleteTrip/{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var response = await tripService.Delete(id);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await tripService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TripAddReq req)
        {
            var response = await tripService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TripAddReq req)
        {
            var response = await tripService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await tripService.Delete(id);
            return Ok(response);
        }
    }
}
