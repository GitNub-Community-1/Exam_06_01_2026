using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIWithJWTAndIdentity.Response;
using Domain.Dtos;
using Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class UserActivityController : ControllerBase
{
    private readonly IUserActivityService _service;

    public UserActivityController(IUserActivityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<UserActivityDto>>>> Get([FromQuery] UserActivityFIlter filter)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;

        var result = await _service.GetUserActivitiesAsync(filter, userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<UserActivityDto>>> GetById(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;

        var result = await _service.GetUserActivityByIdAsync(id, userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Response<UserActivityDto>>> Create([FromBody] UserActivityCreatDto dto)
    {
        var result = await _service.AddUserActivityAsync(dto);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<UserActivityDto>>> Update(int id, [FromBody] UserActivityDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        dto.Id = id;
        var result = await _service.UpdateUserActivityAsync(dto, userId);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<string>>> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.DeleteUserActivityAsync(id, userId);
        return Ok(result);
    }
}
