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
        // Legacy routes (GetTripDetails, GetTrips, addTrip, updateTrip, deleteTrip) are kept for existing clients.
        [HttpGet("GetTripDetails")]
        [HttpGet("GetTrips")]
        public async Task<IActionResult> GetTripDetails() =>
            Ok(await tripService.GetAllDetailsV2Async());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id) =>
            Ok(await tripService.Get(id));

        [HttpPost]
        [HttpPost("addTrip")]
        public async Task<IActionResult> Add(TripAddReq req) =>
            Ok(await tripService.Add(req));

        [HttpPut("{id:int}")]
        [HttpPut("updateTrip/{id:int}")]
        public async Task<IActionResult> Update(int id, TripAddReq req) =>
            Ok(await tripService.Update(id, req));

        [HttpDelete("{id:int}")]
        [HttpDelete("deleteTrip/{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await tripService.Delete(id));
    }
}
