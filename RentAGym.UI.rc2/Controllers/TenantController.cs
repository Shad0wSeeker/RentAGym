using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentAGym.Application.TenantUseCases;
using RentAGym.Domain.Entities;
using RentAGym.UI.rc2.Components.Pages;

namespace RentAGym.UI.rc2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TenantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("t/rents/{tenantId}")]
        public async Task<IActionResult> GetTenantRents(string tenantId)
        {
            var rent = await _mediator.Send(new GetTenantRentsRequest(tenantId));
            return Ok(JsonConvert.SerializeObject(rent));
        }

        [HttpPost("review")]
        public async Task<IActionResult> AddReview([FromBody] Review review)
        {
            var _review = await _mediator.Send(new AddReviewRequest(review));
            return Ok();
        }

        [HttpPost("rent")]
        public async Task<IActionResult> RegisterRent([FromBody] string tenantId, int hallId, bool isRegular, DateTime from, DateTime to)
        {
            var rent = await _mediator.Send(new RegisterRentRequest(tenantId, hallId, isRegular, from, to));
            return Ok();
        }
        /*public async Task<IActionResult> RegisterRent([FromBody] RegisterRentRequest request)
        {
            var rent = await _mediator.Send(request);
            return Ok();
        }*/


    } 
}
