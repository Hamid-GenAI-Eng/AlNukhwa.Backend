using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misan.Modules.Profiles.Application.Features.Health.Commands;
using Misan.Modules.Profiles.Application.Features.Health.Queries;
using Misan.Modules.Profiles.Application.Features.Profiles.Commands;
using Misan.Modules.Profiles.Application.Features.Profiles.Queries;
using Misan.Shared.Kernel.Abstractions;
using System;
using System.Threading.Tasks;

namespace Misan.Modules.Profiles.Infrastructure.Controllers;

[Route("api/profile")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ISender _sender;

    public ProfileController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
        // Extract userId from Claims. Assuming "userId" generic claim or "sub"
        var claim = User.FindFirst("userId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _sender.Send(new GetProfileQuery(GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        if (command.UserId == Guid.Empty) command = command with { UserId = GetUserId() };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealthProfile()
    {
        var result = await _sender.Send(new GetHealthProfileQuery(GetUserId()));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("health")]
    public async Task<IActionResult> UpdateHealthProfile([FromBody] UpdateHealthProfileCommand command)
    {
        if (command.UserId == Guid.Empty) command = command with { UserId = GetUserId() };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("health/conditions")]
    public async Task<IActionResult> AddCondition([FromBody] AddConditionCommand command)
    {
        if (command.UserId == Guid.Empty) command = command with { UserId = GetUserId() };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("health/conditions/{id}")]
    public async Task<IActionResult> RemoveCondition(Guid id)
    {
        var result = await _sender.Send(new RemoveConditionCommand(GetUserId(), id));
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("health/allergies")]
    public async Task<IActionResult> AddAllergy([FromBody] AddAllergyCommand command)
    {
        if (command.UserId == Guid.Empty) command = command with { UserId = GetUserId() };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("emergency-contact")]
    public async Task<IActionResult> UpdateEmergencyContact([FromBody] UpdateEmergencyContactCommand command)
    {
        if (command.UserId == Guid.Empty) command = command with { UserId = GetUserId() };
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
