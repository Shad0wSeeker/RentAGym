using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using RentAGym.Application.TenantUseCases;
using RentAGym.Domain.Entities;
using RentAGym.UI.rc2.Components.Pages;
using Newtonsoft.Json;

namespace RentAGym.UI.rc2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommonController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("data/chatRoom")]
        public async Task<IActionResult> ChatChatRoomData([FromQuery] int rentBorderId)
        {
            var result = await _mediator.Send(new ChatRoomDataRequest(rentBorderId));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("chat/history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] string name)
        {
            var result = await _mediator.Send(new GetChatRoomHistoryRequest(name));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("day")]
        public async Task<IActionResult> GetDaySchedule([FromQuery] int hallId, [FromQuery] DateOnly date)
        {
            var result = await _mediator.Send(new GetDayScheduleRequest(hallId, date));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("hall/{id}")]
        public async Task<IActionResult> GetHallById(int id)
        {
            var result = await _mediator.Send(new GetHallByIdRequest(id));
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            Console.WriteLine(JsonConvert.SerializeObject(result));
            return Ok(JsonConvert.SerializeObject(result, settings));
        }

        [HttpGet("hall/list")]
        public async Task<ActionResult<IEnumerable<HallListRequestDTO>>> GetHallList([FromQuery] HallListFilter filter)
        {
            var result = await _mediator.Send(new GetHallListRequest(filter));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("hall/types")]
        public async Task<ActionResult> GetHallTypes()
        {
            var result = await _mediator.Send(new GetHallTypesRequest());
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("monthSchedule")]
        public async Task<ActionResult> GetMonthSchedule([FromQuery] int hallId, [FromQuery] DateOnly yearMonth)
        {
            var result = await _mediator.Send(new GetMonthScheduleRequest(hallId, yearMonth));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("options")]
        public async Task<ActionResult> GetOptions()
        {
            var result = await _mediator.Send(new GetOptionsListRequest());
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpGet("user/info/{id}")]
        public async Task<ActionResult> GetUserInfo(string id)
        {
            var result = await _mediator.Send(new GetUserInfoRequest(id));
            return Ok(JsonConvert.SerializeObject(result));
        }

        [HttpPost("message")]
        public async Task<ActionResult> SaveMessage([FromBody] ChatMessage msg)
        {
            var result = await _mediator.Send(new SaveMessageRequest(msg));
            return Ok(JsonConvert.SerializeObject(result));
        }

    }
}
