using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIWithJWTAndIdentity.Response;
using Domain.Dtos;
using Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class ActivityTypeController : ControllerBase
{
    private readonly IActivityTypeService _service;

    public ActivityTypeController(IActivityTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<ActivityTypeDto>>>> Get([FromQuery] ActivityTypeFilter filter)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;
        var result = await _service.GetActivityTypesAsync(filter,userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<ActivityTypeDto>>> GetById(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;
        var result = await _service.GetActivityTypeByIdAsync(id,userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Response<ActivityTypeDto>>> Create([FromBody] ActivityTypeCreatDto dto)
    {
        var result = await _service.AddActivityTypeAsync(dto);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<Response<ActivityTypeDto>>> Update([FromBody] ActivityTypeDto dto)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;
        var result = await _service.UpdateActivityTypeAsync(dto,userId);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<string>>> Delete(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value)
            : 0;

        var result = await _service.DeleteActivityTypeAsync(id, userId);
        return Ok(result);
    }
}