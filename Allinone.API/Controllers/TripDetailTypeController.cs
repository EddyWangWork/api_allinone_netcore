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
        [HttpGet("GetTripDetailTypes")]
        public async Task<ActionResult<IEnumerable<TripDetailType>>> GetTripDetailTypesAsync(
            CancellationToken cancellationToken)
        {
            var response = await tripDetailTypeService.GetAllAsync(cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [HttpPost("addTripDetailType")]
        public async Task<ActionResult<TripDetailType>> AddTripDetailType(
            TripDetailTypeAddReq req,
            CancellationToken cancellationToken)
        {
            var response = await tripDetailTypeService.Add(req, cancellationToken);
            return Ok(response);
        }

        [HttpPut("updateTripDetailType/{id}")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TripDetailType>> UpdateTripDetailType(
            int id,
            TripDetailTypeAddReq req,
            CancellationToken cancellationToken)
        {
            var response = await tripDetailTypeService.Update(id, req, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("deleteTripDetailType/{id}")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TripDetailType>> DeleteTripDetailType(
            int id,
            CancellationToken cancellationToken)
        {
            var response = await tripDetailTypeService.Delete(id, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripDetailType>> Get(int id, CancellationToken cancellationToken)
        {
            var response = await tripDetailTypeService.Get(id, cancellationToken);
            return Ok(response);
        }
    }
}
