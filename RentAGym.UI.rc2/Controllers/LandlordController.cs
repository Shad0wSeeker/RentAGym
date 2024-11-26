using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentAGym.Application.LandLordUseCases;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RentAGym.UI.rc2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandlordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LandlordController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("facility")]
        public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityRequestDTO requestDTO)
        {
            var facility = await _mediator.Send(new CreateFacilityRequest(requestDTO));
            return Ok();    
        }

        [HttpPost("hall")]
        public async Task<IActionResult> CreateHall([FromBody] CreateHallRequestDTO requestDTO)
        {
            var facility = await _mediator.Send(new CreateHallRequest(requestDTO));
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditFacility([FromBody] EditHallRequestDTO requestDTO)
        {
            var facility = await _mediator.Send(new EditHallRequest(requestDTO));
            return Ok();
        }

        [HttpGet("ll/facilities/{landlordId}")]
        public async Task<IActionResult> GetFacilities(string landlordId)
        {
            var facility = await _mediator.Send(new GetFacilitiesListRequest(landlordId));
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return Ok(JsonConvert.SerializeObject(facility, settings));
        }

        [HttpGet("ll/rents/{landlordId}")]
        public async Task<IActionResult> GetHallRents(string landlordId)
        {
            var rents = await _mediator.Send(new GetHallRentsRequest(landlordId));
            return Ok(JsonConvert.SerializeObject(rents));
        }

        [HttpGet("ll/halls")]
        public async Task<IActionResult> GetHallsByFacilityId([FromQuery] int facilityId)
        {
            var halls = await _mediator.Send(new GetHallsByFacilityIdRequest(facilityId));
            return Ok(JsonConvert.SerializeObject(halls));
        }

    }
}
